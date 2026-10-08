using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_DataAccess.Models;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class MatchHistoryPage : Page
    {
        public MatchHistoryPage()
        {
            InitializeComponent();
            ShowMatches(new List<MatchItem>());
        }

        private void ShowMatches(List<MatchItem> matches)
        {
            matchesListBox.ItemsSource = matches;
            emptyListTextBlock.Visibility = matches.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
                return;
            }

            NavigationService.Navigate(new Lobby());
        }
    }
}