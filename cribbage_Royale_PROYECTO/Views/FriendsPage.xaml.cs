using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CribbageRoyale.Contracts.DataContracts;
using cribbage_Royale_PROYECTO.Localization;
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
        private readonly FriendRequestsClient _friendRequestsClient;
        private readonly GameClientCallback _sessionCallback;

        private List<FriendSummary> _friends = new List<FriendSummary>();

        public FriendsPage()
        {
            InitializeComponent();

            App application = (App)Application.Current;
            _friendListClient = application.FriendListClient;
            _friendRequestsClient = application.FriendRequestsClient;
            _sessionCallback = application.SessionCallback;

            ShowFriendRequestsButton(NoPendingRequests);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _sessionCallback.PresenceReceived += SessionCallback_PresenceReceived;
            _sessionCallback.SocialEventReceived += SessionCallback_SocialEventReceived;
            await LoadFriendsAsync();
            await LoadPendingRequestCountAsync();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _sessionCallback.PresenceReceived -= SessionCallback_PresenceReceived;
            _sessionCallback.SocialEventReceived -= SessionCallback_SocialEventReceived;
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

        private async Task LoadPendingRequestCountAsync()
        {
            PendingFriendRequestsResult result =
                await _friendRequestsClient.GetPendingFriendRequestsAsync(CurrentSession.SessionToken);

            if (result.Succeeded)
            {
                ShowFriendRequestsButton(CountReceivedRequests(result.Requests));
            }
        }

        private int CountReceivedRequests(FriendRequest[] requests)
        {
            return requests.Count(request => request.Direction == RequestDirection.Received);
        }

        private async void SessionCallback_SocialEventReceived(object sender, SocialEventReceivedEventArgs e)
        {
            if (e.EventType == SocialEventType.FriendRequestAccepted)
            {
                await LoadFriendsAsync();
            }
            else if (e.EventType == SocialEventType.FriendRequestReceived)
            {
                await LoadPendingRequestCountAsync();
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

        private void ShowFriendRequestsButton(int pendingRequestCount)
        {
            friendRequestsButton.Content = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Friends_RequestsTabHeaderFormat,
                pendingRequestCount);
        }
        private async void RemoveFriendButton_Click(object sender, RoutedEventArgs e)
        {
            FriendItem item = (FriendItem)((FrameworkElement)sender).DataContext;
            string confirmation = FormatMessage(Lang.Friends_RemoveConfirmationMessage, item.DisplayName);

            if (MessageDialog.ShowConfirmation(Window.GetWindow(this), confirmation))
            {
                friendsListBox.IsEnabled = false;
                FriendshipOperationResult result =
                    await _friendListClient.RemoveFriendAsync(CurrentSession.SessionToken, item.AccountId);
                friendsListBox.IsEnabled = true;

                await ProcessRemovalResultAsync(result, item.DisplayName);
            }
        }

        private async Task ProcessRemovalResultAsync(FriendshipOperationResult result, string displayName)
        {
            if (result.Succeeded)
            {
                MessageDialog.ShowSuccess(
                    Window.GetWindow(this),
                    FormatMessage(Lang.Friends_FriendRemovedMessage, displayName));
            }
            else if (result.Reason == ServiceFailureReason.RequestRejected)
            {
                MessageDialog.ShowWarning(
                    Window.GetWindow(this),
                    FriendRequestMessages.ResolveRejection(result.Failure.Value));
            }
            else
            {
                MessageDialog.ShowError(Window.GetWindow(this), Lang.Friends_RemoveErrorMessage);
            }

            await LoadFriendsAsync();
        }

        private string FormatMessage(string messageFormat, string displayName)
        {
            return string.Format(CultureInfo.CurrentCulture, messageFormat, displayName);
        }
        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddFriendPage());
        }

        private void FriendRequestsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FriendRequestsPage());
        }

        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            FriendItem friend = (FriendItem)((Button)sender).DataContext;

            NavigationService.Navigate(new ChatPage(friend.AccountId, friend.DisplayName));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Lobby());
        }
    }
}