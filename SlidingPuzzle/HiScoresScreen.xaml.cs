﻿using SlidingPuzzle.Utility;
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
    /// Interaction logic for HiScoresScreen.xaml
    /// </summary>
    public partial class HiScoresScreen : UserControl
    {
        public HiScoresScreen()
        {
            InitializeComponent();
        }

        private void ImgReturn_MouseEnter(object sender, MouseEventArgs e)
        {
            this.ImgReturn.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.Return_Button_Hover);
        }

        private void ImgReturn_MouseLeave(object sender, MouseEventArgs e)
        {
            this.ImgReturn.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.Return_Button_Base);
        }
    }
}
