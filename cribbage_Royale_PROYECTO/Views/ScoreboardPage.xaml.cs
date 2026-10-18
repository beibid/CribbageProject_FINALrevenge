using cribbage_Royale_DataAccess.Models;
using cribbage_Royale_PROYECTO.Logging;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;
using cribbage_Royale_PROYECTO.Views.Dialogs;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class ScoreboardPage : Page
    {
        private const int FirstPosition = 1;
        private const string ScoreboardLoadFailureMessage =
            "No se pudo cargar la tabla de puntuaciones desde el servidor";

        private readonly StatisticsServiceClient _statisticsServiceClient;
        private readonly string _currentUserName;
        private readonly ILogger _logger;

        public ScoreboardPage(string currentUserName)
        {
            InitializeComponent();
            _statisticsServiceClient = new StatisticsServiceClient();
            _currentUserName = currentUserName;
            _logger = ApplicationLogging.CreateLogger<ScoreboardPage>();
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
            try
            {
                List<ScoreboardEntry> ranking =
                    await _statisticsServiceClient.GetScoreboardAsync();

                ShowRanking(BuildScoreboardItems(ranking));
            }
            catch (FaultException<ServiceFault> exceptionServiceFault)
            {
                ShowLoadingFailure(exceptionServiceFault, Lang.Scoreboard_LoadingErrorMessage);
            }
            catch (EndpointNotFoundException exceptionEndpointNotFound)
            {
                ShowLoadingFailure(exceptionEndpointNotFound, Lang.Scoreboard_ConnectionErrorMessage);
            }
            catch (TimeoutException exceptionTimeout)
            {
                ShowLoadingFailure(exceptionTimeout, Lang.Scoreboard_TimeoutMessage);
            }
            catch (CommunicationException exceptionCommunication)
            {
                ShowLoadingFailure(exceptionCommunication, Lang.Scoreboard_ConnectionErrorMessage);
            }
        }

        private void ShowLoadingFailure(Exception exception, string playerMessage)
        {
            _logger.LogError(exception, ScoreboardLoadFailureMessage);
            ShowRanking(new List<ScoreboardItem>());
            MessageDialog.ShowError(Window.GetWindow(this), playerMessage);
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