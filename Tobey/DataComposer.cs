namespace Tobey;

public class DataComposer(List<Dictionary<string, object?>> content)
{
    private readonly List<string> _specialKeys = ["sort_by", "sort_order", "limit", "offset"];

    public List<Dictionary<string, object?>> Compose(Dictionary<string, object?> val)
    {
        var filterVal = val.Where(x => !_specialKeys.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
        var newContent = content.Where(c => MeetsConditions(c, filterVal)).ToList();

        // sort the content
        if (val.TryGetValue("sort_by", out var sortBy))
        {
            if (sortBy is string sortByStr)
            {
                newContent = [.. newContent.OrderBy(x => x.GetValueOrDefault(sortByStr))];
            }
        }
        
        // order the content
        if (val.TryGetValue("sort_order", out var order))
        {
            if (order is string orderStr)
            {
                if (orderStr == "desc")
                {
                    newContent.Reverse();
                }
            }
        }

        // offset the content
        if (val.TryGetValue("offset", out object? offset))
        {
            if (offset is int offsetInt)
            {
                newContent = newContent[offsetInt..];
            }
        }

        // limit the content
        if (val.TryGetValue("limit", out object? limit))
        {
            if (limit is int limitInt)
            {
                newContent = newContent[..limitInt];
            }
        }

        return newContent;
    }

    private static bool MeetsConditions(Dictionary<string, object?> item, Dictionary<string, object?> val)
    {
        var conditionsCount = val.Count;
        var conditionsMet = 0;

        foreach (var (k, v) in val)
        {
            if (item.TryGetValue(k, out var value))
            {
                // is the value a string and the condition a string?
                if (value is string && v is string)
                {
                    if (value.Equals(v))
                    {
                        conditionsMet++;
                        continue;
                    }
                }

                // is the value a int and the condition a int?
                if (value is int && v is int)
                {
                    if (value.Equals(v))
                    {
                        conditionsMet++;
                        continue;
                    }
                }

                // is the value a double and the condition a double?
                if (value is double && v is double)
                {
                    if (value.Equals(v))
                    {
                        conditionsMet++;
                        continue;
                    }
                }

                // is the value a bool and the condition a bool?
                if (value is bool && v is bool)
                {
                    if (value.Equals(v))
                    {
                        conditionsMet++;
                    }
                }
            }
        }

        return conditionsMet == conditionsCount;
    }
}
