namespace Tobey.DataComposer;

public interface IBoolean
{
    public string Name { get; }

    public bool Evaluate(object? value, string[] args);
}