using SlidingPuzzle.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SlidingPuzzle
{
    /// <summary>
    /// Interaction logic for TitleScreen.xaml
    /// </summary>
    public partial class TitleScreen : UserControl
    {
        public TitleScreen()
        {
            InitializeComponent();
        }

        private void ImgNewGame_MouseEnter(object sender, MouseEventArgs e)
        {
            this.ImgNewGame.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.Start_Button_Hover);
        }

        private void ImgNewGame_MouseLeave(object sender, MouseEventArgs e)
        {
            this.ImgNewGame.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.Start_Button_Base);
        }

        private void ImgHowToPlay_MouseEnter(object sender, MouseEventArgs e)
        {
            this.ImgHowToPlay.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.HowtoPlay_Button_Hover);
        }

        private void ImgHowToPlay_MouseLeave(object sender, MouseEventArgs e)
        {
            this.ImgHowToPlay.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.HowtoPlay_Button_Base);
        }

        private void ImgHiScores_MouseEnter(object sender, MouseEventArgs e)
        {
            this.ImgHiScores.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.HiScores_Button_Hover);
        }

        private void ImgHiScores_MouseLeave(object sender, MouseEventArgs e)
        {
            this.ImgHiScores.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.HiScores_Button_Base);
        }
    }
}
