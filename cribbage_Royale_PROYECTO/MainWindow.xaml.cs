using System;
using System.ComponentModel;
using System.Windows;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;
using cribbage_Royale_PROYECTO.Views;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO
{
    public partial class MainWindow : Window
    {
        private readonly PlayerSessionClient _sessionClient;

        private bool _sessionEnded;

        public MainWindow()
        {
            InitializeComponent();
            _sessionClient = ((App)Application.Current).SessionClient;
            _sessionClient.ConnectionLost += SessionClient_ConnectionLost;
            ((App)Application.Current).SessionCallback.RevocationReceived += SessionCallback_RevocationReceived;
            MainFrame.Navigate(new StartMenu());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            RequestSignOut();
            base.OnClosing(e);
        }

        private void RequestSignOut()
        {
            if (!_sessionEnded && _sessionClient.IsConnected())
            {
                _sessionEnded = true;
                _sessionClient.SignOutAsync();
            }
        }

        private void SessionClient_ConnectionLost(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => ShowConnectionLost()));
        }

        private void SessionCallback_RevocationReceived(object sender, SessionRevokedEventArgs e)
        {
            if (!_sessionEnded)
            {
                _sessionEnded = true;
                MessageDialog.ShowError(this, Lang.Session_SignedInElsewhereMessage);
                ReturnToLogin();
            }
        }

        private void ShowConnectionLost()
        {
            if (!_sessionEnded)
            {
                _sessionEnded = true;
                MessageDialog.ShowError(this, Lang.Session_ConnectionLostMessage);
                ReturnToLogin();
            }
        }

        private void ReturnToLogin()
        {
            MainFrame.Navigate(new Login());
        }
    }
}