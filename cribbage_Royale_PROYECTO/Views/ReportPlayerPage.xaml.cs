using cribbage_Royale_DataAccess.DataAccess;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_DataAccess.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Session;
using cribbage_Royale_PROYECTO.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class ReportPlayerPage : Page
    {
        private readonly ReportRepository _reportRepository;
        private readonly BlockRepository _blockRepository;
        private readonly UserRepository _userRepository;
        private readonly int _reporterAccountId;
        private readonly int _reportedAccountId;
        private readonly int? _matchId;

        public ReportPlayerPage(int reportedAccountId, string reportedDisplayName, int? matchId)
        {
            InitializeComponent();
            _reportRepository = new ReportRepository();
            _blockRepository = new BlockRepository();
            _userRepository = new UserRepository();
            _reporterAccountId = _userRepository.GetUserId(CurrentSession.Username);
            _reportedAccountId = reportedAccountId;
            _matchId = matchId;
            reportedUsernameTextBlock.Text = string.Format(Lang.ReportPlayer_ReportedUsernameFormat, reportedDisplayName);
            reasonComboBox.ItemsSource = BuildReasonOptions();
        }

        private List<ReportReasonOption> BuildReasonOptions()
        {
            List<ReportReasonOption> options = new List<ReportReasonOption>();
            options.Add(new ReportReasonOption { DisplayText = Lang.ReportPlayer_ReasonMisconduct, ReasonCode = ReportReason.Misconduct });
            options.Add(new ReportReasonOption { DisplayText = Lang.ReportPlayer_ReasonCheating, ReasonCode = ReportReason.Cheating });
            options.Add(new ReportReasonOption { DisplayText = Lang.ReportPlayer_ReasonOffensiveName, ReasonCode = ReportReason.OffensiveName });
            return options;
        }

        private void SubmitReportButton_Click(object sender, RoutedEventArgs e)
        {
            SubmitReport(false);
        }

        private void SubmitAndBlockButton_Click(object sender, RoutedEventArgs e)
        {
            SubmitReport(true);
        }

        private void SubmitReport(bool alsoBlock)
        {
            messagePanel.HidePanel();
            ReportReasonOption selectedReason = (ReportReasonOption)reasonComboBox.SelectedItem;
            if (selectedReason == null)
            {
                messagePanel.ShowWarning(Lang.ReportPlayer_MissingReasonMessage);
            }
            else
            {
                CheckForDuplicateAndSubmit(selectedReason.ReasonCode, alsoBlock);
            }
        }

        private void CheckForDuplicateAndSubmit(string reasonCode, bool alsoBlock)
        {
            try
            {
                string existingFolio = _reportRepository.FindExistingReportFolio(_reporterAccountId, _reportedAccountId, _matchId);
                if (existingFolio == null)
                {
                    CreateReportAndContinue(reasonCode, alsoBlock);
                }
                else
                {
                    messagePanel.ShowInformation(string.Format(Lang.ReportPlayer_DuplicateReportMessageFormat, existingFolio));
                }
            }
            catch (Exception dataAccessException)
            {
                messagePanel.ShowError(Lang.Dialog_DatabaseAccessErrorMessage);
            }
        }

        private void CreateReportAndContinue(string reasonCode, bool alsoBlock)
        {
            try
            {
                string generatedFolio = _reportRepository.SubmitReport(_reporterAccountId, _reportedAccountId, _matchId, reasonCode, commentTextBox.Text.Trim());
                if (alsoBlock)
                {
                    _blockRepository.BlockPlayer(_reporterAccountId, _reportedAccountId);
                }
                MessageDialog.ShowSuccess(Window.GetWindow(this), string.Format(Lang.ReportPlayer_SuccessMessageFormat, generatedFolio));
                NavigationService.GoBack();
            }
            catch (Exception dataAccessException)
            {
                messagePanel.ShowError(Lang.Dialog_DatabaseAccessErrorMessage);

            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}