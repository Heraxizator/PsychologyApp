using System.Text.Json;
using System.Text.Json.Serialization;

namespace PsychologyApp.Application.Body;

/// <summary>
/// Stores an enum by name and reads a name it does not know (written by a newer version of the app, or edited by hand) as an undefined
/// value instead of failing the whole document. Readers skip what is undefined; the rest of the log and of a backup stays readable.
/// </summary>
public sealed class LenientEnumConverter<T> : JsonConverter<T>
    where T : struct, Enum
{
    /// <summary>What an unknown name reads as; never a defined value.</summary>
    public static T Unknown { get; } = (T)Enum.ToObject(typeof(T), -1);

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.String
        && Enum.TryParse(reader.GetString(), ignoreCase: false, out T value)
        && Enum.IsDefined(value)
            ? value
            : Skip(ref reader);

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());

    private static T Skip(ref Utf8JsonReader reader)
    {
        reader.Skip();
        return Unknown;
    }
}
