using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;

using Windows.Win32;
using Windows.Win32.UI.Shell;
using Windows.Win32.Foundation;

using CommunityToolkit.Mvvm.Input;

using Bloxstrap.Models.SettingTasks;
using Bloxstrap.AppData;

namespace Bloxstrap.UI.ViewModels.Settings
{
    public class ModsViewModel : NotifyPropertyChangedViewModel
    {
        private void OpenModsFolder() => Process.Start("explorer.exe", Paths.Modifications);

        private readonly Dictionary<string, byte[]> FontHeaders = new()
        {
            { "ttf", new byte[4] { 0x00, 0x01, 0x00, 0x00 } },
            { "otf", new byte[4] { 0x4F, 0x54, 0x54, 0x4F } },
            { "ttc", new byte[4] { 0x74, 0x74, 0x63, 0x66 } } 
        };

        // ---- PlusStrapper: one dropdown per thing, no file digging ----

        private const string OwnFontLabel = "My own font file...";
        private const string OwnCursorLabel = "My own image...";

        // label -> embedded resource ("" = Roblox default)
        private static readonly Dictionary<string, string> FontPresets = new()
        {
            { "Roblox default", "" },
            { "Nimbus Sans", "PlusStrapper.Fonts.NimbusSanL-Regu.ttf" },
            { "Nimbus Sans Bold", "PlusStrapper.Fonts.NimbusSanL-Bold.ttf" },
        };

        public IEnumerable<string> FontChoices => FontPresets.Keys.Append(OwnFontLabel);

        private string? _fontChoice;

        public string FontChoice
        {
            get => _reverting ? null! : _fontChoice ??= DetectFontChoice();
            set
            {
                if (_reverting || value is null || value == _fontChoice)
                    return;

                if (FontPresets.TryGetValue(value, out string? resource))
                    TextFontTask.NewState = resource == "" ? "" : ExtractFont(resource);
                else if (!PickFile($"{Strings.Menu_FontFiles}|*.ttf;*.otf;*.ttc", IsValidFont, Strings.Menu_Mods_Misc_CustomFont_Invalid, out string path))
                {
                    RevertLater(nameof(FontChoice));
                    return;
                }
                else
                    TextFontTask.NewState = path;

                _fontChoice = value;
                OnPropertyChanged(nameof(FontChoice));
            }
        }

        private string DetectFontChoice()
        {
            string current = TextFontTask.NewState;

            if (String.IsNullOrEmpty(current) || !File.Exists(current))
                return FontPresets.Keys.First();

            string hash = HashFile(File.OpenRead(current));

            foreach (var pair in FontPresets.Where(x => x.Value != ""))
                if (HashFile(Resource.GetStream(pair.Value)) == hash)
                    return pair.Key;

            return OwnFontLabel;
        }

        private static string ExtractFont(string resource)
        {
            string path = Path.Combine(Paths.Base, "Fonts", resource.Replace("PlusStrapper.Fonts.", ""));

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using var input = Resource.GetStream(resource);
                using var output = File.Create(path);
                input.CopyTo(output);
            }

            return path;
        }

        public IEnumerable<string> CursorChoices => CursorTypeTask.Selections.Select(CursorLabel).Append(OwnCursorLabel);

        private static string CursorLabel(Enums.CursorType type) => (string)new UI.Converters.EnumNameConverter().Convert(type, typeof(string), null!, CultureInfo.InvariantCulture);

        public string CursorChoice
        {
            get => _reverting ? null! : !String.IsNullOrEmpty(CustomCursorTask.NewState) ? OwnCursorLabel : CursorLabel(CursorTypeTask.NewState);
            set
            {
                if (_reverting || value is null || value == CursorChoice)
                    return;

                if (value == OwnCursorLabel)
                {
                    if (!PickFile("PNG image|*.png", x => x.Take(4).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47 }), "That file isn't a PNG image.", out string path))
                    {
                        RevertLater(nameof(CursorChoice));
                        return;
                    }

                    CursorTypeTask.NewState = Enums.CursorType.Default;
                    CustomCursorTask.NewState = path;
                }
                else
                {
                    CursorTypeTask.NewState = CursorTypeTask.Selections.First(x => CursorLabel(x) == value);
                    CustomCursorTask.NewState = "";
                }

