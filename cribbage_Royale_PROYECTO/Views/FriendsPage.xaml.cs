using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_DataAccess.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class FriendsPage : Page
    {
        public FriendsPage()
        {
            InitializeComponent();
            ShowFriends();
        }

        private void ShowFriends()
        {
            List<PlayerItem> friends = new List<PlayerItem>
            {
                new PlayerItem { UserName = "GamerPro99", Status = Lang.Common_OnlineStatus },
                new PlayerItem { UserName = "CribMaster", Status = Lang.Common_OfflineStatus }
            };

            friendsListBox.ItemsSource = friends;
            emptyListTextBlock.Visibility = friends.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            friendRequestsButton.Content = string.Format(CultureInfo.CurrentCulture,
                Lang.Friends_RequestsTabHeaderFormat, 2);
        }

        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddFriendPage());
        }

        private void FriendRequestsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FriendRequestsPage());
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Lobby());
        }
    }
}