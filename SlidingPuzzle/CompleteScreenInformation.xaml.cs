using SlidingPuzzle.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for CompleteScreenInformation.xaml
    /// </summary>
    public partial class CompleteScreenInformation : UserControl
    {
        private bool validName;

        public CompleteScreenInformation()
        {
            InitializeComponent();
            validName = false;
        }

        public bool HasValidName()
        {
            return validName;
        }

        public void ResetValidName()
        {
            validName = false;
            TxtBxName.Text = "";
        }

        private void TxtBxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.Match(TxtBxName.Text, @"^([0-9a-zA-Z ]+)$").Success)
            {
                if (!validName)
                {
                    validName = true;
                    this.ImgSubmit.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.Submit_Button_Base);
                }
            } else if (validName)
            {
                validName = false;
                this.ImgSubmit.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.Submit_Button_Disabled);
            }
        }

        private void ImgSubmit_MouseEnter(object sender, MouseEventArgs e)
        {
            if (validName)
                this.ImgSubmit.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.Submit_Button_Hover);
        }

        private void ImgSubmit_MouseLeave(object sender, MouseEventArgs e)
        {
            if (validName)
                this.ImgSubmit.Source = ImageUtility.GetImageSourceFromBitmap(SlidingPuzzle.Properties.Resources.Submit_Button_Base);
        }
    }
}
