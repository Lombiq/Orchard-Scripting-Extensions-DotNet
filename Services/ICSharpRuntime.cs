using OrchardHUN.Scripting.Models;
using OrchardHUN.Scripting.Services;
using Orchard.Environment.Extensions;

namespace OrchardHUN.Scripting.CSharp.Services
{
    public interface ICSharpRuntime : IScriptingRuntime
    {
        dynamic ExecuteFile(string path, ScriptScope scope);
    }
}
