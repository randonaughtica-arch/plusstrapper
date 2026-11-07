using Bloxstrap.Models.SettingTasks.Base;

namespace Bloxstrap.Models.SettingTasks
{
    /// <summary>
    /// PlusStrapper: lets the user pick any PNG as their in-game cursor.
    /// A copy is kept in Paths.Base so we can tell our files apart from the cursor presets.
    /// </summary>
    public class CustomCursorTask : StringBaseTask
    {
        public static string StoredPath => Path.Combine(Paths.Base, "Cursors", "CustomCursor.png");

        private static string[] TargetPaths => new[] { "ArrowCursor.png", "ArrowFarCursor.png" }
            .Select(x => Path.Combine(Paths.Modifications, @"content\textures\Cursors\KeyboardMouse", x)).ToArray();

        public CustomCursorTask() : base("ModPreset", "CustomCursor")
        {
            if (File.Exists(StoredPath))
                OriginalState = StoredPath;
        }

        public override void Execute()
        {
            if (!String.IsNullOrEmpty(NewState))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(StoredPath)!);

                if (String.Compare(NewState, StoredPath, StringComparison.InvariantCultureIgnoreCase) != 0)
                    File.Copy(NewState, StoredPath, true);

                foreach (string target in TargetPaths)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                    Filesystem.AssertReadOnly(target);
                    File.Copy(StoredPath, target, true);
                }
            }
            else if (File.Exists(StoredPath))
            {
                byte[] stored = File.ReadAllBytes(StoredPath);

                // only remove files that are still ours (a cursor preset may have replaced them)
                foreach (string target in TargetPaths.Where(x => File.Exists(x) && File.ReadAllBytes(x).SequenceEqual(stored)))
                {
                    Filesystem.AssertReadOnly(target);
                    File.Delete(target);
                }

                File.Delete(StoredPath);
            }

            OriginalState = NewState;
        }
    }
}
