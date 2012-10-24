using Microsoft.CSharp;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using OrchardHUN.Scripting.Exceptions;
using OrchardHUN.Scripting.Services;
using System;
using System.CodeDom.Compiler;
using System.Text;

namespace OrchardHUN.Scripting.CSharp.Services
{
    [OrchardFeature("OrchardHUN.Scripting.CSharp")]
    public class CSharpRuntime : ICSharpRuntime
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IDotNetScriptEventHandler _eventHandler;

        public CSharpRuntime(IOrchardServices orchardServices, IDotNetScriptEventHandler eventHandler)
        {
            _orchardServices = orchardServices;
            _eventHandler = eventHandler;
        }

        public string Engine
        {
            get { return "C#"; }
        }

        public dynamic ExecuteExpression(string expression, ScriptScope scope)
        {
            string code = @"
                using System;
                public class Scripting
                {
                    public static void Script()
                    {" +
                       expression + @"
                    }
                }";

            CompilerParameters parameters = new CompilerParameters() { GenerateInMemory = true };
            _eventHandler.BeforeCompilation(new BeforeDotNetCompilationContext(scope));
            CompilerResults result = new CSharpCodeProvider().CompileAssemblyFromSource(parameters, code);
            _eventHandler.AfterCompilation(new AfterDotNetCompilationContext(scope, result));

            if (result.Errors.HasErrors) throw new ScriptRuntimeException("The C# code could not be executed.");
            else
            {
                object myClass = result.CompiledAssembly.CreateInstance("Scripting");
                _eventHandler.BeforeExecution(new BeforeDotNetExecutionContext(scope));
                myClass.GetType().GetMethod("Script").Invoke(myClass, new object[] { });
                _eventHandler.AfterExecution(new AfterDotNetExecutionContext(scope));
            }

            return true;
        }

        public dynamic ExecuteFile(string path, ScriptScope scope)
        {
            throw new NotImplementedException();
        }
    }
}