using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views.Messages
{
    public partial class MessagePanel : UserControl
    {
        private const string ErrorStyleKey = "MessagePanelError";
        private const string WarningStyleKey = "MessagePanelWarning";
        private const string InformationStyleKey = "MessagePanelInformation";

        public MessagePanel()
        {
            InitializeComponent();
        }

        public void ShowError(string message)
        {
            ShowMessage(message, Lang.MessagePanel_ErrorLabel, ErrorStyleKey);
        }

        public void ShowWarning(string message)
        {
            ShowMessage(message, Lang.MessagePanel_WarningLabel, WarningStyleKey);
        }

        public void ShowInformation(string message)
        {
            ShowMessage(message, Lang.MessagePanel_InformationLabel, InformationStyleKey);
        }

        public void HidePanel()
        {
            messageBorder.Visibility = Visibility.Collapsed;
        }

        private void ShowMessage(string message, string severityLabel, string styleKey)
        {
            messageBorder.Style = (Style)FindResource(styleKey);
            severityLabelTextBlock.Text = severityLabel;
            messageTextBlock.Text = message;
            messageBorder.Visibility = Visibility.Visible;
        }

        private void DismissButton_Click(object sender, RoutedEventArgs e)
        {
            HidePanel();
        }
    }
}