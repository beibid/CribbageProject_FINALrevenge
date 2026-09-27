using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class ResetPasswordPage : Page
    {
        private const int MinimumPasswordLength = 8;

        private readonly string _email;

        public ResetPasswordPage(string email)
        {
            InitializeComponent();
            _email = email;
            ShowEmail();
            verificationCodeTextBox.Focus();
        }

        private void ShowEmail()
        {
            emailTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.ResetPassword_EmailFormat,
                _email);
        }

        private string GetVerificationCode()
        {
            return verificationCodeTextBox.Text.Trim();
        }

        private string GetNewPassword()
        {
            return newPasswordBox.Password;
        }

        private string GetConfirmedPassword()
        {
            return confirmPasswordBox.Password;
        }

        private bool AreFieldsEmpty()
        {
            bool areEmpty = false;

            if (string.IsNullOrWhiteSpace(GetVerificationCode())
                || string.IsNullOrWhiteSpace(GetNewPassword())
                || string.IsNullOrWhiteSpace(GetConfirmedPassword()))
            {
                areEmpty = true;
            }

            return areEmpty;
        }

        private bool IsPasswordFormatValid()
        {
            string newPassword = GetNewPassword();
            bool isValid = false;

            if (newPassword.Length >= MinimumPasswordLength
                && newPassword.Any(char.IsLetter)
                && newPassword.Any(char.IsDigit))
            {
                isValid = true;
            }

            return isValid;
        }

        private bool DoPasswordsMatch()
        {
            return string.Equals(GetNewPassword(), GetConfirmedPassword(), System.StringComparison.Ordinal);
        }

        private void ClearPasswordFields()
        {
            newPasswordBox.Clear();
            confirmPasswordBox.Clear();
            newPasswordBox.Focus();
        }

        private void ReturnToLogin()
        {
            NavigationService.Navigate(new Login());
        }

        private void CompletePasswordReset()
        {
            resetMessagePanel.HidePanel();
            MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.ResetPassword_SuccessMessage);
            ReturnToLogin();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (AreFieldsEmpty())
            {
                resetMessagePanel.ShowError(Lang.ResetPassword_EmptyFieldsMessage);
            }
            else if (!IsPasswordFormatValid())
            {
                resetMessagePanel.ShowError(Lang.ResetPassword_InvalidPasswordMessage);
            }
            else if (!DoPasswordsMatch())
            {
                resetMessagePanel.ShowError(Lang.ResetPassword_PasswordsDoNotMatchMessage);
                ClearPasswordFields();
            }
            else
            {
                CompletePasswordReset();
            }
        }

        private void ResendCodeButton_Click(object sender, RoutedEventArgs e)
        {
            resetMessagePanel.ShowInformation(Lang.ResetPassword_CodeResentMessage);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            bool cancellationConfirmed = MessageDialog.ShowConfirmation(
                Window.GetWindow(this),
                Lang.ResetPassword_CancelConfirmationMessage);

            if (cancellationConfirmed)
            {
                ReturnToLogin();
            }
        }
    }
}