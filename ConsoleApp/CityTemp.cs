using System.Globalization;
using System.Text;

namespace ConsoleApp;

public readonly struct CityTemp
{
    public CityTemp(ReadOnlyMemory<byte> city, ReadOnlyMemory<byte> temp)
    {
        City = Encoding.UTF8.GetString(city.Span);
        Temperature = float.Parse(temp.Span);
    }

    public string City { get; }

    public float Temperature { get; }

    public override string ToString() => $"{City};{Temperature}";
}
