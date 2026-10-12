using Savepoint;
var root = Path.Combine(Path.GetTempPath(), "opengamesave-" + Guid.NewGuid().ToString("N")); var save = Path.Combine(root, "save"); var library = Path.Combine(root, "library"); Directory.CreateDirectory(save); File.WriteAllText(Path.Combine(save, "slot.dat"), "progress"); Directory.CreateDirectory(Path.Combine(save, "nested")); File.WriteAllText(Path.Combine(save, "nested", "settings.ini"), "quality=high");
try { SaveEngine.CreateSnapshot(new GameProfile("TestGame", null, save), Path.Combine(save, "backups")); throw new Exception("Nested snapshot library was accepted."); } catch (InvalidOperationException) { }
if (Directory.Exists(Path.Combine(save, "backups"))) throw new Exception("Rejected nested snapshot library created files under the save folder.");
try { SaveEngine.CreateSnapshot(new GameProfile("..", null, save), library); throw new Exception("Path traversal profile name was accepted."); } catch (ArgumentException) { }
var snapshot = SaveEngine.CreateSnapshot(new GameProfile("TestGame", null, save), library); var snapRoot = Path.Combine(library, "TestGame", snapshot.Id); if (!SaveEngine.Verify(snapRoot)) throw new Exception("Fresh snapshot did not verify.");
var blocked = Path.Combine(root, "blocked"); Directory.CreateDirectory(Path.Combine(blocked, "nested")); File.WriteAllText(Path.Combine(blocked, "nested", "settings.ini"), "existing"); try { SaveEngine.Restore(snapRoot, blocked); throw new Exception("Restore collision was accepted."); } catch (IOException) { }
if (File.Exists(Path.Combine(blocked, "slot.dat")) || File.ReadAllText(Path.Combine(blocked, "nested", "settings.ini")) != "existing") throw new Exception("Collision preflight left a partial restore.");
var restore = Path.Combine(root, "restore"); if (SaveEngine.Restore(snapRoot, restore) != 2 || File.ReadAllText(Path.Combine(restore, "slot.dat")) != "progress") throw new Exception("Restore failed.");
var firstFile = snapshot.Files.Single(x => x.RelativePath == "slot.dat"); var copiedFile = Path.Combine(snapRoot, firstFile.RelativePath); var copiedBytes = File.ReadAllBytes(copiedFile); if (firstFile.Bytes != copiedBytes.LongLength || firstFile.Sha256 != Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(copiedBytes))) throw new Exception("Manifest size/hash do not describe copied bytes.");
var directoryBlock = Path.Combine(root, "directory-block"); Directory.CreateDirectory(Path.Combine(directoryBlock, "slot.dat"));
try { SaveEngine.Restore(snapRoot, directoryBlock); throw new Exception("Directory collision accepted."); } catch (IOException) { }
if (Directory.Exists(Path.Combine(directoryBlock, "nested"))) throw new Exception("Directory collision allowed a partial restore.");
var parentBlock = Path.Combine(root, "parent-block"); Directory.CreateDirectory(parentBlock); File.WriteAllText(Path.Combine(parentBlock, "nested"), "keep");
try { SaveEngine.Restore(snapRoot, parentBlock); throw new Exception("Parent-file collision accepted."); } catch (IOException) { }
if (File.Exists(Path.Combine(parentBlock, "slot.dat"))) throw new Exception("Parent collision allowed a partial restore.");
File.WriteAllText(Path.Combine(save, "manifest.json"), "source metadata");
try { SaveEngine.CreateSnapshot(new GameProfile("Reserved", null, save), library); throw new Exception("Reserved manifest filename accepted."); } catch (InvalidDataException) { }
if (Directory.GetFileSystemEntries(Path.Combine(library, "Reserved")).Length != 0) throw new Exception("Reserved filename left an incomplete snapshot.");
File.AppendAllText(Path.Combine(snapRoot, "slot.dat"), "tampered"); if (SaveEngine.Verify(snapRoot)) throw new Exception("Tampered snapshot verified."); Directory.Delete(root, true); Console.WriteLine("PASS: atomic snapshot, complete restore preflight, verified restore, and tamper detection");
