using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

using TopHope.Core;

namespace TopHope
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        loginPage LPage = null;
        chekerPage CPage = null;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click_menuLogin(object sender, RoutedEventArgs e)
        {
            if(LPage == null)
            {
                LPage = new loginPage();
                Main.Content = LPage;
            }
            else
            {
                Main.Content = LPage;
            }


        }
        private void Button_Click_menuCheker(object sender, RoutedEventArgs e)
        {
            if (CPage == null)
            {
                CPage = new chekerPage();
                Main.Content = CPage;
            }
            else
            {
                Main.Content = CPage;
            }


        }
    }
}
