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

        
            SectionContent.Content = new GameObjectiveSection();
        }

        private void ObjectiveButton_Click(object sender, RoutedEventArgs e)
        {
            SectionContent.Content = new GameObjectiveSection();
        }

        private void DealAndCribButton_Click(object sender, RoutedEventArgs e)
        {
            SectionContent.Content = new DealAndCribSection();
        }

        private void PlayPhaseButton_Click(object sender, RoutedEventArgs e)
        {
            SectionContent.Content = new PlayPhaseSection();
        }

        private void HandCountingButton_Click(object sender, RoutedEventArgs e)
        {
            SectionContent.Content = new HandCountingSection();
        }

        private void BoardButton_Click(object sender, RoutedEventArgs e)
        {
            SectionContent.Content = new BoardSection();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartMenu());
        }
    }
}