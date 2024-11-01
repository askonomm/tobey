namespace Tobey.DataComposer.Boolean;

public class EndsWith : IBoolean
{
    public string Name => "ends-with";
    
    public bool Evaluate(object? value, string[] args)
    {
        return value is string strVal && strVal.StartsWith(args[0]);
    }
}