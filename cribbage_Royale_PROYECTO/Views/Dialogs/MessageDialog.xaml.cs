using System.Windows;
using System.Windows.Media;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views.Dialogs
{
    // Reusable dialog for success, error and confirmation messages
    public partial class MessageDialog : Window
    {
        private MessageDialog()
        {
            InitializeComponent();
        }

        // Shows a success message with a green panel
        public static void ShowSuccess(Window owner, string message)
        {
            MessageDialog dialog = Create(owner, Lang.Dialog_SuccessTitle, message);
            dialog.ApplyColor((Brush)dialog.FindResource("SuccessBrush"));
            dialog.AcceptButton.Content = Lang.Dialog_AcceptButton;
            dialog.ShowDialog();
        }

        // Shows an error message with a red panel
        public static void ShowError(Window owner, string message)
        {
            MessageDialog dialog = Create(owner, Lang.Dialog_ErrorTitle, message);
            dialog.ApplyColor((Brush)dialog.FindResource("ErrorBrush"));
            dialog.AcceptButton.Content = Lang.Dialog_CloseButton;
            dialog.ShowDialog();
        }

        // Asks a yes or no question. Returns true when the player answers yes.
        public static bool ShowConfirmation(Window owner, string message)
        {
            MessageDialog dialog = Create(owner, Lang.Dialog_ConfirmationTitle, message);
            dialog.AcceptButton.Content = Lang.Dialog_YesButton;
            dialog.CancelButton.Content = Lang.Dialog_NoButton;
            dialog.CancelButton.Visibility = Visibility.Visible;

            return dialog.ShowDialog() == true;
        }

        // Creates the dialog with its title and message
        private static MessageDialog Create(Window owner, string title, string message)
        {
            MessageDialog dialog = new MessageDialog();
            dialog.Owner = owner;
            dialog.TitleText.Text = title;
            dialog.MessageText.Text = message;
            return dialog;
        }

        // Paints the border, the title and the main button with the given color
        private void ApplyColor(Brush color)
        {
            DialogBorder.BorderBrush = color;
            TitleText.Foreground = color;
            AcceptButton.Background = color;
            AcceptButton.BorderBrush = color;
            AcceptButton.Foreground = Brushes.White;
        }
        // Shows a notice in gold, for validation messages
        public static void ShowWarning(Window owner, string message)
        {
            MessageDialog dialog = Create(owner, Lang.Dialog_NoticeTitle, message);
            dialog.AcceptButton.Content = Lang.Dialog_AcceptButton;
            dialog.ShowDialog();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}