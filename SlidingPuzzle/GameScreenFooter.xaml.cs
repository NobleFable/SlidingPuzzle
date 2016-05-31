using SlidingPuzzle.Utility;
using System.Windows.Controls;
using System.Windows.Input;

namespace SlidingPuzzle
{
    /// <summary>
    /// Interaction logic for GameScreenFooter.xaml
    /// </summary>
    public partial class GameScreenFooter : UserControl
    {
        private bool gameRunning;

        public GameScreenFooter()
        {
            InitializeComponent();
            gameRunning = false;
        }

        public void SetGameRunning(bool running)
        {
            gameRunning = running;
        }

        private void ImgPeek_MouseEnter(object sender, MouseEventArgs e)
        {
            if (gameRunning)
                this.ImgPeek.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.Peek_Button_Hover);
        }

        private void ImgPeek_MouseLeave(object sender, MouseEventArgs e)
        {
            if (gameRunning)
                this.ImgPeek.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.Peek_Button_Base);
        }

        private void ImgStart_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!gameRunning)
                this.ImgStart.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.StartGame_Button_Hover);
        }

        private void ImgStart_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!gameRunning)
                this.ImgStart.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.StartGame_Button_Base);
        }
    }
}
