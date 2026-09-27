using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class VerifyAccountPage : Page
    {
        private readonly string _email;

        public VerifyAccountPage(string email = null)
        {
            InitializeComponent();

            _email = ResolveEmail(email);

            ShowSubtitle();
        }

        private string ResolveEmail(string email)
        {
            string resolvedEmail = email;

            if (string.IsNullOrWhiteSpace(email))
            {
                resolvedEmail = Lang.VerifyAccount_UnknownEmail;
            }

            return resolvedEmail;
        }

        private void ShowSubtitle()
        {
            subtitleTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.VerifyAccount_SubtitleFormat,
                _email);
        }

        private string GetVerificationCode()
        {
            return verificationCodeTextBox.Text.Trim();
        }

        private bool IsVerificationCodeEmpty()
        {
            return string.IsNullOrWhiteSpace(GetVerificationCode());
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsVerificationCodeEmpty())
            {
                verificationMessagePanel.ShowError(Lang.VerifyAccount_EmptyCodeMessage);
            }
            else
            {
                verificationMessagePanel.HidePanel();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }
    }
}