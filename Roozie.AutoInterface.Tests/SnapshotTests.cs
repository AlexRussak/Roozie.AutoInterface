namespace Roozie.AutoInterface.Tests;

[UsesVerify]
public class SnapshotTests
{
    [Fact]
    public Task SimpleClass()
    {
        const string source = @"
using System;
using Roozie.AutoInterface;
using System.Text;

namespace Roozie.AutoInterface.Tests;

[AutoInterface]
public class TestClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TestMethod(string input) => input;
}
}";
        return TestHelper.Verify(source);
    }

    [Fact]
    public Task Class_WithoutAttribute()
    {
        const string source = @"
using System;
using Roozie.AutoInterface;
using System.Text;

namespace Roozie.AutoInterface.Tests;

public class TestClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TestMethod(string input) => input;
}
}";
        return TestHelper.Verify(source);
    }

    [Fact]
    public Task OtherAttributes()
    {
        const string source = @"
using System;
using Roozie.AutoInterface;
using System.Text;

namespace Roozie.AutoInterface.Tests;

[AutoInterface]
[Serializable]
public class TestClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TestMethod(string input) => input;
}
}";
        return TestHelper.Verify(source);
    }

    [Fact]
    public Task NotAValidProperty()
    {
        const string source = @"
using System;
using Roozie.AutoInterface;
using System.Text;

namespace Roozie.AutoInterface.Tests;

[AutoInterface]
public class TestClass
{
    public string? Test { private get; private set; }
}
}";
        return TestHelper.Verify(source);
    }

    [Fact]
    public Task InitProperty()
    {
        const string source = @"
using System;
using Roozie.AutoInterface;
using System.Text;

namespace Roozie.AutoInterface.Tests;

[AutoInterface]
public class TestClass
{
    public string? Test { get; init; }
}
}";
        return TestHelper.Verify(source);
    }

    [Fact]
    public Task PrivateSetProperty()
    {
        const string source = @"
using System;
using Roozie.AutoInterface;
using System.Text;

namespace Roozie.AutoInterface.Tests;

[AutoInterface]
public class TestClass
{
    public string? Test { get; private set; }
}
}";
        return TestHelper.Verify(source);
    }
}
