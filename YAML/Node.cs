namespace YAML;

public record Node
{
    public required string Key { get; set; }
    public required object Value { get; set; }
}
