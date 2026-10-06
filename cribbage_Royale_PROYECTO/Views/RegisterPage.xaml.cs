using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.DataAccess;
using cribbage_Royale_PROYECTO.Mail;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class Register : Page
    {
        private const string UsernamePattern = @"^[A-Za-z0-9_]{3,16}$";
        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        private const int MinimumPasswordLength = 8;

        private readonly UserRepository _userRepository = new UserRepository();
        private readonly VerificationCodeRepository _verificationCodeRepository = new VerificationCodeRepository();
        private readonly VerificationEmailSender _emailSender = new VerificationEmailSender();

        public Register()
        {
            InitializeComponent();
            usernameTextBox.Focus();
        }

        private string GetUsername()
        {
            return usernameTextBox.Text.Trim();
        }

        private string GetEmail()
        {
            return emailTextBox.Text.Trim();
        }

        private string GetPassword()
        {
            return passwordBox.Password;
        }

        private string GetConfirmedPassword()
        {
            return confirmPasswordBox.Password;
        }

        private bool AreTermsAccepted()
        {
            return termsCheckBox.IsChecked == true;
        }

        private bool IsFormIncomplete()
        {
            bool isIncomplete = false;

            if (string.IsNullOrWhiteSpace(GetUsername()) || string.IsNullOrWhiteSpace(GetEmail())
                || string.IsNullOrEmpty(GetPassword()) || string.IsNullOrEmpty(GetConfirmedPassword())
                || !AreTermsAccepted())
            {
                isIncomplete = true;
            }

            return isIncomplete;
        }

        private bool IsUsernameFormatValid()
        {
            return Regex.IsMatch(GetUsername(), UsernamePattern);
        }

        private bool IsEmailFormatValid()
        {
            return Regex.IsMatch(GetEmail(), EmailPattern);
        }

        private bool IsPasswordFormatValid()
        {
            string password = GetPassword();
            bool isValidPassword = false;

            if (password.Length >= MinimumPasswordLength
                && password.Any(char.IsLetter)
                && password.Any(char.IsDigit))
            {
                isValidPassword = true;
            }

            return isValidPassword;
        }

        private bool DoPasswordsMatch()
        {
            return string.Equals(GetPassword(), GetConfirmedPassword(), 
                StringComparison.Ordinal);
        }

        private void ClearPasswordFields()
        {
            passwordBox.Clear();
            confirmPasswordBox.Clear();
            passwordBox.Focus();
        }

        private void ClearForm()
        {
            usernameTextBox.Clear();
            emailTextBox.Clear();
            passwordBox.Clear();
            confirmPasswordBox.Clear();
            termsCheckBox.IsChecked = false;
            registerMessagePanel.HidePanel();
        }

        private void ShowAwaitingVerificationPage(VerificationNotice notice)
        {
            registerMessagePanel.HidePanel();
            NavigationService.Navigate(new AwaitingVerificationPage(GetEmail(), notice));
        }

        private void ShowDatabaseError()
        {
            MessageDialog.ShowError(Window.GetWindow(this), Lang.Register_DatabaseErrorMessage);
        }

        private VerificationNotice SendCodeByEmail(string codeEmail)
        {
            VerificationNotice noticeCodeEmail = VerificationNotice.EmailNotSent;

            if (_emailSender.SendVerificationCode(GetEmail(), codeEmail))
            {
                noticeCodeEmail = VerificationNotice.CodeSent;
            }

            return noticeCodeEmail;
        }

        private VerificationNotice DeliverCode(string codeEmail)
        {
            VerificationNotice noticeCodeEmail = VerificationNotice.EmailNotSent;

            if (!string.IsNullOrEmpty(codeEmail))
            {
                noticeCodeEmail = SendCodeByEmail(codeEmail);
            }

            return noticeCodeEmail;
        }

        private void CompleteRegistration()
        {
            int userId = _userRepository.GetUserId(GetEmail());
            string codeVerificationEmail = _verificationCodeRepository.CreateCode(
                userId,
                VerificationCodeRepository.AccountVerificationPurpose,
                GetEmail());

            VerificationNotice noticeRegistration = DeliverCode(codeVerificationEmail);

            MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.Register_SuccessMessage);
            ShowAwaitingVerificationPage(noticeRegistration);
        }

        private void ProcessRegistrationOutcome(RegistrationOutcome outcome)
        {
            switch (outcome)
            {
                case RegistrationOutcome.Success:
                    CompleteRegistration();
                    break;
                case RegistrationOutcome.EmailPendingVerification:
                    ShowAwaitingVerificationPage(VerificationNotice.AccountAlreadyPending);
                    break;
                case RegistrationOutcome.EmailAlreadyRegistered:
                    registerMessagePanel.ShowError(Lang.Register_EmailAlreadyRegisteredMessage);
                    break;
                case RegistrationOutcome.DatabaseError:
                    ShowDatabaseError();
                    break;
                case RegistrationOutcome.UsernameAlreadyTaken:
                default:
                    registerMessagePanel.ShowError(Lang.Register_UsernameAlreadyTakenMessage);
                    break;
            }
        }

        private void SaveAccount()
        {
            RegistrationOutcome outcome = _userRepository.RegisterAccount(
                GetUsername(),
                GetEmail(),
                GetPassword());

            ProcessRegistrationOutcome(outcome);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsFormIncomplete())
            {
                registerMessagePanel.ShowError(Lang.Register_EmptyFieldsMessage);
            }
            else if (!IsUsernameFormatValid())
            {
                registerMessagePanel.ShowError(Lang.Register_InvalidUsernameMessage);
            }
            else if (!IsEmailFormatValid())
            {
                registerMessagePanel.ShowError(Lang.Register_InvalidEmailMessage);
            }
            else if (!IsPasswordFormatValid())
            {
                registerMessagePanel.ShowError(Lang.Register_InvalidPasswordMessage);
            }
            else if (!DoPasswordsMatch())
            {
                registerMessagePanel.ShowError(Lang.Register_PasswordsDoNotMatchMessage);
                ClearPasswordFields();
            }
            else
            {
                SaveAccount();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            bool cancellationConfirmed = MessageDialog.ShowConfirmation(
                Window.GetWindow(this),
                Lang.Register_CancelConfirmationMessage);

            if (cancellationConfirmed)
            {
                ClearForm();
                NavigationService.Navigate(new Login());
            }
        }
    }
}