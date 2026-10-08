using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_DataAccess.DataAccess;
using cribbage_Royale_PROYECTO.Mail;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_DataAccess.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class VerifyAccountPage : Page
    {
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly VerificationCodeRepository _verificationCodeRepository = new VerificationCodeRepository();
        private readonly VerificationEmailSender _emailSender = new VerificationEmailSender();
        private readonly string _email;

        public VerifyAccountPage(string email)
        {
            InitializeComponent();
            _email = email;
            ShowSubtitle();
            verificationCodeTextBox.Focus();
        }

        private string GetDisplayEmail()
        {
            string displayEmail = Lang.VerifyAccount_UnknownEmail;

            if (!string.IsNullOrWhiteSpace(_email))
            {
                displayEmail = _email;
            }

            return displayEmail;
        }

        private void ShowSubtitle()
        {
            subtitleTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.VerifyAccount_SubtitleFormat,
                GetDisplayEmail());
        }

        private string GetVerificationCode()
        {
            return verificationCodeTextBox.Text.Trim();
        }

        private bool IsVerificationCodeEmpty()
        {
            return string.IsNullOrWhiteSpace(GetVerificationCode());
        }

        private void ShowVerificationSuccess()
        {
            verificationMessagePanel.HidePanel();
            MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.VerifyAccount_SuccessMessage);
            NavigationService.Navigate(new Login());
        }

        private void ProcessValidationOutcome(CodeValidationOutcome outcome)
        {
            switch (outcome)
            {
                case CodeValidationOutcome.Valid:
                    ShowVerificationSuccess();
                    break;
                case CodeValidationOutcome.ExpiredCode:
                    verificationMessagePanel.ShowError(Lang.VerifyAccount_ExpiredCodeMessage);
                    break;
                case CodeValidationOutcome.AccountAlreadyVerified:
                    verificationMessagePanel.ShowInformation(Lang.VerifyAccount_AlreadyVerifiedMessage);
                    break;
                case CodeValidationOutcome.DatabaseError:
                    MessageDialog.ShowError(Window.GetWindow(this), Lang.VerifyAccount_DatabaseErrorMessage);
                    break;
                case CodeValidationOutcome.InvalidCode:
                default:
                    verificationMessagePanel.ShowError(Lang.VerifyAccount_InvalidCodeMessage);
                    break;
            }
        }

        private void VerifyCode()
        {
            CodeValidationOutcome outcome = _verificationCodeRepository.ValidateAccountCode(
                _email,
                GetVerificationCode());

            ProcessValidationOutcome(outcome);
        }

        private void ShowResendResult(string code)
        {
            if (!string.IsNullOrEmpty(code) && _emailSender.SendVerificationCode(_email, code))
            {
                verificationMessagePanel.ShowInformation(string.Format(
                    CultureInfo.CurrentCulture,
                    Lang.VerifyAccount_CodeResentFormat,
                    GetDisplayEmail()));
            }
            else
            {
                verificationMessagePanel.ShowError(Lang.VerifyAccount_ResendFailedMessage);
            }
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsVerificationCodeEmpty())
            {
                verificationMessagePanel.ShowError(Lang.VerifyAccount_EmptyCodeMessage);
            }
            else
            {
                VerifyCode();
            }
        }

        private void ResendButton_Click(object sender, RoutedEventArgs e)
        {
            int userId = _userRepository.GetUserId(_email);
            string code = _verificationCodeRepository.CreateCode(
                userId,
                VerificationCodeRepository.AccountVerificationPurpose,
                _email);

            ShowResendResult(code);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }
    }
}