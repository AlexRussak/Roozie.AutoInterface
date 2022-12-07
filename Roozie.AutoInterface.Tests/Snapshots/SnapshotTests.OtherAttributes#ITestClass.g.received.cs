//HintName: ITestClass.g.cs
using Roozie.AutoInterface;
using System;
using System.CodeDom.Compiler;
using System.Text;
namespace Roozie.AutoInterface.Tests;
#nullable enable

[GeneratedCode("Roozie.AutoInterface", "1.0.0.0")]
public partial interface ITestClass
{
    System.Guid Id {get;set;}

    string TestMethod(string input);
}
