namespace Overclocked.SharedKernel.Extensions;

public static class DictionaryExtensions
{
    public static void Merge(this Dictionary<string, string[]> destination, Dictionary<string, string[]> source)
    {
        if (source is null || source.Count == 0)
            return;

        foreach (KeyValuePair<string, string[]> kvp in source)
        {
            destination[kvp.Key] = destination.TryGetValue(kvp.Key, out var existingErrors)
                ? existingErrors.Concat(kvp.Value).ToArray()
                : kvp.Value;
        }
    }
}
