namespace YAML;

public abstract record Data
{
    public record String(string Value) : Data;

    public record Number(int Value) : Data;

    public record Boolean(bool Value) : Data;

    public record Array(List<string> Value) : Data;

    public record Object(Dictionary<string, Data> Value) : Data;
}