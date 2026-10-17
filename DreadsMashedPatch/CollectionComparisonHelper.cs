namespace DreadsMashedPatch;

internal static class CollectionComparisonHelper
{
    // Match each complete value once so duplicate counts remain significant.
    public static bool UnorderedEquals<T>(IReadOnlyCollection<T> left, IReadOnlyCollection<T> right,
        Func<T, T, bool> equals)
    {
        if (left.Count != right.Count) return false;
        var unmatched = right.ToList();
        foreach (var item in left)
        {
            var matchIndex = unmatched.FindIndex(candidate => equals(item, candidate));
            if (matchIndex < 0) return false;
            unmatched.RemoveAt(matchIndex);
        }

        return true;
    }
}
