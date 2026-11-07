using Bloxstrap.UI.Elements.Base;

namespace Bloxstrap.UI.ViewModels.Settings
{
    /// <summary>PlusStrapper: settings for how the strapper itself looks. Changes preview live, Save keeps them.</summary>
    public class StrapperSettingsViewModel : NotifyPropertyChangedViewModel
    {
        public IEnumerable<Theme> Themes { get; } = Enum.GetValues(typeof(Theme)).Cast<Theme>();

        public Theme Theme
        {
            get => App.Settings.Prop.Theme;
            set { App.Settings.Prop.Theme = value; WpfUiWindow.ApplyToAll(); }
        }

        public IEnumerable<string> AccentColors => WpfUiWindow.AccentColors.Keys;

        public string AccentColor
        {
            get => App.Settings.Prop.AccentColor;
            set { App.Settings.Prop.AccentColor = value; WpfUiWindow.ApplyToAll(); }
        }

        public IEnumerable<string> AppFonts => WpfUiWindow.AppFonts;

        public string AppFont
        {
            get => App.Settings.Prop.AppFont;
            set { App.Settings.Prop.AppFont = value; WpfUiWindow.ApplyToAll(); }
        }

    }
}
