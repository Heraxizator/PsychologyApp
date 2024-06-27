using PsychologyApp.Domain.Common;
using PsychologyApp.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Detector.Domain.Colour.ValueObjects;

public class ColourValue : ValueObject
{
    static ColourValue()
    {
    }

    private ColourValue()
    {
    }

    private ColourValue(string code)
    {
        Code = code;
    }

    public static ColourValue From(string code)
    {
        ColourValue colour = new() { Code = code };

        return !SupportedColours.Contains(colour) ? throw new UnsupportedColourValueException(code) : colour;
    }

    public static ColourValue Black => new(Colors.Black.ToHex());

    public static ColourValue Red => new(Colors.Red.ToHex());

    public static ColourValue Brown => new(Colors.Brown.ToHex());

    public static ColourValue Yellow => new(Colors.Yellow.ToHex());

    public static ColourValue Green => new(Colors.Green.ToHex());

    public static ColourValue Blue => new(Colors.Blue.ToHex());

    public static ColourValue Purple => new(Colors.Purple.ToHex());

    public static ColourValue Gray => new(Colors.Gray.ToHex());

    public string Code { get; private set; } = "#000000";

    public static implicit operator string(ColourValue colour)
    {
        return colour.ToString();
    }

    public static explicit operator ColourValue(string code)
    {
        return From(code);
    }

    public override string ToString()
    {
        return Code;
    }

    protected static IEnumerable<ColourValue> SupportedColours
    {
        get
        {
            yield return Black;
            yield return Red;
            yield return Brown;
            yield return Yellow;
            yield return Green;
            yield return Blue;
            yield return Purple;
            yield return Gray;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}
