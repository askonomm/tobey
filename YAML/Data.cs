namespace YAML;

public abstract record Data
{
    public record String(string Key, string Value) : Data;

    public record Number(string Key, int Value) : Data;

    public record Boolean(string Key, bool Value) : Data;

    public record Array(string Key, List<string> Value) : Data;

    public record Object(string Key, Dictionary<string, Data> Value) : Data;
}