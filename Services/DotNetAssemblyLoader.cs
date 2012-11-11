using OrchardHUN.Scripting.DotNet;
using System.Reflection;

namespace OrchardHUN.Scripting.CSharp.Services
{
    public class DotNetAssemblyLoader : IDotNetScriptEventHandler
    {
        public void BeforeCompilation(BeforeDotNetCompilationContext context)
        {
            context.Scope.LoadAssembly(typeof(System.Dynamic.CallInfo).Assembly); // System.Core.dll
            context.Scope.LoadAssembly(Assembly.LoadFrom("Microsoft.Csharp.dll"));
        }

        public void AfterCompilation(AfterDotNetCompilationContext context) { }

        public void BeforeExecution(BeforeDotNetExecutionContext context) { }

        public void AfterExecution(AfterDotNetExecutionContext context) { }
    }
}