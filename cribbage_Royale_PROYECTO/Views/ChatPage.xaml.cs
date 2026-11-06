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
    public partial class ChatPage : Page
    {
        private const string TimeFormat = "HH:mm";
        private const int EmptyConversation = 0;

        private readonly FriendChatClient _friendChatClient;
        private readonly ChatMessageStore _chatMessageStore;
        private readonly GameClientCallback _sessionCallback;
        private readonly int _friendAccountId;

        public ChatPage(int friendAccountId, string friendDisplayName)
        {
            InitializeComponent();

            App application = (App)Application.Current;
            _friendChatClient = application.FriendChatClient;
            _chatMessageStore = application.ChatMessageStore;
            _sessionCallback = application.SessionCallback;
            _friendAccountId = friendAccountId;

            ShowTitle(friendDisplayName);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _sessionCallback.ChatMessageArrived += SessionCallback_ChatMessageArrived;
            ShowConversation();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _sessionCallback.ChatMessageArrived -= SessionCallback_ChatMessageArrived;
        }

        private void SessionCallback_ChatMessageArrived(object sender, ChatMessageReceivedEventArgs e)
        {
            if (e.Message.Sender.AccountId == _friendAccountId)
            {
                ShowConversation();
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            sendButton.IsEnabled = false;
            await SendMessageAsync();
            sendButton.IsEnabled = true;
        }

        private async Task SendMessageAsync()
        {
            ChatMessageResult result = await _friendChatClient.SendMessageAsync(
                CurrentSession.SessionToken,
                _friendAccountId,
                messageTextBox.Text);

            ShowSendOutcome(result);
        }

        private void ShowSendOutcome(ChatMessageResult result)
        {
            if (result.Succeeded)
            {
                _chatMessageStore.Add(_friendAccountId, result.Message);
                messageTextBox.Clear();
                ShowConversation();
            }
            else
            {
                MessageDialog.ShowError(Window.GetWindow(this), ResolveErrorMessage(result));
            }
        }

        private string ResolveErrorMessage(ChatMessageResult result)
        {
            string message = Lang.Chat_SendErrorMessage;

            if (result.Failure.HasValue)
            {
                message = ChatMessages.ResolveRejection(result.Failure.Value);
            }

            return message;
        }

        private void ShowConversation()
        {
            List<ChatMessage> messages = _chatMessageStore.GetConversation(_friendAccountId);
            List<ChatMessageItem> items = messages.Select(message => BuildItem(message)).ToList();

            messagesListBox.ItemsSource = items;
            emptyConversationTextBlock.Visibility = ResolveEmptyListVisibility(items.Count);
            ScrollToLastMessage(items);
        }

        private ChatMessageItem BuildItem(ChatMessage message)
        {
            ChatMessageItem item = new ChatMessageItem();
            item.SenderName = message.Sender.DisplayName;
            item.Text = message.Text;
            item.TimeText = message.SentAtUtc.ToLocalTime().ToString(TimeFormat, CultureInfo.CurrentCulture);

            return item;
        }

        private void ScrollToLastMessage(List<ChatMessageItem> items)
        {
            if (items.Count > EmptyConversation)
            {
                messagesListBox.ScrollIntoView(items[items.Count - 1]);
            }
        }

        private Visibility ResolveEmptyListVisibility(int messageCount)
        {
            Visibility visibility = Visibility.Collapsed;

            if (messageCount == EmptyConversation)
            {
                visibility = Visibility.Visible;
            }

            return visibility;
        }

        private void ShowTitle(string friendDisplayName)
        {
            titleTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Chat_TitleFormat,
                friendDisplayName);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FriendsPage());
        }
    }
}