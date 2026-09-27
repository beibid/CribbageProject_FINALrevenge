using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Session;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class GuestAccessPage : Page
    {
        public GuestAccessPage()
        {
            InitializeComponent();
            displayNameTextBox.Focus();
        }

        private string GetDisplayName()
        {
            return displayNameTextBox.Text.Trim();
        }

        private bool IsDisplayNameEmpty()
        {
            return string.IsNullOrWhiteSpace(GetDisplayName());
        }

        private string BuildGuestName()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Lang.GuestAccess_DisplayNameFormat,
                GetDisplayName());
        }

        private void StartGuestSession()
        {
            CurrentSession.StartGuestSession(BuildGuestName());
            NavigationService.Navigate(new Lobby());
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsDisplayNameEmpty())
            {
                MessageDialog.ShowWarning(
                    Window.GetWindow(this),
                    Lang.GuestAccess_NameRequiredMessage);
            }
            else
            {
                StartGuestSession();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartMenu());
        }
    }
}