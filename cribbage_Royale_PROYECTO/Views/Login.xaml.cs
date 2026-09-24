using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Linq;
using cribbage_Royale_PROYECTO.Views.Dialogs;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

  
        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Register());
        }

   
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartMenu());
        }
        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageDialog.ShowWarning(Window.GetWindow(this), Lang.Validation_EmptyFields);
                return;
            }
            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.Login_SuccessMessage);
                    NavigationService.Navigate(new Lobby());
                }
            }
            catch (Exception exeptionLogin)
            {
                MessageDialog.ShowError(Window.GetWindow(this), exeptionLogin.Message);
            }
        }
    }
}
