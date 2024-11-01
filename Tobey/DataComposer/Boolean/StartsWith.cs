namespace Tobey.DataComposer.Boolean;

public class StartsWith : IBoolean
{
    public string Name => "starts-with";
    
    public bool Evaluate(object? value, string[] args)
    {
        return value is string strVal && strVal.StartsWith(args[0]);
    }
}