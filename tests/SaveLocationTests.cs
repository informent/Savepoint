using Savepoint;
var root = Path.Combine(Path.GetTempPath(), "savepoint-locator-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);
try
{
    var steam = Path.Combine(root, "Steam");
    var secondary = Path.Combine(root, "SecondaryLibrary");
    var userdata = Path.Combine(steam, "userdata");
    var first = Path.Combine(userdata, "12345", "456", "remote");
    var second = Path.Combine(userdata, "67890", "456");
    Directory.CreateDirectory(first); Directory.CreateDirectory(second);
    Directory.CreateDirectory(Path.Combine(userdata, "12345", "999", "remote"));
    var game = new DetectedGame("456", "Demo Game", Path.Combine(secondary, "steamapps", "common", "Demo Game"), secondary);
    var found = SaveLocationLocator.FindCandidates(game, new[] { userdata });
    if (!found.Contains(first) || !found.Contains(second)) throw new Exception("Cross-library, multi-user discovery failed.");
    if (found.Contains(userdata) || found.Contains(Path.GetDirectoryName(first)!) || found.Any(x => x.Contains("999"))) throw new Exception("Discovery suggested a broader or unrelated save folder.");
    if (!SaveLocationLocator.FindCandidates(game, new[] { Path.Combine(userdata, "12345") }).Contains(first)) throw new Exception("Direct user-folder discovery failed.");
    var primaryGame = game with { LibraryPath = steam };
    if (!SaveLocationLocator.FindCandidates(primaryGame, Array.Empty<string>()).Contains(first)) throw new Exception("Default Steam userdata location failed.");
    var duplicate = SaveLocationLocator.FindCandidates(primaryGame, new[] { userdata, userdata });
    if (duplicate.Count != duplicate.Distinct(StringComparer.OrdinalIgnoreCase).Count()) throw new Exception("Duplicate candidates returned.");
    foreach (var invalid in new[] { "../456", "C:\\escape", "", "456/remote" })
        try { SaveLocationLocator.FindCandidates(game with { AppId = invalid }, new[] { userdata }); throw new Exception("Unsafe application ID accepted."); }
        catch (ArgumentException) { }
    var outside = Path.Combine(root, "outside"); Directory.CreateDirectory(Path.Combine(outside, "456", "remote"));
    var linkedUser = Path.Combine(userdata, "77777");
    var start = new System.Diagnostics.ProcessStartInfo("cmd.exe") { UseShellExecute = false, CreateNoWindow = true };
    foreach (var argument in new[] { "/c", "mklink", "/J", linkedUser, outside }) start.ArgumentList.Add(argument);
    using var process = System.Diagnostics.Process.Start(start)!; process.WaitForExit();
    if (process.ExitCode != 0) throw new Exception("Junction fixture failed.");
    try { if (SaveLocationLocator.FindCandidates(game, new[] { userdata }).Any(x => x.StartsWith(linkedUser))) throw new Exception("Linked saves suggested."); }
    finally { Directory.Delete(linkedUser); }
    Console.WriteLine("PASS: real Steam userdata layout, secondary libraries, multiple accounts, remote preference, exact-game scope, deduplication and linked-save exclusion");
}
finally { Directory.Delete(root, true); }