                OnPropertyChanged(nameof(CursorChoice));
            }
        }

        private bool IsValidFont(byte[] data) => FontHeaders.Values.Any(x => data.Take(4).SequenceEqual(x));

        private static bool PickFile(string filter, Func<byte[], bool> validate, string invalidMessage, out string path)
        {
            path = "";
            var dialog = new OpenFileDialog { Filter = filter };

            if (dialog.ShowDialog() != true)
                return false;

            if (!validate(File.ReadAllBytes(dialog.FileName)))
            {
                Frontend.ShowMessageBox(invalidMessage, MessageBoxImage.Error);
                return false;
            }

            path = dialog.FileName;
            return true;
        }

        private static string HashFile(Stream stream)
        {
            using (stream)
                return MD5Hash.Stringify(App.MD5Provider.ComputeHash(stream));
        }

        private bool _reverting;

        // a ComboBox won't re-select the old item after a cancelled pick unless it is cleared first
        private void RevertLater(string property) =>
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                _reverting = true;
                OnPropertyChanged(property);
                _reverting = false;
                OnPropertyChanged(property);
            }, System.Windows.Threading.DispatcherPriority.ContextIdle);

        public IEnumerable<string> PerformanceLevels => FastFlagManager.PerformanceLevels;

        public string PerformanceLevel
        {
            get => App.Settings.Prop.PerformanceLevel;
            set
            {
                App.FastFlags.ApplyPerformanceLevel(App.Settings.Prop.PerformanceLevel, value);
                App.Settings.Prop.PerformanceLevel = value;
                OnPropertyChanged(nameof(PerformanceDescription));
            }
        }

        public string PerformanceDescription => FastFlagManager.PerformanceLevelInfo.GetValueOrDefault(PerformanceLevel, "");

        public ICommand OpenModsFolderCommand => new RelayCommand(OpenModsFolder);

        public ICommand OpenCompatSettingsCommand => new RelayCommand(OpenCompatSettings);

        public ModPresetTask OldAvatarBackgroundTask { get; } = new("OldAvatarBackground", @"ExtraContent\places\Mobile.rbxl", "OldAvatarBackground.rbxl");

        public ModPresetTask OldCharacterSoundsTask { get; } = new("OldCharacterSounds", new()
        {
            { @"content\sounds\action_footsteps_plastic.mp3", "Sounds.OldWalk.mp3"  },
            { @"content\sounds\action_jump.mp3",              "Sounds.OldJump.mp3"  },
            { @"content\sounds\action_get_up.mp3",            "Sounds.OldGetUp.mp3" },
            { @"content\sounds\action_falling.mp3",           "Sounds.Empty.mp3"    },
            { @"content\sounds\action_jump_land.mp3",         "Sounds.Empty.mp3"    },
            { @"content\sounds\action_swim.mp3",              "Sounds.Empty.mp3"    },
            { @"content\sounds\impact_water.mp3",             "Sounds.Empty.mp3"    }
        });

        public EmojiModPresetTask EmojiFontTask { get; } = new();

        public EnumModPresetTask<Enums.CursorType> CursorTypeTask { get; } = new("CursorType", new()
        {
            {
                Enums.CursorType.From2006, new()
                {
                    { @"content\textures\Cursors\KeyboardMouse\ArrowCursor.png",    "Cursor.From2006.ArrowCursor.png"    },
                    { @"content\textures\Cursors\KeyboardMouse\ArrowFarCursor.png", "Cursor.From2006.ArrowFarCursor.png" }
                }
            },
            {
                Enums.CursorType.From2013, new()
                {
                    { @"content\textures\Cursors\KeyboardMouse\ArrowCursor.png",    "Cursor.From2013.ArrowCursor.png"    },
                    { @"content\textures\Cursors\KeyboardMouse\ArrowFarCursor.png", "Cursor.From2013.ArrowFarCursor.png" }
                }
            }
        });

        public FontModPresetTask TextFontTask { get; } = new();

        public CustomCursorTask CustomCursorTask { get; } = new();

        private void OpenCompatSettings()
        {
            string path = new RobloxPlayerData().ExecutablePath;

            if (File.Exists(path))
                PInvoke.SHObjectProperties(HWND.Null, SHOP_TYPE.SHOP_FILEPATH, path, "Compatibility");
            else
                Frontend.ShowMessageBox(Strings.Common_RobloxNotInstalled, MessageBoxImage.Error);

        }
    }
}
