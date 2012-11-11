using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Orchard;
using Orchard.Localization;
using OrchardHUN.Scripting.DotNet;
using OrchardHUN.Scripting.Exceptions;
using OrchardHUN.Scripting.Services;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace OrchardHUN.Scripting.CSharp.Services
{
    public interface IDotNetRuntime : IDependency
    {
        dynamic CompileAndRun(string engine, string expression, ScriptScope scope);
    }

    public class DotNetRuntime : IDotNetRuntime
    {
        private readonly IDotNetScriptEventHandler _eventHandler;

        public Localizer T { get; set; }

        public DotNetRuntime(IDotNetScriptEventHandler eventHandler)
        {
            _eventHandler = eventHandler;

            T = NullLocalizer.Instance;
        }

        public dynamic CompileAndRun(string engine, string expression, ScriptScope scope)
        {
            _eventHandler.BeforeCompilation(new BeforeDotNetCompilationContext(scope));

            CompilerParameters parameters = new CompilerParameters() { GenerateInMemory = true, TreatWarningsAsErrors = false };
            foreach (var item in scope.Assemblies) parameters.ReferencedAssemblies.Add(item.Location);

            CompilerResults result;
            var providerOptions = new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } };
            switch (engine)
            {
                case "C#":
                    result = new CSharpCodeProvider(providerOptions).CompileAssemblyFromSource(parameters, expression);
                    break;
                case "VB":
                    result = new VBCodeProvider(providerOptions).CompileAssemblyFromSource(parameters, expression);
                    break;
                default:
                    throw new ArgumentException(T("Undefined .NET scripting engine.").ToString());
            }

            _eventHandler.AfterCompilation(new AfterDotNetCompilationContext(scope, result));

            if (result.Errors.HasErrors)
            {
                var builder = new StringBuilder();
                foreach (var item in result.Errors) builder.Append(Environment.NewLine + item);
                throw new ScriptRuntimeException(Environment.NewLine + T("The following compile error(s) need to be fixed:") + builder.ToString());
            }
            else
            {
                var entryClass = result.CompiledAssembly.CreateInstance("DotNetScripting");

                _eventHandler.BeforeExecution(new BeforeDotNetExecutionContext(scope));

                var scriptResult = entryClass.GetType().GetMethod("Main").Invoke(entryClass, new object[] { });

                _eventHandler.AfterExecution(new AfterDotNetExecutionContext(scope));

                return scriptResult;
            }
        }
    }
}