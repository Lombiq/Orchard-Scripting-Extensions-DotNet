using Microsoft.CSharp;
using Orchard;
using Orchard.Environment.Extensions;
using OrchardHUN.Scripting.Exceptions;
using OrchardHUN.Scripting.Services;
using System.CodeDom.Compiler;
using System.Text;

namespace OrchardHUN.Scripting.CSharp.Services
{
    [OrchardFeature("OrchardHUN.Scripting.CSharp")]
    public class CSharpRuntime : IScriptingRuntime
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IDotNetScriptEventHandler _eventHandler;

        public CSharpRuntime(IOrchardServices orchardServices, IDotNetScriptEventHandler eventHandler)
        {
            _orchardServices = orchardServices;
            _eventHandler = eventHandler;
        }

        public string Engine { get { return "C#"; } }

        public dynamic ExecuteExpression(string expression, ScriptScope scope)
        {
            CompilerParameters parameters = new CompilerParameters() { GenerateInMemory = true };            
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");

            _eventHandler.BeforeCompilation(new BeforeDotNetCompilationContext(scope));
            CompilerResults result = new CSharpCodeProvider().CompileAssemblyFromSource(parameters, expression);
            _eventHandler.AfterCompilation(new AfterDotNetCompilationContext(scope, result));

            if (result.Errors.HasErrors)
            {
                var builder = new StringBuilder();
                foreach (var item in result.Errors) builder.Append(item + "\n");
                throw new ScriptRuntimeException("The C# code could not be executed.", new System.Exception(builder.ToString()));
            }
            else
            {
                var wrapperClass = result.CompiledAssembly.CreateInstance("CSharpScripting");
                _eventHandler.BeforeExecution(new BeforeDotNetExecutionContext(scope));
                var scriptResult = wrapperClass.GetType().GetMethod("Main").Invoke(wrapperClass, new object[] { });
                _eventHandler.AfterExecution(new AfterDotNetExecutionContext(scope));
                return scriptResult;
            }
        }
    }
}