using System;
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
    public partial class FriendRequestsPage : Page
    {
        private readonly FriendRequestsClient _friendRequestsClient;
        private readonly GameClientCallback _sessionCallback;

        public FriendRequestsPage()
        {
            InitializeComponent();

            App application = (App)Application.Current;
            _friendRequestsClient = application.FriendRequestsClient;
            _sessionCallback = application.SessionCallback;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _sessionCallback.SocialEventReceived += SessionCallback_SocialEventReceived;
            await LoadRequestsAsync();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _sessionCallback.SocialEventReceived -= SessionCallback_SocialEventReceived;
        }

        private async void SessionCallback_SocialEventReceived(object sender, SocialEventReceivedEventArgs e)
        {
            if (e.EventType == SocialEventType.FriendRequestReceived)
            {
                await LoadRequestsAsync();
            }
        }

        private async void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            await RespondAsync(GetItem(sender), true);
        }

        private async void DeclineButton_Click(object sender, RoutedEventArgs e)
        {
            await RespondAsync(GetItem(sender), false);
        }

        private async void CancelRequestButton_Click(object sender, RoutedEventArgs e)
        {
            FriendRequestItem item = GetItem(sender);

            if (Confirm(Lang.Dialog_ConfirmCancelRequestMessageFormat, item.DisplayName))
            {
                FriendRequestResult result = await RunWithListDisabledAsync(
                    () => _friendRequestsClient.CancelFriendRequestAsync(
                        CurrentSession.SessionToken,
                        item.RequestId));

                await ProcessResultAsync(result, Lang.Dialog_RequestCancelledMessage);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FriendsPage());
        }

        private async Task LoadRequestsAsync()
        {
            PendingFriendRequestsResult result =
                await _friendRequestsClient.GetPendingFriendRequestsAsync(CurrentSession.SessionToken);

            if (result.Succeeded)
            {
                ShowRequests(result.Requests);
            }
            else
            {
                MessageDialog.ShowError(Window.GetWindow(this), Lang.FriendRequests_LoadingErrorMessage);
            }
        }

        private async Task RespondAsync(FriendRequestItem item, bool accepted)
        {
            if (Confirm(ResolveConfirmationFormat(accepted), item.DisplayName))
            {
                FriendRequestResult result = await RunWithListDisabledAsync(
                    () => _friendRequestsClient.RespondToFriendRequestAsync(
                        CurrentSession.SessionToken,
                        item.RequestId,
                        accepted));

                await ProcessResultAsync(result, FormatMessage(ResolveResponseFormat(accepted), item.DisplayName));
            }
        }

        private async Task<FriendRequestResult> RunWithListDisabledAsync(Func<Task<FriendRequestResult>> operation)
        {
            requestsListBox.IsEnabled = false;
            FriendRequestResult result = await operation();
            requestsListBox.IsEnabled = true;

            return result;
        }

        private async Task ProcessResultAsync(FriendRequestResult result, string successMessage)
        {
            if (result.Succeeded)
            {
                MessageDialog.ShowSuccess(Window.GetWindow(this), successMessage);
            }
            else if (result.Reason == ServiceFailureReason.RequestRejected)
            {
                MessageDialog.ShowWarning(
                    Window.GetWindow(this),
                    FriendRequestMessages.ResolveRejection(result.Failure.Value));
            }
            else
            {
                MessageDialog.ShowError(Window.GetWindow(this), Lang.FriendRequests_ProcessingErrorMessage);
            }

            await LoadRequestsAsync();
        }

        private void ShowRequests(FriendRequest[] requests)
        {
            List<FriendRequestItem> items = requests
                .OrderBy(request => request.Direction)
                .Select(request => BuildItem(request))
                .ToList();

            requestsListBox.ItemsSource = items;
            emptyListTextBlock.Visibility = ResolveVisibility(items.Count == 0);
        }

        private FriendRequestItem BuildItem(FriendRequest request)
        {
            bool isReceived = request.Direction == RequestDirection.Received;

            FriendRequestItem item = new FriendRequestItem();
            item.RequestId = request.RequestId;
            item.DisplayName = request.Counterpart.DisplayName;
            item.StatusText = FormatMessage(ResolveStatusFormat(isReceived), item.DisplayName);
            item.ResponseVisibility = ResolveVisibility(isReceived);
            item.CancelVisibility = ResolveVisibility(!isReceived);

            return item;
        }

        private string ResolveConfirmationFormat(bool accepted)
        {
            string confirmationFormat = Lang.Dialog_ConfirmRejectRequestMessageFormat;

            if (accepted)
            {
                confirmationFormat = Lang.Dialog_ConfirmAcceptRequestMessageFormat;
            }

            return confirmationFormat;
        }

        private string ResolveResponseFormat(bool accepted)
        {
            string responseFormat = Lang.FriendRequests_DeclinedMessage;

            if (accepted)
            {
                responseFormat = Lang.FriendRequests_AcceptedMessage;
            }

            return responseFormat;
        }

        private string ResolveStatusFormat(bool isReceived)
        {
            string statusFormat = Lang.FriendRequests_SentFormat;

            if (isReceived)
            {
                statusFormat = Lang.FriendRequests_ReceivedFormat;
            }

            return statusFormat;
        }

        private Visibility ResolveVisibility(bool isVisible)
        {
            Visibility visibility = Visibility.Collapsed;

            if (isVisible)
            {
                visibility = Visibility.Visible;
            }

            return visibility;
        }

        private bool Confirm(string messageFormat, string displayName)
        {
            return MessageDialog.ShowConfirmation(Window.GetWindow(this), FormatMessage(messageFormat, displayName));
        }

        private string FormatMessage(string messageFormat, string displayName)
        {
            return string.Format(CultureInfo.CurrentCulture, messageFormat, displayName);
        }

        private FriendRequestItem GetItem(object sender)
        {
            return (FriendRequestItem)((FrameworkElement)sender).DataContext;
        }
    }
}