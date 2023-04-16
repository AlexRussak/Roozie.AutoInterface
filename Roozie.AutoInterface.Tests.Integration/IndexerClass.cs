namespace Roozie.AutoInterface.Tests.Integration;

[AutoInterface]
public class IndexerClass : IIndexerClass
{
    private int value1;
    private string value2 = "test";

    public int this[int val1] { get => value1; set => value1 = value; }

    public string this[string val2] { get => value2; set => value2 = value; }

    public int this[string v1 = "", decimal v2 = 1234.1234m, KnownColor color = KnownColor.Aqua]
    {
        get => value1;
        set => value1 = value;
    }
}
