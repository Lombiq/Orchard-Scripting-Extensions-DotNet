using Orchard.Environment.Extensions;
using OrchardHUN.Scripting.CSharp.Services;
using OrchardHUN.Scripting.Services;

namespace OrchardHUN.Scripting.DotNet.Services
{
    [OrchardFeature("OrchardHUN.Scripting.CSharp")]
    public class CSharpRuntime : IScriptingRuntime
    {
        private readonly IEngineDescriptor _descriptor = new EngineDescriptor("C#", new Orchard.Localization.LocalizedString("C#"));
        private readonly IDotNetRuntime _runtime;

        public IEngineDescriptor Descriptor
        {
            get { return _descriptor; }
        }


        public CSharpRuntime(IDotNetRuntime runtime)
        {
            _runtime = runtime;
        }


        public dynamic ExecuteExpression(string expression, ScriptScope scope)
        {
            return _runtime.CompileAndRun(Descriptor.Name, expression, scope);
        }
    }
}