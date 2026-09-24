using System.Windows;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views.Dialogs
{
    public partial class ChangePasswordDialogWindow : Window
    {
        public ChangePasswordDialogWindow()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = currentPasswordBox.Password;
            string newPassword = newPasswordBox.Password;
            string confirmPassword = confirmPasswordBox.Password;

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword)
                || string.IsNullOrEmpty(confirmPassword))
            {
                MessageDialog.ShowWarning(this, Lang.Common_EmptyFieldsMessage);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageDialog.ShowWarning(this, Lang.ChangePassword_MismatchMessage);
                return;
            }

            if (newPassword == currentPassword)
            {
                MessageDialog.ShowWarning(this, Lang.ChangePassword_SamePasswordMessage);
                return;
            }

            MessageDialog.ShowSuccess(this, Lang.ChangePassword_SuccessMessage);
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}