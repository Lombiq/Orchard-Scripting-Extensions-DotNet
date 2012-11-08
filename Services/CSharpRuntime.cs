using Orchard.Environment.Extensions;
using OrchardHUN.Scripting.CSharp.Services;
using OrchardHUN.Scripting.Services;

namespace OrchardHUN.Scripting.DotNet.Services
{
    [OrchardFeature("OrchardHUN.Scripting.CSharp")]
    public class CSharpRuntime : IScriptingRuntime
    {
        private readonly IDotNetRuntime _runtime;

        public string Engine { get { return "C#"; } }

        public CSharpRuntime(IDotNetRuntime runtime)
        {
            _runtime = runtime;
        }

        public dynamic ExecuteExpression(string expression, ScriptScope scope)
        {
            return _runtime.CompileAndRun(Engine, expression, scope);
        }
    }
}