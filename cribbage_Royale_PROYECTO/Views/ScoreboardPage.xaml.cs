using cribbage_Royale_DataAccess.Models;
using cribbage_Royale_PROYECTO.Logging;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;
using cribbage_Royale_PROYECTO.Views.Dialogs;
using CribbageRoyale.Contracts.DataContracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class ScoreboardPage : Page
    {
        private const int FirstPosition = 1;

        private readonly StatisticsServiceClient _statisticsServiceClient;
        private readonly string _currentUserName;

        public ScoreboardPage(string currentUserName)
        {
            InitializeComponent();
            _statisticsServiceClient = new StatisticsServiceClient(
                ApplicationLogging.CreateLogger<StatisticsServiceClient>());
            _currentUserName = currentUserName;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadScoreboardAsync();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadScoreboardAsync();
        }

        private async Task LoadScoreboardAsync()
        {
            ScoreboardResult scoreboardResult = await _statisticsServiceClient.GetScoreboardAsync();

            if (scoreboardResult.Succeeded)
            {
                ShowRanking(BuildScoreboardItems(scoreboardResult.Entries));
            }
            else
            {
                ShowLoadingFailure(scoreboardResult.Reason);
            }
        }

        private void ShowLoadingFailure(ServiceFailureReason reason)
        {
            ShowRanking(new List<ScoreboardItem>());
            MessageDialog.ShowError(Window.GetWindow(this), BuildFailureMessage(reason));
        }

        private string BuildFailureMessage(ServiceFailureReason reason)
        {
            string message;

            switch (reason)
            {
                case ServiceFailureReason.Timeout:
                    message = Lang.Scoreboard_TimeoutMessage;
                    break;
                case ServiceFailureReason.ServerError:
                    message = Lang.Scoreboard_LoadingErrorMessage;
                    break;
                default:
                    message = Lang.Scoreboard_ConnectionErrorMessage;
                    break;
            }

            return message;
        }

        private List<ScoreboardItem> BuildScoreboardItems(List<ScoreboardEntry> ranking)
        {
            List<ScoreboardItem> items = new List<ScoreboardItem>();
            int position = FirstPosition;

            foreach (ScoreboardEntry entry in ranking)
            {
                items.Add(BuildScoreboardItem(entry, position));
                position++;
            }

            return items;
        }

        private ScoreboardItem BuildScoreboardItem(ScoreboardEntry entry, int position)
        {
            return new ScoreboardItem
            {
                Position = position,
                PlayerName = entry.DisplayName,
                Level = entry.SkillLevel,
                Played = entry.MatchesPlayed,
                Won = entry.Wins,
                Lost = entry.Losses,
                WinRate = entry.WinRate,
                WinRateText = string.Format(CultureInfo.CurrentCulture,
                    Lang.Scoreboard_WinRateFormat, entry.WinRate),
                IsCurrentPlayer = entry.UserName == _currentUserName
            };
        }

        private void ShowRanking(List<ScoreboardItem> items)
        {
            scoreboardDataGrid.ItemsSource = items;
            ShowEmptyListNotice(items.Count == 0);
        }

        private void ShowEmptyListNotice(bool isListEmpty)
        {
            if (isListEmpty)
            {
                emptyListTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                emptyListTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new Lobby());
            }
        }
    }
}