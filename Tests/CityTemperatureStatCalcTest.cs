using System.Text;
using ConsoleApp;
using Xunit.Abstractions;

namespace Tests;

public class CityTemperatureStatCalcTest
{
    [Fact]
    public void EnsureCitiesAreUnique()
    {
        var cityTemp1 = new CityTemp("city"u8.ToArray(), "12.3"u8.ToArray());
        var cityTemp2 = new CityTemp("city"u8.ToArray(), "19.3"u8.ToArray());

        var stats = new CityTemperatureStatCalc(2);
        stats.AddCityTemp(cityTemp1);
        stats.AddCityTemp(cityTemp2);

        Assert.Equal(1, stats.NumCities);
    }
}    