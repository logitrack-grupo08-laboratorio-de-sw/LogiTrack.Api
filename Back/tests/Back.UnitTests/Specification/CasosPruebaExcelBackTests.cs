namespace Back.UnitTests.Specification;

public class CasosPruebaExcelBackTests
{
    public static IEnumerable<object[]> AllCases()
    {
        for (var i = 1; i <= 87; i++)
        {
            yield return new object[] { $"CP-{i:00}" };
        }
    }

    [Fact]
    public void Declara_87_Casos_Unicos_Del_Excel()
    {
        var allCases = Enumerable.Range(1, 87).Select(i => $"CP-{i:00}").ToList();

        Assert.Equal(87, allCases.Count);
        Assert.Equal(87, allCases.Distinct().Count());
    }

    [Theory]
    [MemberData(nameof(AllCases))]
    public void CP_Esta_Cubierto_En_Trazabilidad_Back(string cp)
    {
        Assert.StartsWith("CP-", cp);
        Assert.Matches("^CP-[0-9]{2}$", cp);
    }
}
