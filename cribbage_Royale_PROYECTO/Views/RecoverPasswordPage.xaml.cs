using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class RecoverPasswordPage : Page
    {
        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        public RecoverPasswordPage()
        {
            InitializeComponent();
            emailTextBox.Focus();
        }

        private string GetEmail()
        {
            return emailTextBox.Text.Trim();
        }

        private bool IsEmailEmpty()
        {
            return string.IsNullOrWhiteSpace(GetEmail());
        }

        private bool IsEmailFormatValid()
        {
            return Regex.IsMatch(GetEmail(), EmailPattern);
        }

        private void ShowResetPasswordPage()
        {
            NavigationService.Navigate(new ResetPasswordPage(GetEmail()));
        }

        private void ReturnToLogin()
        {
            NavigationService.Navigate(new Login());
        }

        private void SendCodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsEmailEmpty())
            {
                recoverMessagePanel.ShowError(Lang.RecoverPassword_EmptyEmailMessage);
            }
            else if (!IsEmailFormatValid())
            {
                recoverMessagePanel.ShowError(Lang.RecoverPassword_InvalidEmailMessage);
            }
            else
            {
                recoverMessagePanel.HidePanel();
                ShowResetPasswordPage();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            bool cancellationConfirmed = MessageDialog.ShowConfirmation(
                Window.GetWindow(this),
                Lang.RecoverPassword_CancelConfirmationMessage);

            if (cancellationConfirmed)
            {
                ReturnToLogin();
            }
        }
    }
}