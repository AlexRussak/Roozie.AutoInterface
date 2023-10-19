namespace Roozie.AutoInterface.Tests.Integration;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
#pragma warning disable CA1024
#pragma warning disable CA1044
#pragma warning disable CA1822
#pragma warning disable CA1852

[AutoInterface]
public class RoozieTestClassGenerics<TClass, TStruct> : IRoozieTestClassGenerics<TClass, TStruct> where TClass : class
{
    /// <summary>
    /// This is a test comment for <see cref="System.Drawing.Color"/>
    /// </summary>
    /// <param name="input"></param>
    /// <typeparam name="T">doc</typeparam>
    /// <returns></returns>
    public T TestMethod<T>(T input) => input;

    public TOutput? TestMethod2<TInput1, TInput2, TOutput>(TInput1 input1, TInput2 input2) => default;

    public T TestMethod3<T>(T input) where T : class => input;

    public TClass? TestMethod4(TStruct input1) => default;
}
