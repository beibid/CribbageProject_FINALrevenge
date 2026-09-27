using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class ChangeEmailPage : Page
    {
        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        private const char EmailSeparator = '@';
        private const string MaskCharacters = "***";

        private readonly string _currentEmail;

        public ChangeEmailPage(string currentEmail)
        {
            InitializeComponent();
            _currentEmail = currentEmail;
            ShowCurrentEmail();
            newEmailTextBox.Focus();
        }

        private void ShowCurrentEmail()
        {
            currentEmailTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.ChangeEmail_CurrentEmailFormat,
                MaskEmail(_currentEmail));
        }

        private string MaskEmail(string email)
        {
            string maskedEmail = Lang.ChangeEmail_UnknownEmail;

            if (!string.IsNullOrWhiteSpace(email))
            {
                maskedEmail = BuildMaskedEmail(email);
            }

            return maskedEmail;
        }

        private string BuildMaskedEmail(string email)
        {
            string maskedEmail = email;
            int separatorIndex = email.IndexOf(EmailSeparator);

            if (separatorIndex > 0)
            {
                maskedEmail = email.Substring(0, 1) + MaskCharacters + email.Substring(separatorIndex);
            }

            return maskedEmail;
        }

        private string GetNewEmail()
        {
            return newEmailTextBox.Text.Trim();
        }

        private string GetConfirmedEmail()
        {
            return confirmEmailTextBox.Text.Trim();
        }

        private string GetPassword()
        {
            return passwordBox.Password;
        }

        private bool AreFieldsEmpty()
        {
            bool areEmpty = false;

            if (string.IsNullOrWhiteSpace(GetNewEmail())
                || string.IsNullOrWhiteSpace(GetConfirmedEmail())
                || string.IsNullOrEmpty(GetPassword()))
            {
                areEmpty = true;
            }

            return areEmpty;
        }

        private bool DoEmailsMatch()
        {
            return string.Equals(GetNewEmail(), GetConfirmedEmail(), StringComparison.OrdinalIgnoreCase);
        }

        private bool IsEmailFormatValid()
        {
            return Regex.IsMatch(GetNewEmail(), EmailPattern);
        }

        private bool IsNewEmailDifferent()
        {
            return !string.Equals(GetNewEmail(), _currentEmail, StringComparison.OrdinalIgnoreCase);
        }

        private void ClearPasswordField()
        {
            passwordBox.Clear();
            passwordBox.Focus();
        }

        private void ClearForm()
        {
            newEmailTextBox.Clear();
            confirmEmailTextBox.Clear();
            passwordBox.Clear();
            changeEmailMessagePanel.HidePanel();
        }

        private void ContinueToVerification()
        {
            changeEmailMessagePanel.HidePanel();
            NavigationService.Navigate(new VerifyAccountPage(GetNewEmail()));
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if (AreFieldsEmpty())
            {
                changeEmailMessagePanel.ShowError(Lang.Validation_EmptyFields);
            }
            else if (!DoEmailsMatch())
            {
                changeEmailMessagePanel.ShowError(Lang.ChangeEmail_EmailsDoNotMatchMessage);
            }
            else if (!IsEmailFormatValid())
            {
                changeEmailMessagePanel.ShowError(Lang.ChangeEmail_InvalidEmailMessage);
            }
            else if (!IsNewEmailDifferent())
            {
                changeEmailMessagePanel.ShowError(Lang.ChangeEmail_SameEmailMessage);
            }
            else
            {
                ContinueToVerification();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            NavigationService.Navigate(new ProfilePage());
        }
    }
}