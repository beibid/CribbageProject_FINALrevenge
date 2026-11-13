using System.Collections.Generic;
using System.Globalization;
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
    public partial class GamePage : Page
    {
        private const int WinningScore = 61;
        private const int CardsToDiscard = 2;
        private const int HandSize = 6;

        private readonly MatchStart _matchStart;
        private readonly GameClientCallback _sessionCallback;
        private readonly MatchPlayClient _matchPlayClient;

        private List<PlayingCardItem> _playerHand;
        private List<Card> _ownCards;
        private int _stateVersion;

        public GamePage(MatchStart matchStart)
        {
            InitializeComponent();
            _matchStart = matchStart;
            _playerHand = new List<PlayingCardItem>();
            _ownCards = new List<Card>();
            _sessionCallback = ((App)Application.Current).SessionCallback;
            _matchPlayClient = ((App)Application.Current).MatchPlayClient;
            _sessionCallback.MatchStateReceived += SessionCallback_MatchStateReceived;
            Unloaded += GamePage_Unloaded;
            ShowMatchHeader();
            ShowEmptyTable();
        }

        private void SessionCallback_MatchStateReceived(object sender, MatchStateChangedEventArgs e)
        {
            if (e.State.MatchId == _matchStart.MatchId)
            {
                ShowState(e.State);
            }
        }

        private void GamePage_Unloaded(object sender, RoutedEventArgs e)
        {
            _sessionCallback.MatchStateReceived -= SessionCallback_MatchStateReceived;
            Unloaded -= GamePage_Unloaded;
        }

        private void ShowMatchHeader()
        {
            opponentNameTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Game_OpponentFormat,
                ResolveOpponentName());

            hostBadgeBorder.Visibility = ResolveVisibility(_matchStart.IsHost);
            cribOwnerTextBlock.Text = ResolveCribOwner();

            goalTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Game_GoalFormat,
                WinningScore);

            timeRemainingTextBlock.Text = string.Empty;
            lastGainTextBlock.Text = string.Empty;
        }

        private void ShowEmptyTable()
        {
            opponentHandItemsControl.ItemsSource = BuildFaceDownHand(HandSize);
            playerHandItemsControl.ItemsSource = _playerHand;
            playedCardsItemsControl.ItemsSource = new List<PlayingCardItem>();
            opponentTrackItemsControl.ItemsSource = BuildTrack(0);
            playerTrackItemsControl.ItemsSource = BuildTrack(0);
            ShowCutCard(null);
            ShowScore(0, 0);
            ShowRunningCount(0);
            ShowPhase(MatchPhase.Discarding);
        }

        private void ShowState(MatchState state)
        {
            _stateVersion = state.StateVersion;
            _ownCards = ResolveOwnCards(state);
            _playerHand = BuildCards(_ownCards);
            playerHandItemsControl.ItemsSource = _playerHand;
            opponentHandItemsControl.ItemsSource = BuildFaceDownHand(_playerHand.Count);
            playedCardsItemsControl.ItemsSource = BuildCards(state.PlayedCards);
            opponentTrackItemsControl.ItemsSource = BuildTrack(state.RivalScore);
            playerTrackItemsControl.ItemsSource = BuildTrack(state.OwnScore);
            ShowCutCard(state.CutCard);
            ShowScore(state.OwnScore, state.RivalScore);
            ShowRunningCount(state.PeggingCount);
            ShowPhase(state.Phase);
            discardButton.IsEnabled = _playerHand.Count == HandSize;
        }

        private void ShowPhase(MatchPhase phase)
        {
            bool isDiscardPhase = phase == MatchPhase.Discarding;

            discardPanel.Visibility = ResolveVisibility(isDiscardPhase);
            playPanel.Visibility = ResolveVisibility(!isDiscardPhase);
            discardButton.Visibility = ResolveVisibility(isDiscardPhase);

            if (isDiscardPhase)
            {
                ShowDiscardTexts();
            }
            else
            {
                ShowPlayTexts();
            }
        }

        private void ShowDiscardTexts()
        {
            phaseTitleTextBlock.Text = Lang.Game_PhaseDiscardTitle;
            phaseFirstLineTextBlock.Text = Lang.Game_PhaseDiscardText1;
            phaseSecondLineTextBlock.Text = Lang.Game_PhaseDiscardText2;
        }

        private void ShowPlayTexts()
        {
            phaseTitleTextBlock.Text = Lang.Game_PhasePlayTitle;
            phaseFirstLineTextBlock.Text = Lang.Game_PhasePlayText1;
            phaseSecondLineTextBlock.Text = Lang.Game_PhasePlayText2;
        }

        private void ShowCutCard(Card card)
        {
            PlayingCardItem item = new PlayingCardItem();

            if (card != null)
            {
                item = BuildCard(card);
            }

            starterCardContentControl.Content = item;
        }

        private void ShowScore(int ownScore, int rivalScore)
        {
            scoreTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Game_ScoreFormat,
                ownScore,
                rivalScore,
                WinningScore);
        }

        private void ShowRunningCount(int peggingCount)
        {
            runningCountTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Game_RunningCountFormat,
                peggingCount);
        }

        private string ResolveOpponentName()
        {
            string name = Lang.Game_OpponentUnknown;

            if (!string.IsNullOrWhiteSpace(_matchStart.Opponent.DisplayName))
            {
                name = _matchStart.Opponent.DisplayName;
            }

            return name;
        }

        private string ResolveCribOwner()
        {
            string owner = Lang.Game_CribOwnerOpponent;

            if (_matchStart.IsHost)
            {
                owner = Lang.Game_CribOwnerYou;
            }

            return owner;
        }

        private List<Card> GetSelectedCards()
        {
            List<Card> selected = new List<Card>();

            for (int index = 0; index < _playerHand.Count; index++)
            {
                if (_playerHand[index].IsSelected)
                {
                    selected.Add(_ownCards[index]);
                }
            }

            return selected;
        }

        private CribDiscard BuildDiscard(List<Card> cards)
        {
            CribDiscard discard = new CribDiscard();
            discard.MatchId = _matchStart.MatchId;
            discard.Cards = cards;
            discard.ExpectedStateVersion = _stateVersion;

            return discard;
        }

        private async Task SendDiscardAsync(List<Card> cards)
        {
            discardButton.IsEnabled = false;

            DiscardResult result = await _matchPlayClient.DiscardAsync(
                CurrentSession.SessionToken,
                BuildDiscard(cards));

            if (!result.Succeeded)
            {
                discardButton.IsEnabled = true;
                matchMessagePanel.ShowError(Lang.Application_UnexpectedErrorMessage);
            }
        }

        private static List<Card> ResolveOwnCards(MatchState state)
        {
            List<Card> cards = new List<Card>();

            if (state.OwnHand != null)
            {
                cards.AddRange(state.OwnHand);
            }

            return cards;
        }

        private static Visibility ResolveVisibility(bool isVisible)
        {
            Visibility visibility = Visibility.Collapsed;

            if (isVisible)
            {
                visibility = Visibility.Visible;
            }

            return visibility;
        }

        private static List<PlayingCardItem> BuildCards(List<Card> cards)
        {
            List<PlayingCardItem> items = new List<PlayingCardItem>();

            if (cards != null)
            {
                foreach (Card card in cards)
                {
                    items.Add(BuildCard(card));
                }
            }

            return items;
        }

        private static PlayingCardItem BuildCard(Card card)
        {
            PlayingCardItem item = new PlayingCardItem();
            item.Rank = ResolveRank(card.Rank);
            item.Suit = ResolveSuit(card.Suit);
            item.IsRedSuit = IsRedSuit(card.Suit);

            return item;
        }

        private static List<PlayingCardItem> BuildFaceDownHand(int count)
        {
            List<PlayingCardItem> cards = new List<PlayingCardItem>();

            for (int index = 0; index < count; index++)
            {
                cards.Add(new PlayingCardItem());
            }

            return cards;
        }

        private static List<BoardHoleItem> BuildTrack(int score)
        {
            List<BoardHoleItem> holes = new List<BoardHoleItem>();

            for (int position = 1; position <= WinningScore; position++)
            {
                BoardHoleItem hole = new BoardHoleItem();
                hole.Position = position;
                hole.HasPeg = position == score;
                holes.Add(hole);
            }

            return holes;
        }

        private static string ResolveRank(CardRank rank)
        {
            string text;

            switch (rank)
            {
                case CardRank.Ace:
                    text = "A";
                    break;
                case CardRank.Jack:
                    text = "J";
                    break;
                case CardRank.Queen:
                    text = "Q";
                    break;
                case CardRank.King:
                    text = "K";
                    break;
                default:
                    text = ((int)rank).ToString(CultureInfo.InvariantCulture);
                    break;
            }

            return text;
        }

        private static string ResolveSuit(CardSuit suit)
        {
            string symbol;

            switch (suit)
            {
                case CardSuit.Hearts:
                    symbol = "\u2665";
                    break;
                case CardSuit.Diamonds:
                    symbol = "\u2666";
                    break;
                case CardSuit.Clubs:
                    symbol = "\u2663";
                    break;
                case CardSuit.Spades:
                    symbol = "\u2660";
                    break;
                default:
                    symbol = string.Empty;
                    break;
            }

            return symbol;
        }

        private static bool IsRedSuit(CardSuit suit)
        {
            return suit == CardSuit.Hearts || suit == CardSuit.Diamonds;
        }

        private async void DiscardButton_Click(object sender, RoutedEventArgs e)
        {
            List<Card> selected = GetSelectedCards();

            if (selected.Count == CardsToDiscard)
            {
                matchMessagePanel.HidePanel();
                await SendDiscardAsync(selected);
            }
            else
            {
                matchMessagePanel.ShowError(Lang.Game_SelectTwoCardsMessage);
            }
        }

        private void RulesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Tutorial());
        }

        private void LeaveMatchButton_Click(object sender, RoutedEventArgs e)
        {
            bool isConfirmed = MessageDialog.ShowConfirmation(
                Window.GetWindow(this),
                Lang.Game_LeaveConfirmationMessage);

            if (isConfirmed)
            {
                NavigationService.Navigate(new Lobby());
            }
        }
    }
}