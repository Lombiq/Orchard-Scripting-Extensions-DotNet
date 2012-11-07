using Microsoft.VisualBasic;
using Orchard;
using Orchard.Environment.Extensions;
using OrchardHUN.Scripting.Exceptions;
using OrchardHUN.Scripting.Services;
using System.CodeDom.Compiler;
using System.Text;
using System.Web.Hosting;

namespace OrchardHUN.Scripting.DotNet.Services
{
    [OrchardFeature("OrchardHUN.Scripting.VisualBasic")]
    public class VisualBasicRuntime : IScriptingRuntime
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IDotNetScriptEventHandler _eventHandler;

        public VisualBasicRuntime(IOrchardServices orchardServices, IDotNetScriptEventHandler eventHandler)
        {
            _orchardServices = orchardServices;
            _eventHandler = eventHandler;
        }

        public string Engine { get { return "VB"; } }

        public dynamic ExecuteExpression(string expression, ScriptScope scope)
        {
            CompilerParameters parameters = new CompilerParameters() { GenerateInMemory = true };
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Web.dll");

            var appPath = HostingEnvironment.ApplicationPhysicalPath;
            parameters.ReferencedAssemblies.Add(appPath + "/Core/bin/Orchard.Core.dll");
            parameters.ReferencedAssemblies.Add(appPath + "/Core/bin/Orchard.Framework.dll");

            _eventHandler.BeforeCompilation(new BeforeDotNetCompilationContext(scope));
            CompilerResults result = new VBCodeProvider().CompileAssemblyFromSource(parameters, expression);
            _eventHandler.AfterCompilation(new AfterDotNetCompilationContext(scope, result));

            if (result.Errors.HasErrors)
            {
                var builder = new StringBuilder();
                foreach (var item in result.Errors) builder.Append(item + "\n");
                throw new ScriptRuntimeException("The VB code could not be executed.", new System.Exception(builder.ToString()));
            }
            else
            {
                var entryClass = result.CompiledAssembly.CreateInstance("VisualBasicScripting");
                _eventHandler.BeforeExecution(new BeforeDotNetExecutionContext(scope));
                var scriptResult = entryClass.GetType().GetMethod("Main").Invoke(entryClass, new object[] { });
                _eventHandler.AfterExecution(new AfterDotNetExecutionContext(scope));
                return scriptResult;
            }
        }
    }
}