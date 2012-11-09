using Orchard.Environment.Extensions;
using OrchardHUN.Scripting.CSharp.Services;
using OrchardHUN.Scripting.Services;

namespace OrchardHUN.Scripting.DotNet.Services
{
    [OrchardFeature("OrchardHUN.Scripting.VisualBasic")]
    public class VisualBasicRuntime : IScriptingRuntime
    {
        private readonly IEngineDescriptor _descriptor = new EngineDescriptor("VB", new Orchard.Localization.LocalizedString("Visual Basic"));
        private readonly IDotNetRuntime _runtime;

        public IEngineDescriptor Descriptor
        {
            get { return _descriptor; }
        }
        

        public VisualBasicRuntime(IDotNetRuntime runtime)
        {
            _runtime = runtime;
        }


        public dynamic ExecuteExpression(string expression, ScriptScope scope)
        {
            return _runtime.CompileAndRun(Descriptor.Name, expression, scope);
        }
    }
}