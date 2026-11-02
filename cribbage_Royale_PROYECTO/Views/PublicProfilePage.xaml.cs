using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CribbageRoyale.Contracts.FaultContracts;
using cribbage_Royale_DataAccess.DataAccess;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_DataAccess.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;
using cribbage_Royale_PROYECTO.Views.Dialogs;
using cribbage_Royale_PROYECTO.Session;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class PublicProfilePage : Page
    {
        private readonly PlayerProfileRepository _profileRepository;
        private readonly FriendshipRepository _friendshipRepository;
        private readonly BlockRepository _blockRepository;
        private readonly UserRepository _userRepository;
        private readonly FriendRequestsClient _friendRequestsClient;
        private readonly int _viewerAccountId;
        private readonly int _targetAccountId;

        public PublicProfilePage(int targetAccountId)
        {
            InitializeComponent();
            _profileRepository = new PlayerProfileRepository();
            _friendshipRepository = new FriendshipRepository();
            _blockRepository = new BlockRepository();
            _userRepository = new UserRepository();
            _friendRequestsClient = ((App)Application.Current).FriendRequestsClient;
            _viewerAccountId = _userRepository.GetUserId(CurrentSession.Username);
            _targetAccountId = targetAccountId;
            LoadProfile();
        }

        private void LoadProfile()
        {
            messagePanel.HidePanel();
            try
            {
                PublicProfileSummary profile = _profileRepository.GetPublicProfile(_viewerAccountId, _targetAccountId);
                displayNameTextBlock.Text = profile.DisplayName;
                usernameTextBlock.Text = profile.Username;
                skillLevelTextBlock.Text = string.Format(Lang.PublicProfile_SkillLevelFormat, profile.SkillLevel);
                matchesPlayedTextBlock.Text = string.Format(Lang.PublicProfile_MatchesPlayedFormat, profile.MatchesPlayed, profile.Wins);
                winPercentageTextBlock.Text = string.Format(Lang.PublicProfile_WinPercentageFormat, profile.WinPercentage);
                registrationDateTextBlock.Text = string.Format(Lang.PublicProfile_RegistrationDateFormat, profile.RegistrationDate);
                UpdateActionButtons(profile.RelationshipStatus);
            }
            catch (Exception dataAccessException)
            {
                MessageDialog.ShowError(Window.GetWindow(this), Lang.Dialog_DatabaseAccessErrorMessage);
            }
        }

        private void UpdateActionButtons(string relationshipStatus)
        {
            addFriendButton.Visibility = Visibility.Collapsed;
            cancelRequestButton.Visibility = Visibility.Collapsed;
            acceptRequestButton.Visibility = Visibility.Collapsed;
            rejectRequestButton.Visibility = Visibility.Collapsed;
            removeFriendButton.Visibility = Visibility.Collapsed;
            unblockButton.Visibility = Visibility.Collapsed;
            blockButton.Visibility = Visibility.Visible;
            if (relationshipStatus == RelationshipStatus.Friend)
            {
                removeFriendButton.Visibility = Visibility.Visible;
            }
            else if (relationshipStatus == RelationshipStatus.RequestSent)
            {
                cancelRequestButton.Visibility = Visibility.Visible;
            }
            else if (relationshipStatus == RelationshipStatus.RequestReceived)
            {
                acceptRequestButton.Visibility = Visibility.Visible;
                rejectRequestButton.Visibility = Visibility.Visible;
            }
            else if (relationshipStatus == RelationshipStatus.Blocked)
            {
                unblockButton.Visibility = Visibility.Visible;
                blockButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                addFriendButton.Visibility = Visibility.Visible;
            }
        }

        private async void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            bool confirmed = MessageDialog.ShowConfirmation(
                Window.GetWindow(this),
                string.Format(Lang.Dialog_ConfirmSendRequestMessageFormat, displayNameTextBlock.Text));

            if (confirmed)
            {
                await SendFriendRequestAsync();
            }
        }

        private async Task SendFriendRequestAsync()
        {
            FriendRequestResult result = await _friendRequestsClient.SendFriendRequestAsync(
                CurrentSession.SessionToken,
                _targetAccountId);

            ProcessSendResult(result);
        }

        private void ProcessSendResult(FriendRequestResult result)
        {
            if (result.Succeeded)
            {
                MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.Dialog_FriendRequestSentMessage);
                LoadProfile();
            }
            else if (result.Reason == ServiceFailureReason.RequestRejected)
            {
                MessageDialog.ShowWarning(Window.GetWindow(this), ResolveRejectionMessage(result.Failure.Value));
            }
            else
            {
                MessageDialog.ShowError(Window.GetWindow(this), Lang.AddFriend_RequestErrorMessage);
            }
        }

        private string ResolveRejectionMessage(FriendshipFailure failure)
        {
            string message;

            switch (failure)
            {
                case FriendshipFailure.CannotTargetSelf:
                    message = Lang.AddFriend_CannotAddYourselfMessage;
                    break;
                case FriendshipFailure.FriendshipAlreadyExists:
                    message = Lang.AddFriend_AlreadyFriendsMessage;
                    break;
                case FriendshipFailure.RequestAlreadyPending:
                    message = Lang.AddFriend_RequestAlreadySentMessage;
                    break;
                default:
                    message = Lang.AddFriend_PlayerUnavailableMessage;
                    break;
            }

            return message;
        }

        private void CancelRequestButton_Click(object sender, RoutedEventArgs e)
        {
            bool confirmed = MessageDialog.ShowConfirmation(Window.GetWindow(this), string.Format(Lang.Dialog_ConfirmCancelRequestMessageFormat, displayNameTextBlock.Text));
            if (confirmed)
            {
                try
                {
                    _friendshipRepository.CancelFriendRequest(_viewerAccountId, _targetAccountId);
                    MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.Dialog_RequestCancelledMessage);
                    LoadProfile();
                }
                catch (Exception dataAccessException)
                {
                    MessageDialog.ShowError(Window.GetWindow(this), Lang.Dialog_DatabaseAccessErrorMessage);
                }
            }
        }

        private void AcceptRequestButton_Click(object sender, RoutedEventArgs e)
        {
            bool confirmed = MessageDialog.ShowConfirmation(Window.GetWindow(this), string.Format(Lang.Dialog_ConfirmAcceptRequestMessageFormat, displayNameTextBlock.Text));
            if (confirmed)
            {
                RespondToRequest(true);
            }
        }

        private void RejectRequestButton_Click(object sender, RoutedEventArgs e)
        {
            bool confirmed = MessageDialog.ShowConfirmation(Window.GetWindow(this), string.Format(Lang.Dialog_ConfirmRejectRequestMessageFormat, displayNameTextBlock.Text));
            if (confirmed)
            {
                RespondToRequest(false);
            }
        }

        private void RespondToRequest(bool isAccepted)
        {
            try
            {
                _friendshipRepository.RespondToFriendRequest(_viewerAccountId, _targetAccountId, isAccepted);
                if (isAccepted)
                {
                    MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.Dialog_NowFriendsMessage);
                }
                else
                {
                    MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.Dialog_RequestRejectedMessage);
                }
                LoadProfile();
            }
            catch (Exception dataAccessException)
            {
                MessageDialog.ShowError(Window.GetWindow(this), Lang.Dialog_DatabaseAccessErrorMessage);
            }
        }

        private void RemoveFriendButton_Click(object sender, RoutedEventArgs e)
        {
            bool confirmed = MessageDialog.ShowConfirmation(Window.GetWindow(this), string.Format(Lang.Dialog_ConfirmRemoveFriendMessageFormat, displayNameTextBlock.Text));
            if (confirmed)
            {
                try
                {
                    _friendshipRepository.RemoveFriend(_viewerAccountId, _targetAccountId);
                    MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.Dialog_FriendRemovedMessage);
                    LoadProfile();
                }
                catch (Exception dataAccessException)
                {
                    MessageDialog.ShowError(Window.GetWindow(this), Lang.Dialog_DatabaseAccessErrorMessage);
                }
            }
        }

        private void BlockButton_Click(object sender, RoutedEventArgs e)
        {
            bool confirmed = MessageDialog.ShowConfirmation(Window.GetWindow(this), string.Format(Lang.Dialog_ConfirmBlockMessageFormat, displayNameTextBlock.Text));
            if (confirmed)
            {
                try
                {
                    _blockRepository.BlockPlayer(_viewerAccountId, _targetAccountId);
                    MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.Dialog_PlayerBlockedMessage);
                    LoadProfile();
                }
                catch (Exception dataAccessException)
                {
                    MessageDialog.ShowError(Window.GetWindow(this), Lang.Dialog_DatabaseAccessErrorMessage);
                }
            }
        }

        private void UnblockButton_Click(object sender, RoutedEventArgs e)
        {
            bool confirmed = MessageDialog.ShowConfirmation(Window.GetWindow(this), string.Format(Lang.Dialog_ConfirmUnblockMessageFormat, displayNameTextBlock.Text));
            if (confirmed)
            {
                try
                {
                    _blockRepository.UnblockPlayer(_viewerAccountId, _targetAccountId);
                    MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.Dialog_UnblockSuccessMessage);
                    LoadProfile();
                }
                catch (Exception dataAccessException)
                {
                    MessageDialog.ShowError(Window.GetWindow(this), Lang.Dialog_DatabaseAccessErrorMessage);
                }
            }
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReportPlayerPage(_targetAccountId, displayNameTextBlock.Text, null));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}