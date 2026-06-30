namespace PsychologyApp.Application.Models.Tests;

public sealed class ParseResult<T>
{
    public T? Value { get; init; }
    public string? Error { get; init; }

    public bool IsSuccess => Error is null && Value is not null;

    public static ParseResult<T> Success(T value) => new() { Value = value };

    public static ParseResult<T> Failure(string error) => new() { Error = error };
}
