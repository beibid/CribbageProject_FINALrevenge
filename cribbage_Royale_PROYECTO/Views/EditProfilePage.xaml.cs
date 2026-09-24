using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class EditProfilePage : Page
    {
        private string originalUsername = "XxDavidxx";
        private string originalEmail = "david@correo.com";

        public EditProfilePage()
        {
            InitializeComponent();
            usernameTextBox.Text = originalUsername;
            emailTextBox.Text = originalEmail;
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text.Trim();
            string email = emailTextBox.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                MessageDialog.ShowWarning(Window.GetWindow(this), Lang.Common_EmptyFieldsMessage);
                return;
            }

            if (username == originalUsername && email == originalEmail)
            {
                MessageDialog.ShowWarning(Window.GetWindow(this), Lang.EditProfile_NoChangesMessage);
                return;
            }

            MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.EditProfile_SaveSuccessMessage);
            NavigationService.Navigate(new ProfilePage());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            bool discard = MessageDialog.ShowConfirmation(Window.GetWindow(this), Lang.EditProfile_DiscardConfirmationMessage);

            if (discard)
            {
                NavigationService.Navigate(new ProfilePage());
            }
        }
    }
}