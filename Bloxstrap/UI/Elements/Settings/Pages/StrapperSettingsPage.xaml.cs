using Bloxstrap.UI.ViewModels.Settings;

namespace Bloxstrap.UI.Elements.Settings.Pages
{
    public partial class StrapperSettingsPage
    {
        public StrapperSettingsPage()
        {
            DataContext = new StrapperSettingsViewModel();
            InitializeComponent();
        }
    }
}
