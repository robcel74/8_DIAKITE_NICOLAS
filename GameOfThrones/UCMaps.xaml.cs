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

namespace GameOfThrones
{
    /// <summary>
    /// Logique d'interaction pour UCMaps.xaml
    /// </summary>
    public partial class UCMaps : UserControl
    {

        public UCMaps()
        {
            InitializeComponent();
        }

        private void mpJungle_Click(object sender, RoutedEventArgs e)
        {
            but_appliquer.IsEnabled = true;
            MainWindow.Map = "Jungle";
        }

        private void mpHiver_Click(object sender, RoutedEventArgs e)
        {
            but_appliquer.IsEnabled = true;
            MainWindow.Map = "Hiver";
        }

        private void mpSable_Click(object sender, RoutedEventArgs e)
        {
            but_appliquer.IsEnabled = true;
            MainWindow.Map = "Sable";
        }

        private void mpLave_Click(object sender, RoutedEventArgs e)
        {
            but_appliquer.IsEnabled = true;
            MainWindow.Map = "Lave";
        }
    }
}
