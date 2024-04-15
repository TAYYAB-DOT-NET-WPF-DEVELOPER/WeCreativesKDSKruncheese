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
using WeCreatives_EOI;

namespace WeCreatives_KDSPJ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        //    Closing += MainWindow_Closing;
            // listView.SelectedIndex = 0;
        }
        //private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    CloseWindow passwordDialog = new CloseWindow();
        //    if (passwordDialog.ShowDialog() == true)
        //    {
        //        // Check the entered password
        //        if (CheckPassword(passwordDialog.EnteredPassword))
        //        {
        //            // Password is correct, perform cleanup or other actions if needed
        //        }
        //        else
        //        {
        //            // Password is incorrect, cancel the closing event
        //            e.Cancel = true;
        //        }
        //    }
        //    else
        //    {
        //        e.Cancel = true;
        //    }
        //}

        //private bool CheckPassword(string enteredPassword)
        //{
        //    string correctPassword = "1122";
        //    return enteredPassword == correctPassword;
        //}

    }
}
