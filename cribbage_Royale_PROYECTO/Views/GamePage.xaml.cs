using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class GamePage : Page
    {
        private const MatchPhase CurrentPhase = MatchPhase.Discard;
        private const bool PlayerIsDealer = true;

        private const int WinningScore = 61;
        private const int CardsToDiscard = 2;
        private const int HandSize = 6;

        private const int SamplePlayerScore = 42;
        private const int SampleOpponentScore = 38;
        private const int SamplePlayerGain = 8;
        private const int SampleOpponentGain = 8;
        private const int SampleRemainingSeconds = 25;
        private const int SampleRunningCount = 15;

        private readonly string _opponentName;
        private readonly List<PlayingCardItem> _playerHand;

        public GamePage(string opponentName = null)
        {
            InitializeComponent();

            _opponentName = string.IsNullOrWhiteSpace(opponentName)
                ? Lang.Game_OpponentUnknown
                : opponentName;

            _playerHand = BuildPlayerHand();

            ShowMatch();
            ShowPhase();
        }

        private void ShowMatch()
        {
            opponentNameTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Game_OpponentFormat,
                _opponentName);

            cribOwnerTextBlock.Text = PlayerIsDealer
                ? Lang.Game_CribOwnerYou
                : Lang.Game_CribOwnerOpponent;

            opponentHandItemsControl.ItemsSource = BuildFaceDownHand();
            playerHandItemsControl.ItemsSource = _playerHand;
            starterCardContentControl.Content = BuildStarterCard();
            playedCardsItemsControl.ItemsSource = BuildPlayedCards();

            opponentTrackItemsControl.ItemsSource =
                BuildTrack(SampleOpponentScore, SampleOpponentGain);
            playerTrackItemsControl.ItemsSource =
                BuildTrack(SamplePlayerScore, SamplePlayerGain);

            goalTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Game_GoalFormat,
                WinningScore);

            lastGainTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Game_LastGainFormat,
                SamplePlayerGain);

            scoreTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Game_ScoreFormat,
                SamplePlayerScore,
                SampleOpponentScore,
                WinningScore);

            timeRemainingTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Game_TimeRemainingFormat,
                SampleRemainingSeconds);

            runningCountTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Game_RunningCountFormat,
                SampleRunningCount);
        }

        private void ShowPhase()
        {
            bool isDiscardPhase = CurrentPhase == MatchPhase.Discard;

            discardPanel.Visibility = isDiscardPhase ? Visibility.Visible : Visibility.Collapsed;
            playPanel.Visibility = isDiscardPhase ? Visibility.Collapsed : Visibility.Visible;
            discardButton.Visibility = isDiscardPhase ? Visibility.Visible : Visibility.Collapsed;

            phaseTitleTextBlock.Text = isDiscardPhase
                ? Lang.Game_PhaseDiscardTitle
                : Lang.Game_PhasePlayTitle;

            phaseFirstLineTextBlock.Text = isDiscardPhase
                ? Lang.Game_PhaseDiscardText1
                : Lang.Game_PhasePlayText1;

            phaseSecondLineTextBlock.Text = isDiscardPhase
                ? Lang.Game_PhaseDiscardText2
                : Lang.Game_PhasePlayText2;
        }

        private List<PlayingCardItem> BuildFaceDownHand()
        {
            List<PlayingCardItem> cards = new List<PlayingCardItem>();

            for (int index = 0; index < HandSize; index++)
            {
                cards.Add(new PlayingCardItem());
            }

            return cards;
        }

        private List<PlayingCardItem> BuildPlayerHand()
        {
            return new List<PlayingCardItem>
            {
                new PlayingCardItem { Rank = "J", Suit = "♥", IsRedSuit = true },
                new PlayingCardItem { Rank = "2", Suit = "♠", IsRedSuit = false },
                new PlayingCardItem { Rank = "3", Suit = "♦", IsRedSuit = true },
                new PlayingCardItem { Rank = "10", Suit = "♦", IsRedSuit = true },
                new PlayingCardItem { Rank = "K", Suit = "♠", IsRedSuit = false },
                new PlayingCardItem { Rank = "5", Suit = "♠", IsRedSuit = false }
            };
        }

        private PlayingCardItem BuildStarterCard()
        {
            return new PlayingCardItem { Rank = "9", Suit = "♥", IsRedSuit = true };
        }

        private List<PlayingCardItem> BuildPlayedCards()
        {
            return new List<PlayingCardItem>
            {
                new PlayingCardItem { Rank = "7", Suit = "♠", IsRedSuit = false },
                new PlayingCardItem { Rank = "8", Suit = "♥", IsRedSuit = true }
            };
        }

        private List<BoardHoleItem> BuildTrack(int score, int lastGain)
        {
            List<BoardHoleItem> holes = new List<BoardHoleItem>();
            int previousScore = score - lastGain;

            for (int position = 1; position <= WinningScore; position++)
            {
                holes.Add(new BoardHoleItem
                {
                    Position = position,
                    HasPeg = position == score || position == previousScore
                });
            }

            return holes;
        }

        private void DiscardButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedCards = _playerHand.Count(card => card.IsSelected);

            if (selectedCards != CardsToDiscard)
            {
                matchMessagePanel.ShowError(Lang.Game_SelectTwoCardsMessage);
                return;
            }

            matchMessagePanel.HidePanel();
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