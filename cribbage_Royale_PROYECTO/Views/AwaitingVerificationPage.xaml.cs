using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class AwaitingVerificationPage : Page
    {
        private readonly string _email;
        private readonly VerificationNotice _notice;

        public AwaitingVerificationPage(
            string email = null,
            VerificationNotice notice = VerificationNotice.CodeSent)
        {
            InitializeComponent();

            _email = ResolveEmail(email);
            _notice = notice;

            ShowNoticeMessage();
            ShowAvailableOptions();
        }

        private string ResolveEmail(string email)
        {
            string resolvedEmail = email;

            if (string.IsNullOrWhiteSpace(email))
            {
                resolvedEmail = Lang.AwaitingVerification_UnknownEmail;
            }

            return resolvedEmail;
        }

        private void ShowNoticeMessage()
        {
            noticeTextBlock.Text = GetNoticeText();
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

        private string GetCodeSentText()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Lang.AwaitingVerification_CodeSentFormat,
                _email);
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

        private void AlreadyVerifiedButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new VerifyAccountPage(_email));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }
    }
}