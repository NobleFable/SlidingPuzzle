using SlidingPuzzle.Code.Data;
using SlidingPuzzle.Code.Database;
using SlidingPuzzle.Code.Utility;
using SlidingPuzzle.Data;
using SlidingPuzzle.GameLogic;
using SlidingPuzzle.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SlidingPuzzle
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly object syncLock = new object();

        public const int WIDTH = 5, HEIGHT = 5;
        public const string IMG_SQUARE_PREFIX = "ImgSquare";

        private MainWindow window;
        private PuzzleLogic logic;
        private CurrentGame currentGame;

        private DatabaseHandler dbHandler;

        private bool transitioning;
        private int transitioningTimer;

        private DispatcherTimer gameTicker;

        public App()
            : base()
        {
            window = new MainWindow();
            logic = new PuzzleLogic(App.WIDTH, App.HEIGHT);

            dbHandler = new DatabaseHandler();

            gameTicker = new DispatcherTimer();
            gameTicker.Interval = new TimeSpan(0, 0, 1);

            InitializeDatabase();
            InitializeMainWindow();
            InitializeEventHandlers();

            window.Show();
        }

        private void InitializeDatabase()
        {
            dbHandler.LoadData();
        }

        private void InitializeMainWindow()
        {
            System.Drawing.Bitmap bmpImage = SlidingPuzzle.Properties.Resources.Present_Picture;
            System.Drawing.Bitmap bmpBlank = SlidingPuzzle.Properties.Resources.Blank_Square;

            int imgWidth = bmpImage.Width / App.WIDTH;
            int imgHeight = bmpImage.Height / App.HEIGHT;

            for (int x = 0; x < App.WIDTH; ++x)
            {
                for (int y = 0; y < App.HEIGHT; ++y)
                {
                    if (x + 1 == App.WIDTH && y + 1 == App.HEIGHT)
                        break;
                    ImageSource imgSource = ImageUtility.GetImageSourceFromBitmap(bmpImage, x * imgWidth, y * imgHeight, imgWidth, imgHeight);
                    ((Image)window.UsrCntrlGameScreen.UsrCntrlPuzzleBox.FindName(App.IMG_SQUARE_PREFIX + (x + 1 + y * App.HEIGHT))).Source = imgSource;
                }
            }
            ImageSource imgBlankSource = ImageUtility.GetImageSourceFromBitmap(bmpBlank);
            ((Image)window.UsrCntrlGameScreen.UsrCntrlPuzzleBox.FindName(App.IMG_SQUARE_PREFIX + (App.WIDTH * App.HEIGHT))).Source = imgBlankSource;
        }

        private void InitializeEventHandlers()
        {
            // General App events
            gameTicker.Tick += this.OnGameTicker_Tick;
            window.Closed += this.Window_Closed;

            // UsrCntrlTitleScreen
            window.UsrCntrlTitleScreen.ImgNewGame.MouseUp += this.TitleScreenNewGame_MouseUp;
            window.UsrCntrlTitleScreen.ImgHowToPlay.MouseUp += this.TitleScreenHowToPlay_MouseUp;
            window.UsrCntrlTitleScreen.ImgHiScores.MouseUp += this.TitleScreenHiScores_MouseUp;

            // UsrCntrlHowToPlayScreen
            window.UsrCntrlHowToPlayScreen.ImgReturn.MouseUp += this.HowToPlayScreenReturn_MouseUp;

            // UsrCntrlGameScreen(Header/Footer)
            window.UsrCntrlGameScreen.UsrCntrlGameScreenHeader.ImgReturn.MouseUp += this.GameScreenReturn_MouseUp;
            window.UsrCntrlGameScreen.UsrCntrlGameScreenFooter.ImgStart.MouseUp += this.GameScreenStart_MouseUp;
            window.UsrCntrlGameScreen.UsrCntrlGameScreenFooter.ImgPeek.MouseEnter += this.GameScreenPeek_MouseEnter;
            window.UsrCntrlGameScreen.UsrCntrlGameScreenFooter.ImgPeek.MouseLeave += this.GameScreenPeek_MouseLeave;

            // UsrCntrlPuzzleBox
            for (int i = 1; i <= App.WIDTH * App.HEIGHT; ++i)
            {
                ((Image)window.UsrCntrlGameScreen.UsrCntrlPuzzleBox.FindName(App.IMG_SQUARE_PREFIX + i)).MouseUp += this.ImgSquare_MouseUp;
            }

            // UsrCntrlCompleteScreen
            window.UsrCntrlCompleteScreen.UsrCntrlCompleteScreenInformation.ImgSubmit.MouseUp += this.CompleteScreenSubmit_MouseUp;
            window.UsrCntrlCompleteScreen.ImgNewGame.MouseUp += this.CompleteScreenNewGame_MouseUp;
            window.UsrCntrlCompleteScreen.ImgReturn.MouseUp += this.CompleteScreenReturn_MouseUp;

            // UsrCntrlHiScoresScreen
            window.UsrCntrlHiScoresScreen.ImgReturn.MouseUp += this.HiScoresScreen_MouseUp;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (gameTicker.IsEnabled)
                gameTicker.Stop();
        }

        private void TitleScreenNewGame_MouseUp(object sender, MouseButtonEventArgs e)
        {
            window.UsrCntrlTitleScreen.IsEnabled = false;
            window.UsrCntrlTitleScreen.Visibility = Visibility.Hidden;
            window.UsrCntrlGameScreen.IsEnabled = true;
            window.UsrCntrlGameScreen.Visibility = Visibility.Visible;
            ResetGame();
        }

        private void TitleScreenHowToPlay_MouseUp(object sender, MouseButtonEventArgs e)
        {
            window.UsrCntrlTitleScreen.IsEnabled = false;
            window.UsrCntrlTitleScreen.Visibility = Visibility.Hidden;
            window.UsrCntrlHowToPlayScreen.IsEnabled = true;
            window.UsrCntrlHowToPlayScreen.Visibility = Visibility.Visible;
        }

        private void TitleScreenHiScores_MouseUp(object sender, MouseButtonEventArgs e)
        {
            window.UsrCntrlTitleScreen.IsEnabled = false;
            window.UsrCntrlTitleScreen.Visibility = Visibility.Hidden;
            window.UsrCntrlHiScoresScreen.IsEnabled = true;
            window.UsrCntrlHiScoresScreen.Visibility = Visibility.Visible;
            LoadHiScores();
        }

        private void HowToPlayScreenReturn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            window.UsrCntrlHowToPlayScreen.IsEnabled = false;
            window.UsrCntrlHowToPlayScreen.Visibility = Visibility.Hidden;
            window.UsrCntrlTitleScreen.IsEnabled = true;
            window.UsrCntrlTitleScreen.Visibility = Visibility.Visible;
        }

        private void GameScreenReturn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            window.UsrCntrlGameScreen.IsEnabled = false;
            window.UsrCntrlGameScreen.Visibility = Visibility.Hidden;
            window.UsrCntrlTitleScreen.IsEnabled = true;
            window.UsrCntrlTitleScreen.Visibility = Visibility.Visible;
        }

        private void GameScreenStart_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentGame.GameUnderway)
                return;
            StartGame();
        }

        private void GameScreenPeek_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!currentGame.GameUnderway)
                return;
            ShowDefaultPuzzle();
        }

        private void GameScreenPeek_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!currentGame.GameUnderway)
                return;
            ShowMixedPuzzle();
        }

        private void ImgSquare_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!currentGame.GameUnderway)
                return;

            var imageClicked = (System.Windows.Controls.Image)sender;
            int xCoord = ((int)imageClicked.GetValue(Grid.ColumnProperty)) / 2;
            int yCoord = ((int)imageClicked.GetValue(Grid.RowProperty)) / 2;

            if (!logic.IsValidMove(xCoord, yCoord))
                return;

            MoveData move = logic.MakeMove(xCoord, yCoord);
            var imageEmpty = (Image)window.UsrCntrlGameScreen.UsrCntrlPuzzleBox.FindName(App.IMG_SQUARE_PREFIX + move.newValue);
            MoveImage(imageClicked, move.newX, move.newY);
            MoveImage(imageEmpty, move.oldX, move.oldY);

            currentGame.MovesCompleted += 1;
            window.UsrCntrlGameScreen.UsrCntrlGameScreenHeader.LblMoves.Content = currentGame.MovesCompleted.ToString();

            if (!logic.CheckComplete())
                return;

            CompleteGame();
        }

        private void MoveImage(Image image, int x, int y)
        {
            image.SetValue(Grid.ColumnProperty, 1 + x * 2);
            image.SetValue(Grid.RowProperty, 1 + y * 2);
        }

        private void CompleteScreenSubmit_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!window.UsrCntrlCompleteScreen.UsrCntrlCompleteScreenInformation.HasValidName())
                return;
            dbHandler.InsertNewHiScoreAsync(window.UsrCntrlCompleteScreen.UsrCntrlCompleteScreenInformation.TxtBxName.Text, currentGame.TimeElapsed, currentGame.MovesCompleted);
            window.UsrCntrlCompleteScreen.IsEnabled = false;
            window.UsrCntrlCompleteScreen.Visibility = Visibility.Hidden;
            window.UsrCntrlHiScoresScreen.IsEnabled = true;
            window.UsrCntrlHiScoresScreen.Visibility = Visibility.Visible;
            LoadHiScores();
        }

        private void CompleteScreenNewGame_MouseUp(object sender, MouseButtonEventArgs e)
        {
            window.UsrCntrlCompleteScreen.IsEnabled = false;
            window.UsrCntrlCompleteScreen.Visibility = Visibility.Hidden;
            window.UsrCntrlGameScreen.IsEnabled = true;
            window.UsrCntrlGameScreen.Visibility = Visibility.Visible;
            ResetGame();
        }

        private void CompleteScreenReturn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            window.UsrCntrlCompleteScreen.IsEnabled = false;
            window.UsrCntrlCompleteScreen.Visibility = Visibility.Hidden;
            window.UsrCntrlTitleScreen.IsEnabled = true;
            window.UsrCntrlTitleScreen.Visibility = Visibility.Visible;
        }

        private void HiScoresScreen_MouseUp(object sender, MouseButtonEventArgs e)
        {
            window.UsrCntrlHiScoresScreen.IsEnabled = false;
            window.UsrCntrlHiScoresScreen.Visibility = Visibility.Hidden;
            window.UsrCntrlTitleScreen.IsEnabled = true;
            window.UsrCntrlTitleScreen.Visibility = Visibility.Visible;
        }

        private void ResetGame()
        {
            currentGame.GameUnderway = false;
            currentGame.MovesCompleted = 0;
            currentGame.TimeElapsed = 0;
            transitioning = false;

            window.UsrCntrlGameScreen.UsrCntrlGameScreenFooter.SetGameRunning(false);
            window.UsrCntrlGameScreen.UsrCntrlGameScreenHeader.SetDisableReturn(false);
            window.UsrCntrlGameScreen.UsrCntrlGameScreenFooter.ImgStart.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.StartGame_Button_Base);
            window.UsrCntrlGameScreen.UsrCntrlGameScreenHeader.ImgReturn.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.Return_Button_Base);
            window.UsrCntrlGameScreen.UsrCntrlGameScreenHeader.LblMoves.Content = "0";
            window.UsrCntrlGameScreen.UsrCntrlGameScreenHeader.LblTime.Content = "00:00";

            ShowDefaultPuzzle();
        }

        private void StartGame()
        {
            currentGame.GameUnderway = true;

            window.UsrCntrlGameScreen.UsrCntrlGameScreenFooter.SetGameRunning(true);
            window.UsrCntrlGameScreen.UsrCntrlGameScreenFooter.ImgStart.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.StartGame_Button_Disabled);

            logic.ResetPuzzle();
            ShowMixedPuzzle();

            gameTicker.Interval = new TimeSpan(0, 0, 1);
            gameTicker.Start();
        }

        public void CompleteGame()
        {
            currentGame.GameUnderway = false;
            window.UsrCntrlGameScreen.UsrCntrlGameScreenHeader.ImgReturn.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.Return_Button_Disabled);
            window.UsrCntrlGameScreen.UsrCntrlGameScreenHeader.SetDisableReturn(true);
            transitioning = true;
            transitioningTimer = 0;
            gameTicker.Stop();
            gameTicker.Interval = new TimeSpan(0, 0, 0, 0, 250);
            gameTicker.Start();
        }

        public void QuitGame()
        {
            gameTicker.Stop();
        }

        public void ShowDefaultPuzzle()
        {
            for (int x = 0; x < App.WIDTH; ++x)
            {
                for (int y = 0; y < App.HEIGHT; ++y)
                {
                    Image image = (Image)window.UsrCntrlGameScreen.UsrCntrlPuzzleBox.FindName(App.IMG_SQUARE_PREFIX + (x + 1 + y * App.HEIGHT));
                    image.SetValue(Grid.ColumnProperty, x * 2 + 1);
                    image.SetValue(Grid.RowProperty, y * 2 + 1);
                }
            }
        }

        public void ShowMixedPuzzle()
        {
            for (int x = 0; x < App.WIDTH; ++x)
            {
                for (int y = 0; y < App.HEIGHT; ++y)
                {
                    int imageAtLoc = logic.GetPuzzleNumberAt(x, y);
                    Image image = (Image)window.UsrCntrlGameScreen.UsrCntrlPuzzleBox.FindName(App.IMG_SQUARE_PREFIX + imageAtLoc);
                    image.SetValue(Grid.ColumnProperty, x * 2 + 1);
                    image.SetValue(Grid.RowProperty, y * 2 + 1);
                }
            }
        }

        public void OnGameTicker_Tick(object source, EventArgs e)
        {
            if (currentGame.GameUnderway)
            {
                currentGame.TimeElapsed += 1;
                window.UsrCntrlGameScreen.UsrCntrlGameScreenHeader.LblTime.Content = TimeUtility.GetTimeString(currentGame.TimeElapsed);
            }
            else if (transitioning)
            {
                transitioningTimer += 1;
                if (transitioningTimer < 3)
                    return;

                if (transitioningTimer < 9)
                {
                    for (int i = 1; i < App.WIDTH * App.HEIGHT + 1; ++i)
                    {
                        ((Image)window.UsrCntrlGameScreen.UsrCntrlPuzzleBox.FindName(App.IMG_SQUARE_PREFIX + i)).Visibility = transitioningTimer % 2 == 1 ? Visibility.Hidden : Visibility.Visible;
                    }
                }

                if (transitioningTimer == 15)
                {
                    gameTicker.Stop();
                    window.UsrCntrlGameScreen.IsEnabled = false;
                    window.UsrCntrlGameScreen.Visibility = Visibility.Hidden;
                    window.UsrCntrlCompleteScreen.IsEnabled = true;
                    window.UsrCntrlCompleteScreen.Visibility = Visibility.Visible;
                    window.UsrCntrlCompleteScreen.UsrCntrlCompleteScreenInformation.LblMoves.Content = currentGame.MovesCompleted.ToString();
                    window.UsrCntrlCompleteScreen.UsrCntrlCompleteScreenInformation.LblTime.Content = TimeUtility.GetTimeString(currentGame.TimeElapsed);
                    window.UsrCntrlCompleteScreen.UsrCntrlCompleteScreenInformation.ResetValidName();
                    window.UsrCntrlCompleteScreen.UsrCntrlCompleteScreenInformation.ImgTimeHiScore.Visibility = dbHandler.CheckTimeHiScoresAsync(currentGame.TimeElapsed) ? Visibility.Visible : Visibility.Hidden;
                    window.UsrCntrlCompleteScreen.UsrCntrlCompleteScreenInformation.ImgMovesHiScore.Visibility = dbHandler.CheckMovesHiScoresAsync(currentGame.MovesCompleted) ? Visibility.Visible : Visibility.Hidden;
                }
            }
        }

        private void LoadHiScores()
        {
            var timeScores = dbHandler.GetTopFiveTimeHiScoresAsync();
            if (timeScores != null && timeScores.Count() > 0)
            {
                int count = 1;
                foreach (HiScoreData data in timeScores)
                {
                    ((Label)window.UsrCntrlHiScoresScreen.FindName("LblTimesName" + count)).Content = data.Name;
                    ((Label)window.UsrCntrlHiScoresScreen.FindName("LblTimesScore" + count)).Content = TimeUtility.GetTimeString(data.Value);
                    count += 1;
                }
            }

            var moveScores = dbHandler.GetTopFiveMovesHiScoresAsync();
            if (moveScores != null && moveScores.Count() > 0)
            {
                int count = 1;
                foreach (HiScoreData data in moveScores)
                {
                    ((Label)window.UsrCntrlHiScoresScreen.FindName("LblMovesName" + count)).Content = data.Name;
                    ((Label)window.UsrCntrlHiScoresScreen.FindName("LblMovesScore" + count)).Content = data.Value.ToString();
                    count += 1;
                }
            }
        }
    }
}
