using System.Linq;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Localization;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Session;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class Settings : Page
    {
        private const int DefaultLanguageIndex = 0;
        private const int DefaultColorBlindModeIndex = 0;
        private const double DefaultMusicVolume = 70;
        private const double DefaultEffectsVolume = 80;

        public Settings()
        {
            InitializeComponent();
            SelectCurrentLanguage();
            ShowAvailableOptions();
        }

        private ComboBoxItem FindLanguageItem(string cultureCode)
        {
            return languageComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => (string)item.Tag == cultureCode);
        }

        private void SelectCurrentLanguage()
        {
            ComboBoxItem matchingItem = FindLanguageItem(LanguageManager.CurrentCultureCode);

            if (matchingItem == null)
            {
                languageComboBox.SelectedIndex = DefaultLanguageIndex;
            }
            else
            {
                languageComboBox.SelectedItem = matchingItem;
            }
        }
        private bool HasActiveSession()
        {
            return CurrentSession.Kind != CurrentSession.SessionKind.None;
        }

        private void ShowAvailableOptions()
        {
            if (HasActiveSession())
            {
                logoutButton.Visibility = Visibility.Visible;
            }
            else
            {
                logoutButton.Visibility = Visibility.Collapsed;
            }
        }

        private void ReturnToPreviousScreen()
        {
            if (HasActiveSession())
            {
                NavigationService.Navigate(new Lobby());
            }
            else
            {
                NavigationService.Navigate(new StartMenu());
            }
        }

        private void RestoreDefaultSettings()
        {
            musicVolumeSlider.Value = DefaultMusicVolume;
            effectsVolumeSlider.Value = DefaultEffectsVolume;
            languageComboBox.SelectedIndex = DefaultLanguageIndex;
            colorBlindModeComboBox.SelectedIndex = DefaultColorBlindModeIndex;
            animationsToggle.IsChecked = true;
            friendRequestNotificationsToggle.IsChecked = true;
        }

        private string GetSelectedCulture()
        {
            ComboBoxItem selectedItem = (ComboBoxItem)languageComboBox.SelectedItem;

            return (string)selectedItem.Tag;
        }

        private void ApplySelectedLanguage()
        {
            string selectedCulture = GetSelectedCulture();

            if (selectedCulture != LanguageManager.CurrentCultureCode)
            {
                LanguageManager.ApplyCulture(selectedCulture);
                NavigationService.Navigate(new Settings());
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ApplySelectedLanguage();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            RestoreDefaultSettings();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            bool logoutConfirmed = MessageDialog.ShowConfirmation(
                Window.GetWindow(this),
                Lang.Settings_LogoutConfirmationMessage);

            if (logoutConfirmed)
            {
                CurrentSession.EndSession();
                NavigationService.Navigate(new StartMenu());
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnToPreviousScreen();
        }
    }
}