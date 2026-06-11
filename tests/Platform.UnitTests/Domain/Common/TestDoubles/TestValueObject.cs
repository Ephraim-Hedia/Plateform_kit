using Platform.Domain.Common;

namespace Platform.UnitTests.Domain.Common.TestDoubles;

internal sealed class TestValueObject : ValueObject
{
    public TestValueObject(string line1, string line2)
    {
        Line1 = line1;
        Line2 = line2;
    }

    public string Line1 { get; }

    public string Line2 { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Line1;
        yield return Line2;
    }
}

internal sealed class OtherTestValueObject : ValueObject
{
    public OtherTestValueObject(string line1, string line2)
    {
        Line1 = line1;
        Line2 = line2;
    }

    public string Line1 { get; }

    public string Line2 { get; }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Line1;
        yield return Line2;
    }
}
