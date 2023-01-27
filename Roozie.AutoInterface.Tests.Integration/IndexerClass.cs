using System.Drawing;

namespace Roozie.AutoInterface.Tests.Integration;

[AutoInterface]
public class IndexerClass : IIndexerClass
{
    private int _value1;
    private string _value2 = "test";

    public int this[int value1]
    {
        get => _value1;
        set => _value1 = value;
    }

    public string this[string value2]
    {
        get => _value2;
        set => _value2 = value;
    }

    public int this[string v1 = "", decimal v2 = 1234.1234m, KnownColor color = KnownColor.Aqua]
    {
        get => _value1;
        set => _value1 = value;
    }
}
