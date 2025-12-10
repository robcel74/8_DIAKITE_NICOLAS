using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
      
        public MainWindow()
        {
            InitializeComponent();
            AfficheUCDemarrage();
        }      
        private void AfficheUCDemarrage()
        {
            UCDemarrage uc = new UCDemarrage();
            AireJeu.Content = uc;
            uc.butParametres.Click += AfficheUCParametres;
            uc.butJouer.Click += AfficheUCMaps;
        }

        private void AfficheUCMaps(object sender, RoutedEventArgs e)
        {
            UCMaps uc = new UCMaps();
            AireJeu.Content = uc;
            uc.but_retour.Click += RetourUCDemarrage;
            uc.but_appliquer.Click += AfficheUCJeu;
        }

        private void AfficheUCJeu(object sender, RoutedEventArgs e)
        {
            UCJeu uc = new UCJeu();
            AireJeu.Content=uc;
        }

        private void AfficheUCParametres(object sender, RoutedEventArgs e)
        {
            UCParametres uc = new UCParametres();
            AireJeu.Content=uc;
            uc.but_retour.Click += RetourUCDemarrage;
        }

        private void RetourUCDemarrage(object sender, RoutedEventArgs e)
        {
            UCDemarrage uc = new UCDemarrage();
            AireJeu.Content = uc;
            uc.butParametres.Click += AfficheUCParametres;
            uc.butJouer.Click += AfficheUCMaps;
        }
    }
}