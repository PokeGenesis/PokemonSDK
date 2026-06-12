namespace SDK.Tools.Fakemons;

using System.Text.Json;

public static class FakemonFilter
{
    public static IReadOnlyList<FakemonPartLayer> Apply(
        IReadOnlyList<FakemonPartLayer> catalog,
        FakemonPartsCatalog catalogInstance,
        string? filterExpression)
    {
        if (filterExpression is null)
            return catalog;

        var criteria = ParseFilter(filterExpression);
        if (criteria.Count == 0)
            return catalog;

        var result = new List<FakemonPartLayer>();
        foreach (var layer in catalog)
        {
            var meta = catalogInstance.GetMetadata(layer.Path);
            if (meta is null)
            {
                result.Add(layer);
                continue;
            }

            bool match = true;
            foreach (var (key, value) in criteria)
            {
                if (!meta.RootElement.TryGetProperty(key, out var propValue))
                    continue; // key absent in sidecar → compatible

                var propStr = propValue.ValueKind == JsonValueKind.Number
                    ? propValue.GetInt32().ToString()
                    : propValue.GetString() ?? "";

                if (!string.Equals(propStr, value, StringComparison.OrdinalIgnoreCase))
                {
                    match = false;
                    break;
                }
            }

            if (match)
                result.Add(layer);
        }

        return result;
    }

    private static Dictionary<string, string> ParseFilter(string expression)
    {
        var dict = new Dictionary<string, string>();
        foreach (var part in expression.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var idx = part.IndexOf(':');
            if (idx < 0) continue;
            dict[part[..idx].Trim()] = part[(idx + 1)..].Trim();
        }
        return dict;
    }
}
