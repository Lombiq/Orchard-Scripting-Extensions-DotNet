using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Orchard.Environment.Extensions;
using OrchardHUN.Scripting.Models;
using System.Reflection;

namespace OrchardHUN.Scripting.CSharp.Services
{
    [OrchardFeature("OrchardHUN.Scripting.CSharp")]
    public class CSharpRuntime : ICSharpRuntime
    {
        public string Engine
        {
            get { return "C#"; }
        }

        public dynamic ExecuteExpression(string expression, Models.ScriptScope scope)
        {
            throw new NotImplementedException();
        }

        public dynamic ExecuteFile(string path, ScriptScope scope)
        {
            throw new NotImplementedException();
        }
    }
}