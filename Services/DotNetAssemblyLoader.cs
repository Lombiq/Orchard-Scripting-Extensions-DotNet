using OrchardHUN.Scripting.DotNet;

namespace OrchardHUN.Scripting.CSharp.Services
{
    public class DotNetAssemblyLoader : IDotNetScriptEventHandler
    {
        public void BeforeCompilation(BeforeDotNetCompilationContext context)
        {
            context.Scope.LoadAssembly(typeof(System.Dynamic.CallInfo).Assembly); // System.Core.dll
        }

        public void AfterCompilation(AfterDotNetCompilationContext context) { }

        public void BeforeExecution(BeforeDotNetExecutionContext context) { }

        public void AfterExecution(AfterDotNetExecutionContext context) { }
    }
}