using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_DataAccess.DataAccess;
using cribbage_Royale_PROYECTO.Mail;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_DataAccess.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class AwaitingVerificationPage : Page
    {
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly VerificationCodeRepository _verificationCodeRepository = new VerificationCodeRepository();
        private readonly VerificationEmailSender _emailSender = new VerificationEmailSender();
        private readonly string _email;
        private readonly VerificationNotice _notice;

        public AwaitingVerificationPage(string email, VerificationNotice notice)
        {
            InitializeComponent();
            _email = email;
            _notice = notice;
            ShowNotice();
            ShowAvailableOptions();
        }

        private string GetDisplayEmail()
        {
            string displayEmail = Lang.AwaitingVerification_UnknownEmail;

            if (!string.IsNullOrWhiteSpace(_email))
            {
                displayEmail = _email;
            }

            return displayEmail;
        }

        private string GetCodeSentText()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Lang.AwaitingVerification_CodeSentFormat,
                GetDisplayEmail());
        }

        private string GetNoticeText()
        {
            string noticeText;

            switch (_notice)
            {
                case VerificationNotice.AccountAlreadyPending:
                    noticeText = Lang.AwaitingVerification_AccountAlreadyPending;
                    break;
                case VerificationNotice.EmailNotSent:
                    noticeText = Lang.AwaitingVerification_EmailNotSent;
                    break;
                case VerificationNotice.AccountNotVerified:
                    noticeText = Lang.AwaitingVerification_AccountNotVerified;
                    break;
                case VerificationNotice.CodeSent:
                default:
                    noticeText = GetCodeSentText();
                    break;
            }

            return noticeText;
        }

        private void ShowNotice()
        {
            noticeTextBlock.Text = GetNoticeText();
        }

        private void ShowAvailableOptions()
        {
            if (_notice == VerificationNotice.EmailNotSent)
            {
                alreadyVerifiedButton.Visibility = Visibility.Collapsed;
                closeButton.Visibility = Visibility.Visible;
            }
            else
            {
                alreadyVerifiedButton.Visibility = Visibility.Visible;
                closeButton.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowVerificationOption()
        {
            alreadyVerifiedButton.Visibility = Visibility.Visible;
            closeButton.Visibility = Visibility.Collapsed;
        }

        private void ShowResendResult(string code)
        {
            if (!string.IsNullOrEmpty(code) && _emailSender.SendVerificationCode(_email, code))
            {
                verificationMessagePanel.ShowInformation(GetCodeSentText());
                ShowVerificationOption();
            }
            else
            {
                verificationMessagePanel.ShowError(Lang.AwaitingVerification_EmailNotSent);
            }
        }

        private void AlreadyVerifiedButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new VerifyAccountPage(_email));
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }
    }
}