using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_DataAccess.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class AddFriendPage : Page
    {
        public AddFriendPage()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string search = searchTextBox.Text.Trim();

            if (string.IsNullOrEmpty(search))
            {
                MessageDialog.ShowWarning(Window.GetWindow(this), Lang.AddFriend_SearchRequiredMessage);
                return;
            }

            List<PlayerItem> results = new List<PlayerItem>
            {
                new PlayerItem { UserName = "PlayerOne" },
                new PlayerItem { UserName = "PlayerTwo" }
            };

            resultsListBox.ItemsSource = results;
            noResultsTextBlock.Visibility = results.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FriendsPage());
        }
    }
}