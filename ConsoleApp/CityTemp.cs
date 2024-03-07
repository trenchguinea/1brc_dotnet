using System.Globalization;
using System.Text;

namespace ConsoleApp;

public readonly struct CityTemp(ReadOnlyMemory<byte> city, ReadOnlyMemory<byte> temp)
{
    public string City { get; } = Encoding.UTF8.GetString(city.Span);

    public float Temperature { get; } = float.Parse(temp.Span);

    public override string ToString() => $"{City};{Temperature}";
}
