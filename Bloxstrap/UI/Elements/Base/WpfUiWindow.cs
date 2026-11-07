using System.Windows;
using System.Windows.Interop;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace Bloxstrap.UI.Elements.Base
{
    public abstract class WpfUiWindow : UiWindow
    {
        private readonly IThemeService _themeService = new ThemeService();

        public WpfUiWindow()
        {
            ApplyTheme();
        }

        // null = use the Windows accent colour
        public static readonly Dictionary<string, string?> AccentColors = new()
        {
            { "PlusStrapper Teal", "#0F9A8C" },
            { "Green", "#22B550" },
            { "Blue", "#2F80ED" },
            { "Purple", "#8E5CF7" },
            { "Pink", "#E8569A" },
            { "Orange", "#F2994A" },
            { "Red", "#E5484D" },
            { "Windows accent", null },
        };

        public static readonly string[] AppFonts = { "Nimbus Sans", "Segoe UI" };

        /// <summary>Re-applies theme, accent and font to every open strapper window (used by Strapper Settings for a live preview).</summary>
        public static void ApplyToAll()
        {
            foreach (Window window in Application.Current.Windows)
                if (window is WpfUiWindow uiWindow)
                    uiWindow.ApplyTheme();
        }

        public void ApplyTheme()
        {
            const int customThemeIndex = 2; // index for CustomTheme merged dictionary

            _themeService.SetTheme(App.Settings.Prop.Theme.GetFinal() == Enums.Theme.Dark ? ThemeType.Dark : ThemeType.Light);
            _themeService.SetSystemAccent();

            if (AccentColors.GetValueOrDefault(App.Settings.Prop.AccentColor) is string hex)
                Accent.Apply((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex),
                    App.Settings.Prop.Theme.GetFinal() == Enums.Theme.Dark ? ThemeType.Dark : ThemeType.Light);

            FontFamily = App.Settings.Prop.AppFont == "Segoe UI"
                ? new System.Windows.Media.FontFamily("Segoe UI")
                : (System.Windows.Media.FontFamily)Application.Current.Resources["NimbusSans"];

            // there doesn't seem to be a way to query the name for merged dictionaries
            var dict = new ResourceDictionary { Source = new Uri($"pack://application:,,,/UI/Style/{Enum.GetName(App.Settings.Prop.Theme.GetFinal())}.xaml") };
            Application.Current.Resources.MergedDictionaries[customThemeIndex] = dict;

#if QA_BUILD
            this.BorderBrush = System.Windows.Media.Brushes.Red;
            this.BorderThickness = new Thickness(4);
#endif
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            if (App.Settings.Prop.WPFSoftwareRender || App.LaunchSettings.NoGPUFlag.Active)
            {
                if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
                    hwndSource.CompositionTarget.RenderMode = RenderMode.SoftwareOnly;
            }

            base.OnSourceInitialized(e);
        }
    }
}
