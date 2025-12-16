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

        private void Map_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton btn && btn.Name is string map)
            {
                but_appliquer.IsEnabled = true;
                MainWindow.Map = map;
            }
        }
    }
}
