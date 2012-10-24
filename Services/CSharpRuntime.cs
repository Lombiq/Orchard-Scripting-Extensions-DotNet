using Microsoft.CSharp;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using OrchardHUN.Scripting.Services;
using System;
using System.CodeDom.Compiler;

namespace OrchardHUN.Scripting.CSharp.Services
{
    [OrchardFeature("OrchardHUN.Scripting.CSharp")]
    public class CSharpRuntime : ICSharpRuntime
    {
        private readonly IOrchardServices _orchardServices;

        public CSharpRuntime(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
        }

        public string Engine
        {
            get { return "C#"; }
        }

        public dynamic ExecuteExpression(string expression, ScriptScope scope)
        {
            string code = @"
                using System;
                public class Scripting
                {
                    public static void Script()
                    {" +
                       expression + @"
                    }
                }";

            CompilerParameters compParameters = new CompilerParameters() { GenerateInMemory = true };
            CompilerResults result = new CSharpCodeProvider().CompileAssemblyFromSource(compParameters, code);

            if (result.Errors.HasErrors)
            {
                _orchardServices.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, new LocalizedString("There is a problem with your script."));
                return false;
            }
            else
            {
                object myClass = result.CompiledAssembly.CreateInstance("Scripting");
                myClass.GetType().GetMethod("Script").Invoke(myClass, new object[] { });
                _orchardServices.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, new LocalizedString("Your script ran successfully."));
                return true;
            }
        }

        public dynamic ExecuteFile(string path, ScriptScope scope)
        {
            throw new NotImplementedException();
        }
    }
}