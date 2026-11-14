namespace RecipeBook.Api.Infrastructure;

/// <summary>Turns "Steps[0].Instruction" or "$.steps[0].order" into the JSON-style key "steps[0].instruction".</summary>
public static class ErrorKeys
{
    public static string ToCamelCasePath(string propertyPath)
    {
        if (string.IsNullOrEmpty(propertyPath))
            return string.Empty;

        var path = propertyPath.StartsWith("$.") ? propertyPath[2..] : propertyPath.TrimStart('$');

        var segments = path.Split('.', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Length == 0 ? s : char.ToLowerInvariant(s[0]) + s[1..]);

        return string.Join('.', segments);
    }
}
