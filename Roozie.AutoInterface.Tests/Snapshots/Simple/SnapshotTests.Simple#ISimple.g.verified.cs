﻿//HintName: ISimple.g.cs
// <auto-generated>
// This file was generated by Roozie.AutoInterface v0.5.0
// </auto-generated>

using System;
using System.Text;

namespace Roozie.AutoInterface.Tests;

#nullable enable

public partial interface ISimple
{
    System.Guid Property {get;set;}

    int PropertyInit {get;init;}

    string? PropertyPrivateSet {get;}

    string TestMethod(string input = "test");
}
