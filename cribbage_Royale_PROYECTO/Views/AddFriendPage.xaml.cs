using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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
    public partial class AddFriendPage : Page
    {
        private const string SearchTextPattern = @"^[A-Za-z0-9_]{1,16}$";

        private readonly FriendListClient _friendListClient;
        private readonly FriendRequestsClient _friendRequestsClient;

        private bool _operationInProgress;
        private string _lastSearchText = string.Empty;

        public AddFriendPage()
        {
            InitializeComponent();

            App application = (App)Application.Current;
            _friendListClient = application.FriendListClient;
            _friendRequestsClient = application.FriendRequestsClient;

            searchTextBox.Focus();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_operationInProgress)
            {
                await StartSearchAsync(searchTextBox.Text.Trim());
            }
        }

        private async void SendRequestButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerSearchItem item = GetItem(sender);
            string message = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Dialog_ConfirmSendRequestMessageFormat,
                item.DisplayName);
            bool confirmed = MessageDialog.ShowConfirmation(Window.GetWindow(this), message);

            if (confirmed && !_operationInProgress)
            {
                await SendFriendRequestAsync(item);
            }
        }

        private void ViewProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PublicProfilePage(GetItem(sender).AccountId));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FriendsPage());
        }

        private async Task StartSearchAsync(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                MessageDialog.ShowWarning(Window.GetWindow(this), Lang.AddFriend_SearchRequiredMessage);
            }
            else if (!Regex.IsMatch(searchText, SearchTextPattern))
            {
                MessageDialog.ShowWarning(Window.GetWindow(this), Lang.AddFriend_InvalidSearchMessage);
            }
            else
            {
                await SearchPlayersAsync(searchText);
            }
        }

        private async Task SearchPlayersAsync(string searchText)
        {
            _operationInProgress = true;
            _lastSearchText = searchText;
            PlayerSearchResult result = await _friendListClient.FindPlayersAsync(
                CurrentSession.SessionToken,
                searchText);
            _operationInProgress = false;

            ProcessSearchResult(result);
        }

        private void ProcessSearchResult(PlayerSearchResult result)
        {
            if (result.Succeeded)
            {
                ShowPlayers(result.Players);
            }
            else if (result.Reason == ServiceFailureReason.RequestRejected)
            {
                MessageDialog.ShowWarning(Window.GetWindow(this), Lang.AddFriend_InvalidSearchMessage);
            }
            else
            {
                MessageDialog.ShowError(Window.GetWindow(this), Lang.AddFriend_SearchErrorMessage);
            }
        }

        private async Task SendFriendRequestAsync(PlayerSearchItem item)
        {
            _operationInProgress = true;
            FriendRequestResult result = await _friendRequestsClient.SendFriendRequestAsync(
                CurrentSession.SessionToken,
                item.AccountId);
            _operationInProgress = false;

            await ProcessSendResultAsync(result);
        }

        private async Task ProcessSendResultAsync(FriendRequestResult result)
        {
            if (result.Succeeded)
            {
                MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.Dialog_FriendRequestSentMessage);
                await SearchPlayersAsync(_lastSearchText);
            }
            else if (result.Reason == ServiceFailureReason.RequestRejected)
            {
                MessageDialog.ShowWarning(
                    Window.GetWindow(this),
                    FriendRequestMessages.ResolveRejection(result.Failure.Value));
            }
            else
            {
                MessageDialog.ShowError(Window.GetWindow(this), Lang.AddFriend_RequestErrorMessage);
            }
        }

        private void ShowPlayers(PlayerSummary[] players)
        {
            List<PlayerSearchItem> items = players.Select(player => BuildItem(player)).ToList();

            resultsListBox.ItemsSource = items;
            noResultsTextBlock.Visibility = ResolveNoResultsVisibility(items.Count);
        }

        private PlayerSearchItem BuildItem(PlayerSummary player)
        {
            PlayerSearchItem item = new PlayerSearchItem();
            item.AccountId = player.Account.AccountId;
            item.DisplayName = player.Account.DisplayName;
            item.UserName = player.Account.UserName;
            item.RelationshipText = ResolveRelationshipText(player.Relationship);
            item.SendRequestVisibility = ResolveSendRequestVisibility(player.Relationship);

            return item;
        }

        private string ResolveRelationshipText(RelationshipKind relationship)
        {
            string relationshipText;

            switch (relationship)
            {
                case RelationshipKind.Friend:
                    relationshipText = Lang.AddFriend_FriendStatus;
                    break;
                case RelationshipKind.RequestSent:
                    relationshipText = Lang.AddFriend_RequestSentStatus;
                    break;
                case RelationshipKind.RequestReceived:
                    relationshipText = Lang.AddFriend_RequestReceivedStatus;
                    break;
                default:
                    relationshipText = string.Empty;
                    break;
            }

            return relationshipText;
        }

        private Visibility ResolveSendRequestVisibility(RelationshipKind relationship)
        {
            Visibility visibility = Visibility.Collapsed;

            if (relationship == RelationshipKind.None)
            {
                visibility = Visibility.Visible;
            }

            return visibility;
        }

        private Visibility ResolveNoResultsVisibility(int resultCount)
        {
            Visibility visibility = Visibility.Collapsed;

            if (resultCount == 0)
            {
                visibility = Visibility.Visible;
            }

            return visibility;
        }

        private PlayerSearchItem GetItem(object sender)
        {
            return (PlayerSearchItem)((FrameworkElement)sender).DataContext;
        }
    }
}