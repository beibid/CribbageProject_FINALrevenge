using System.Globalization;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views.TutorialSections
{
    public partial class PlayPhaseSection : UserControl
    {
        private const int FirstCardCount = 7;
        private const int SecondCardCount = 15;

        public PlayPhaseSection()
        {
            InitializeComponent();
            ShowRunningCount();
        }

        private void ShowRunningCount()
        {
            firstCountTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Tutorial_PlayPhase_CountFormat,
                FirstCardCount);

            secondCountTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Tutorial_PlayPhase_CountFormat,
                SecondCardCount);
        }
    }
}