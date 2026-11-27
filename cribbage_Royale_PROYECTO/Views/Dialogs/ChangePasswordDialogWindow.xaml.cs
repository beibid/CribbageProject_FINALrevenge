using System.Threading.Tasks;
using System.Windows;
using cribbage_Royale_PROYECTO.Localization;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;
using cribbage_Royale_PROYECTO.Session;

namespace cribbage_Royale_PROYECTO.Views.Dialogs
{
    public partial class ChangePasswordDialogWindow : Window
    {
        private readonly PlayerProfileClient _profileClient;

        public ChangePasswordDialogWindow()
        {
            InitializeComponent();
            _profileClient = ((App)Application.Current).PlayerProfileClient;
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = currentPasswordBox.Password;
            string newPassword = newPasswordBox.Password;
            string confirmPassword = confirmPasswordBox.Password;

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword)
                || string.IsNullOrEmpty(confirmPassword))
            {
                MessageDialog.ShowWarning(this, Lang.Common_EmptyFieldsMessage);
            }
            else if (newPassword != confirmPassword)
            {
                MessageDialog.ShowWarning(this, Lang.ChangePassword_MismatchMessage);
            }
            else if (newPassword == currentPassword)
            {
                MessageDialog.ShowWarning(this, Lang.ChangePassword_SamePasswordMessage);
            }
            else
            {
                await ChangePasswordAsync(currentPassword, newPassword);
            }
        }

        private async Task ChangePasswordAsync(string currentPassword, string newPassword)
        {
            confirmButton.IsEnabled = false;
            cancelButton.IsEnabled = false;

            ProfileResult result = await _profileClient.ChangePasswordAsync(
                CurrentSession.SessionToken,
                currentPassword,
                newPassword);

            confirmButton.IsEnabled = true;
            cancelButton.IsEnabled = true;

            if (result.Succeeded)
            {
                MessageDialog.ShowSuccess(this, Lang.ChangePassword_SuccessMessage);
                DialogResult = true;
            }
            else
            {
                MessageDialog.ShowWarning(this, ProfileMessages.Resolve(result));
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}