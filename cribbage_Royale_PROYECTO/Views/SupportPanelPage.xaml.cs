using cribbage_Royale_DataAccess.DataAccess;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_DataAccess.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Session;
using cribbage_Royale_PROYECTO.Views.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class SupportPanelPage : Page
    {
        private readonly SupportRepository _supportRepository;

        public SupportPanelPage()
        {
            InitializeComponent();
            _supportRepository = new SupportRepository();
            LoadPendingReports();
            LoadSuspendedAccounts();
        }

        private void LoadPendingReports()
        {
            try
            {
                pendingReportsDataGrid.ItemsSource = _supportRepository.GetPendingReports();
            }
            catch (Exception dataAccessException)
            {
                MessageDialog.ShowError(Window.GetWindow(this), Lang.Dialog_DatabaseAccessErrorMessage);
            }
        }

        private void LoadSuspendedAccounts()
        {
            try
            {
                suspendedAccountsDataGrid.ItemsSource = _supportRepository.GetActiveSuspensions();
            }
            catch (Exception dataAccessException)
            {
                MessageDialog.ShowError(Window.GetWindow(this), Lang.Dialog_DatabaseAccessErrorMessage);
            }
        }

        private void ReviewReportButton_Click(object sender, RoutedEventArgs e)
        {
            Button reviewButton = (Button)sender;
            PendingReportSummary selectedReport = (PendingReportSummary)reviewButton.DataContext;
        }

        private void ReactivateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            Button reactivateButton = (Button)sender;
            SuspendedAccountSummary selectedAccount = (SuspendedAccountSummary)reactivateButton.DataContext;
            bool confirmed = MessageDialog.ShowConfirmation(Window.GetWindow(this), Lang.Dialog_ConfirmReactivateAccountMessage);
            if (confirmed)
            {
                try
                {
                    _supportRepository.ReactivateAccount(selectedAccount.AccountId, CurrentSession.SupportAccountId);
                    LoadSuspendedAccounts();
                }
                catch (Exception dataAccessException)
                {
                    MessageDialog.ShowError(Window.GetWindow(this), Lang.Dialog_DatabaseAccessErrorMessage);
                }
            }
        }
    }
}