using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using cribbage_Royale_PROYECTO.Localization;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
            SelectCurrentLanguage();
        }

        // Selects in the combo box the language that is currently active
        private void SelectCurrentLanguage()
        {
            string currentCulture = LanguageManager.CurrentCultureCode;

            foreach (ComboBoxItem item in LanguageComboBox.Items)
            {
                if ((string)item.Tag == currentCulture)
                {
                    LanguageComboBox.SelectedItem = item;
                    return;
                }
            }

            LanguageComboBox.SelectedIndex = 0;
        }

        // Applies the selected language and reloads the page so every
        // x:Static text is read again from the new .resx
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)LanguageComboBox.SelectedItem;
            string selectedCulture = (string)selectedItem.Tag;

            if (selectedCulture != LanguageManager.CurrentCultureCode)
            {
                LanguageManager.ApplyCulture(selectedCulture);
                NavigationService.Navigate(new Settings());
            }
        }

        // Returns to the start menu. A new instance is created so its texts
        // are reloaded in the current language.
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartMenu());
        }

        // Hovering a button moves the selection to it
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Focus();
        }
    }
}