using System.Globalization;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views.TutorialSections
{
    public partial class BoardSection : UserControl
    {
        private const int WinningScore = 61;
        private const int ExamplePlayerScore = 42;
        private const int ExampleOpponentScore = 38;
        private const int ExamplePointsGained = 8;

        public BoardSection()
        {
            InitializeComponent();
            ShowExampleScores();
        }

        private void ShowExampleScores()
        {
            goalTextBlock.Text = string.Format(CultureInfo.CurrentCulture,
                Lang.Tutorial_Board_GoalFormat, WinningScore);

            pointsGainedTextBlock.Text = string.Format(CultureInfo.CurrentCulture,
                Lang.Tutorial_Board_PointsGainedFormat, ExamplePointsGained);

            scoreTextBlock.Text = string.Format(CultureInfo.CurrentCulture,
                Lang.Tutorial_Board_ScoreFormat, ExamplePlayerScore, ExampleOpponentScore, WinningScore);
        }
    }
}