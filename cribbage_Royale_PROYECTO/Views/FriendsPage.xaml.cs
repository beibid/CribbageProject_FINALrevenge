using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CribbageRoyale.Contracts.DataContracts;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;
using cribbage_Royale_PROYECTO.Session;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class FriendsPage : Page
    {
        private const int NoPendingRequests = 0;

        private readonly FriendListClient _friendListClient;
        private readonly GameClientCallback _sessionCallback;

        private List<FriendSummary> _friends = new List<FriendSummary>();

        public FriendsPage()
        {
            InitializeComponent();

            App application = (App)Application.Current;
            _friendListClient = application.FriendListClient;
            _sessionCallback = application.SessionCallback;

            ShowFriendRequestsButton();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _sessionCallback.PresenceReceived += SessionCallback_PresenceReceived;
            await LoadFriendsAsync();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _sessionCallback.PresenceReceived -= SessionCallback_PresenceReceived;
        }

        private async Task LoadFriendsAsync()
        {
            FriendListResult result = await _friendListClient.GetFriendsAsync(CurrentSession.SessionToken);

            if (result.Succeeded)
            {
                _friends = result.Friends.ToList();
                ShowFriends();
            }
            else
            {
                MessageDialog.ShowError(Window.GetWindow(this), Lang.Friends_LoadingErrorMessage);
            }
        }

        private void SessionCallback_PresenceReceived(object sender, PresenceChangedEventArgs e)
        {
            if (e.Presence.Scope == PresenceScope.Friend)
            {
                UpdatePresence(e.Presence);
            }
        }

        private void UpdatePresence(PresenceChange presence)
        {
            FriendSummary friend = _friends.FirstOrDefault(
                summary => summary.Account.AccountId == presence.Account.AccountId);

            if (friend != null)
            {
                friend.IsOnline = presence.IsConnected;
                ShowFriends();
            }
        }

        private void ShowFriends()
        {
            List<FriendItem> items = _friends.Select(friend => BuildItem(friend)).ToList();

            friendsListBox.ItemsSource = items;
            emptyListTextBlock.Visibility = ResolveEmptyListVisibility(items.Count);
        }

        private FriendItem BuildItem(FriendSummary friend)
        {
            FriendItem item = new FriendItem();
            item.AccountId = friend.Account.AccountId;
            item.DisplayName = friend.Account.DisplayName;
            item.StatusText = ResolveStatusText(friend.IsOnline);

            return item;
        }

        private string ResolveStatusText(bool isOnline)
        {
            string statusText = Lang.Common_OfflineStatus;

            if (isOnline)
            {
                statusText = Lang.Common_OnlineStatus;
            }

            return statusText;
        }

        private Visibility ResolveEmptyListVisibility(int friendCount)
        {
            Visibility visibility = Visibility.Collapsed;

            if (friendCount == 0)
            {
                visibility = Visibility.Visible;
            }

            return visibility;
        }

        private void ShowFriendRequestsButton()
        {
            friendRequestsButton.Content = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Friends_RequestsTabHeaderFormat,
                NoPendingRequests);
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