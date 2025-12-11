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
    /// Logique d'interaction pour UCReglesJeu.xaml
    /// </summary>
    public partial class UCReglesJeu : UserControl
    {
        public UCReglesJeu()
        {
            InitializeComponent();
        }

        private void but_suivant_Click(object sender, RoutedEventArgs e)
        {
            lab_regleN1.Content = "Regle N°2";
            lab_regle1.Content = "Tenir le plus longtemps possible";
            but_suivant.IsEnabled = false;
            but_skip.Content = "Jouer";
        }
    }
}
