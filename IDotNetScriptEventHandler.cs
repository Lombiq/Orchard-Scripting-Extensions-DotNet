using Orchard.Events;
using OrchardHUN.Scripting.Services;
using System.CodeDom.Compiler;

namespace OrchardHUN.Scripting.DotNet
{
    public interface IDotNetScriptEventHandler : IEventHandler
    {
        void BeforeCompilation(BeforeDotNetCompilationContext context);
        void AfterCompilation(AfterDotNetCompilationContext context);
        void BeforeExecution(BeforeDotNetExecutionContext context);
        void AfterExecution(AfterDotNetExecutionContext context);
    }

    #region Context classes

    public abstract class DotNetScriptingEventContext
    {
        public ScriptScope Scope { get; set; }

        protected DotNetScriptingEventContext(ScriptScope scope)
        {
            Scope = scope;
        }
    }

    public class BeforeDotNetCompilationContext : DotNetScriptingEventContext
    {
        public BeforeDotNetCompilationContext(ScriptScope scope)
            : base(scope) { }
    }

    public class AfterDotNetCompilationContext : DotNetScriptingEventContext
    {
        public CompilerResults Result { get; private set; }

        public AfterDotNetCompilationContext(ScriptScope scope, CompilerResults result)
            : base(scope)
        {
            Result = result;
        }
    }

    public class BeforeDotNetExecutionContext : DotNetScriptingEventContext
    {
        public BeforeDotNetExecutionContext(ScriptScope scope)
            : base(scope) { }
    }

    public class AfterDotNetExecutionContext : DotNetScriptingEventContext
    {
        public AfterDotNetExecutionContext(ScriptScope scope)
            : base(scope) { }
    }

    #endregion
}