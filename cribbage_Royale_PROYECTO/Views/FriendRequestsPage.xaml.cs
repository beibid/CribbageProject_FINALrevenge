using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class FriendRequestsPage : Page
    {
        public FriendRequestsPage()
        {
            InitializeComponent();
            ShowRequests();
        }

        private void ShowRequests()
        {
            List<PlayerItem> requests = new List<PlayerItem>
            {
                new PlayerItem
                {
                    UserName = "Noob99",
                    Status = string.Format(CultureInfo.CurrentCulture, Lang.FriendRequests_ReceivedFormat, "Noob99")
                },
                new PlayerItem
                {
                    UserName = "PlayerTwo",
                    Status = string.Format(CultureInfo.CurrentCulture, Lang.FriendRequests_ReceivedFormat, "PlayerTwo")
                }
            };

            requestsListBox.ItemsSource = requests;
            emptyListTextBlock.Visibility = requests.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FriendsPage());
        }
    }
}