using System.IO;
namespace Savepoint;
public static class SaveLocationLocator
{
    public static IReadOnlyList<string> FindCandidates(DetectedGame game, IEnumerable<string> roots)
    {
        var candidates = new HashSet<string>(StringComparer.OrdinalIgnoreCase); var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); var roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (!System.Text.RegularExpressions.Regex.IsMatch(game.AppId, "^[0-9]{1,10}$")) throw new ArgumentException("Invalid Steam application ID.");
        if (SafeSegment(game.Name))
            foreach (var path in new[] { Path.Combine(documents, "My Games", game.Name), Path.Combine(documents, game.Name), Path.Combine(roaming, game.Name), Path.Combine(local, game.Name) }) AddCandidate(candidates, path);
        foreach (var root in roots.Append(Path.Combine(game.LibraryPath, "userdata")).Distinct(StringComparer.OrdinalIgnoreCase).Where(Directory.Exists))
        {
            var direct = Path.Combine(root, game.AppId);
            AddGameFolder(candidates, direct);
            try
            {
                foreach (var user in Directory.EnumerateDirectories(root))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileName(user), "^[0-9]+$")) continue;
                    AddGameFolder(candidates, Path.Combine(user, game.AppId));
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
        return candidates.OrderBy(x => x).ToArray();
    }
    private static bool SafeSegment(string name) => !string.IsNullOrWhiteSpace(name) && name is not "." and not ".." && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 && !name.EndsWith('.') && !name.EndsWith(' ');
    private static void AddGameFolder(ISet<string> candidates, string folder)
    {
        var remote = Path.Combine(folder, "remote");
        AddCandidate(candidates, Directory.Exists(remote) ? remote : folder);
    }
    private static void AddCandidate(ISet<string> candidates, string folder)
    {
        try
        {
            if (!Directory.Exists(folder)) return;
            for (string? current = Path.GetFullPath(folder); !string.IsNullOrEmpty(current); current = Path.GetDirectoryName(current))
                if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0) return;
            candidates.Add(Path.GetFullPath(folder));
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}
