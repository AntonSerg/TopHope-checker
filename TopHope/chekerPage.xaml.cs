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
using TopHope.Core;
using System.Threading;
using System.Windows.Threading;

namespace TopHope
{
    /// <summary>
    /// Interaction logic for chekerPage.xaml
    /// </summary>
    public partial class chekerPage : Page
    {
        TopHopeCheker topHopeCheker;
        public chekerPage()
        {
            InitializeComponent();
            topHopeCheker = new TopHopeCheker(mainGrid, sourceCount, goodCount, badCount, threadsCount, testBox);
        }

        private void Button_Click_testStart(object sender, RoutedEventArgs e)
        {
            topHopeCheker.checkStart();
        }
        
        private void Button_Click_loadBase(object sender, RoutedEventArgs e)
        {
            topHopeCheker.loadBase();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }
    }
}
