using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class GuestAccessPage : Page
    {
        public GuestAccessPage()
        {
            InitializeComponent();
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            string displayName = displayNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(displayName))
            {
                MessageDialog.ShowWarning(Window.GetWindow(this), Lang.GuestAccess_NameRequiredMessage);
                return;
            }

            string guestName = string.Format(CultureInfo.CurrentCulture, Lang.GuestAccess_DisplayNameFormat, displayName);
            MessageDialog.ShowSuccess(Window.GetWindow(this), guestName);
            NavigationService.Navigate(new Lobby());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartMenu());
        }
    }
}