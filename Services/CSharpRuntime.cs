using Microsoft.CSharp;
using Orchard.Environment.Extensions;
using OrchardHUN.Scripting.Exceptions;
using OrchardHUN.Scripting.Services;
using System.CodeDom.Compiler;
using System.Text;

namespace OrchardHUN.Scripting.DotNet.Services
{
    [OrchardFeature("OrchardHUN.Scripting.CSharp")]
    public class CSharpRuntime : IScriptingRuntime
    {
        private readonly IDotNetScriptEventHandler _eventHandler;

        public string Engine { get { return "C#"; } }

        public CSharpRuntime(IDotNetScriptEventHandler eventHandler)
        {
            _eventHandler = eventHandler;
        }

        public dynamic ExecuteExpression(string expression, ScriptScope scope)
        {
            _eventHandler.BeforeCompilation(new BeforeDotNetCompilationContext(scope));

            CompilerParameters parameters = new CompilerParameters() { GenerateInMemory = true };            
            foreach (var item in scope.Assemblies) parameters.ReferencedAssemblies.Add(item.Location);
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
                var entryClass = result.CompiledAssembly.CreateInstance("CSharpScripting");

                _eventHandler.BeforeExecution(new BeforeDotNetExecutionContext(scope));

                var scriptResult = entryClass.GetType().GetMethod("Main").Invoke(entryClass, new object[] { });

                _eventHandler.AfterExecution(new AfterDotNetExecutionContext(scope));

                return scriptResult;
            }
        }
    }
}