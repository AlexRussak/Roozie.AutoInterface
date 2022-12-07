//HintName: ITestClass.g.cs
using System.CodeDom.Compiler;
namespace Roozie.AutoInterface.Tests;
#nullable enable

[GeneratedCode("Roozie.AutoInterface", "1.0.0.0")]
public partial interface ITestClass
{
    System.Guid Id {get;set;}

    string TestMethod(string input);
}
