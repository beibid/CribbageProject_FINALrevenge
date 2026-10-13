using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_DataAccess;
using cribbage_Royale_DataAccess.DataAccess;
using cribbage_Royale_DataAccess.Models;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class ScoreboardPage : Page
    {
        private const int FirstPosition = 1;
        private const decimal WinRateWithoutMatches = 0m;

        private readonly ScoreboardRepository _scoreboardRepository;
        private readonly string _currentUserName;

        public ScoreboardPage(string currentUserName)
        {
            InitializeComponent();
            _scoreboardRepository = new ScoreboardRepository();
            _currentUserName = currentUserName;
            LoadScoreboard();
        }

        private void LoadScoreboard()
        {
            try
            {
                List<vista_estadisticas_jugador> ranking = _scoreboardRepository.GetRanking();
                ShowRanking(BuildScoreboardItems(ranking));
            }
            catch (EntityException)
            {
                ShowRanking(new List<ScoreboardItem>());
                MessageDialog.ShowError(Window.GetWindow(this), Lang.Scoreboard_LoadingErrorMessage);
            }
        }

        private List<ScoreboardItem> BuildScoreboardItems(List<vista_estadisticas_jugador> ranking)
        {
            List<ScoreboardItem> items = new List<ScoreboardItem>();
            int position = FirstPosition;

            foreach (vista_estadisticas_jugador record in ranking)
            {
                items.Add(BuildScoreboardItem(record, position));
                position++;
            }

            return items;
        }

        private ScoreboardItem BuildScoreboardItem(vista_estadisticas_jugador record, int position)
        {
            decimal winRate = record.porcentaje_victorias ?? WinRateWithoutMatches;

            return new ScoreboardItem
            {
                Position = position,
                PlayerName = record.nombre_mostrado,
                Level = record.nivel_habilidad,
                Played = record.partidas_jugadas,
                Won = record.victorias,
                Lost = record.derrotas,
                WinRate = winRate,
                WinRateText = string.Format(CultureInfo.CurrentCulture,
                    Lang.Scoreboard_WinRateFormat, winRate),
                IsCurrentPlayer = record.nombre_usuario == _currentUserName
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

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadScoreboard();
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