using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Session;
using cribbage_Royale_PROYECTO.Views.Dialogs;
using cribbage_Royale_PROYECTO.Views.Messages;
using cribbage_Royale_PROYECTO.DataAccess;
using cribbage_Royale_PROYECTO.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace cribbage_Royale_PROYECTO.Views.Dialogs
{
    public partial class SuspendAccountDialogWindow : Window
    {
        private const int OneDayDurationInDays = 1;
        private const int ThreeDaysDurationInDays = 3;
        private const int SevenDaysDurationInDays = 7;
        private const int ThirtyDaysDurationInDays = 30;

        private readonly SupportRepository _supportRepository;
        private readonly int _reportedAccountId;
        private readonly int? _reportId;
        private readonly string _reportedAccountEmail;

        public bool SanctionApplied { get; private set; }

        public SuspendAccountDialogWindow(int reportedAccountId, int? reportId, string reportedUsername, string reportedAccountEmail, string prefilledReason)
        {
            InitializeComponent();
            _supportRepository = new SupportRepository();
            _reportedAccountId = reportedAccountId;
            _reportId = reportId;
            _reportedAccountEmail = reportedAccountEmail;
            SanctionApplied = false;
            usernameSubtitleTextBlock.Text = string.Format(Lang.SuspendAccount_UsernameFormat, reportedUsername);
            reasonTextBox.Text = prefilledReason;
            durationComboBox.ItemsSource = BuildDurationOptions();
            UpdateApplyButtonState();
        }

        private List<SuspensionDurationOption> BuildDurationOptions()
        {
            List<SuspensionDurationOption> options = new List<SuspensionDurationOption>();
            options.Add(new SuspensionDurationOption { DisplayText = Lang.SuspendAccount_Duration1Day, DurationInDays = OneDayDurationInDays });
            options.Add(new SuspensionDurationOption { DisplayText = Lang.SuspendAccount_Duration3Days, DurationInDays = ThreeDaysDurationInDays });
            options.Add(new SuspensionDurationOption { DisplayText = Lang.SuspendAccount_Duration7Days, DurationInDays = SevenDaysDurationInDays });
            options.Add(new SuspensionDurationOption { DisplayText = Lang.SuspendAccount_Duration30Days, DurationInDays = ThirtyDaysDurationInDays });
            return options;
        }

        private void ReasonTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateApplyButtonState();
        }

        private void DurationComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateApplyButtonState();
        }

        private void UpdateApplyButtonState()
        {
            bool hasReason = reasonTextBox.Text.Trim().Length > 0;
            bool hasDuration = durationComboBox.SelectedItem != null;
            if (hasReason && hasDuration)
            {
                applyButton.IsEnabled = true;
            }
            else
            {
                applyButton.IsEnabled = false;
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            messagePanel.HidePanel();
            SuspensionDurationOption selectedDuration = (SuspensionDurationOption)durationComboBox.SelectedItem;
            DateTime suspensionEndDate = CalculateSuspensionEndDate(selectedDuration.DurationInDays);
            AccountSanctionStatus currentStatus = _supportRepository.GetSanctionStatus(_reportedAccountId);
            if (currentStatus.IsCurrentlySuspended)
            {
                ShowAlreadySuspendedState(currentStatus);
            }
            else
            {
                ApplyNewSuspension(reasonTextBox.Text.Trim(), suspensionEndDate);
            }
        }

        private DateTime CalculateSuspensionEndDate(int durationInDays)
        {
            DateTime suspensionEndDate = DateTime.Now.AddDays(durationInDays);
            return suspensionEndDate;
        }

        private void ApplyNewSuspension(string reason, DateTime suspensionEndDate)
        {
            try
            {
                _supportRepository.ApplyNewSuspension(_reportedAccountId, _reportId, reason, suspensionEndDate, CurrentSession.SupportAccountId);
                SendSanctionEmail(reason, suspensionEndDate);
                SanctionApplied = true;
                MessageDialog.ShowSuccess(this, Lang.SuspendAccount_SuccessMessage);
                Close();
            }
            catch (Exception dataAccessException)
            {
                messagePanel.ShowError(Lang.SuspendAccount_ApplyErrorMessage);
            }
        }

        private void SendSanctionEmail(string reason, DateTime suspensionEndDate)
        {
            try
            {
                Mail.SanctionEmailSender sanctionEmailSender = new Mail.SanctionEmailSender();
                sanctionEmailSender.SendSanctionNotification(_reportedAccountEmail, reason, suspensionEndDate);
            }
            catch (Exception mailException)
            {
                messagePanel.ShowWarning(Lang.SuspendAccount_EmailPendingMessage);
            }
        }

        private void ShowAlreadySuspendedState(AccountSanctionStatus currentStatus)
        {
            messagePanel.ShowInformation(string.Format(Lang.SuspendAccount_AlreadySuspendedMessage, currentStatus.SuspensionEndDate));
            applyButton.Visibility = Visibility.Collapsed;
            modifyDurationButton.Visibility = Visibility.Visible;
            reactivateButton.Visibility = Visibility.Visible;
        }

        private void ModifyDurationButton_Click(object sender, RoutedEventArgs e)
        {
            messagePanel.HidePanel();
            applyButton.Visibility = Visibility.Visible;
            modifyDurationButton.Visibility = Visibility.Collapsed;
            reactivateButton.Visibility = Visibility.Collapsed;
            SuspensionDurationOption selectedDuration = (SuspensionDurationOption)durationComboBox.SelectedItem;
            DateTime suspensionEndDate = CalculateSuspensionEndDate(selectedDuration.DurationInDays);
            try
            {
                _supportRepository.UpdateSuspensionDuration(_reportedAccountId, reasonTextBox.Text.Trim(), suspensionEndDate, CurrentSession.SupportAccountId);
                SanctionApplied = true;
                MessageDialog.ShowSuccess(this, Lang.SuspendAccount_SuccessMessage);
                Close();
            }
            catch (Exception dataAccessException)
            {
                messagePanel.ShowError(Lang.SuspendAccount_ApplyErrorMessage);
            }
        }

        private void ReactivateButton_Click(object sender, RoutedEventArgs e)
        {
            bool confirmed = MessageDialog.ShowConfirmation(this, Lang.Dialog_ConfirmReactivateAccountMessage);
            if (confirmed)
            {
                try
                {
                    _supportRepository.ReactivateAccount(_reportedAccountId, CurrentSession.SupportAccountId);
                    SanctionApplied = true;
                    Close();
                }
                catch (Exception dataAccessException)
                {
                    messagePanel.ShowError(Lang.SuspendAccount_ApplyErrorMessage);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}