using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Views.TutorialSections;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class Tutorial : Page
    {
        public Tutorial()
        {
            InitializeComponent();
            objectiveNavButton.IsChecked = true;
        }

        private void ObjectiveNavButton_Checked(object sender, RoutedEventArgs e)
        {
            sectionContentControl.Content = new GameObjectiveSection();
        }

        private void DealAndCribNavButton_Checked(object sender, RoutedEventArgs e)
        {
            sectionContentControl.Content = new DealAndCribSection();
        }

        private void PlayPhaseNavButton_Checked(object sender, RoutedEventArgs e)
        {
            sectionContentControl.Content = new PlayPhaseSection();
        }

        private void HandCountingNavButton_Checked(object sender, RoutedEventArgs e)
        {
            sectionContentControl.Content = new HandCountingSection();
        }

        private void BoardNavButton_Checked(object sender, RoutedEventArgs e)
        {
            sectionContentControl.Content = new BoardSection();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new StartMenu());
            }
        }
    }
}