using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class DeleteAccountPage : Page
    {
        public DeleteAccountPage()
        {
            InitializeComponent();
            ShowConfirmationLabel();
            passwordBox.Focus();
        }

        private void ShowConfirmationLabel()
        {
            confirmationLabelTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.DeleteAccount_ConfirmationLabelFormat,
                Lang.DeleteAccount_ConfirmationWord);
        }

        private string GetPassword()
        {
            return passwordBox.Password;
        }

        private string GetConfirmationWord()
        {
            return confirmationTextBox.Text.Trim();
        }

        private bool AreFieldsEmpty()
        {
            bool areEmpty = false;

            if (string.IsNullOrEmpty(GetPassword())
                || string.IsNullOrWhiteSpace(GetConfirmationWord()))
            {
                areEmpty = true;
            }

            return areEmpty;
        }

        private bool IsConfirmationWordCorrect()
        {
            return string.Equals(
                GetConfirmationWord(),
                Lang.DeleteAccount_ConfirmationWord,
                StringComparison.OrdinalIgnoreCase);
        }

        private void ShowInvalidConfirmationWord()
        {
            string message = string.Format(
                CultureInfo.CurrentCulture,
                Lang.DeleteAccount_InvalidConfirmationWordFormat,
                Lang.DeleteAccount_ConfirmationWord);

            deleteAccountMessagePanel.ShowError(message);
        }

        private void ClearPasswordField()
        {
            passwordBox.Clear();
            passwordBox.Focus();
        }

        private void ClearForm()
        {
            passwordBox.Clear();
            confirmationTextBox.Clear();
            deleteAccountMessagePanel.HidePanel();
        }

        private void CompleteDeletion()
        {
            MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.DeleteAccount_SuccessMessage);
            NavigationService.Navigate(new StartMenu());
        }

        private void RequestDeletionConfirmation()
        {
            deleteAccountMessagePanel.HidePanel();

            bool deletionConfirmed = MessageDialog.ShowConfirmation(
                Window.GetWindow(this),
                Lang.DeleteAccount_ConfirmationMessage);

            if (deletionConfirmed)
            {
                CompleteDeletion();
            }
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (AreFieldsEmpty())
            {
                deleteAccountMessagePanel.ShowError(Lang.Validation_EmptyFields);
            }
            else if (!IsConfirmationWordCorrect())
            {
                ShowInvalidConfirmationWord();
            }
            else
            {
                RequestDeletionConfirmation();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            NavigationService.Navigate(new ProfilePage());
        }
    }
}