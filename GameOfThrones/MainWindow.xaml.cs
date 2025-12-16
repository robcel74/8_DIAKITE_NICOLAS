using System.Media;
using System.Windows;
using System.Windows.Media;

namespace GameOfThrones
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private MediaPlayer musiqueFond;
        private static SoundPlayer sonCadeauGagne;

        public static string Map { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            AfficheUCDemarrage();
            InitialiserMusique();
        }


        //GESTION DES UC

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
            uc.but_appliquer.Click += AfficheUCRegles;
        }

        private void AfficheUCRegles(object sender, RoutedEventArgs e)
        {
            UCReglesJeu uc = new UCReglesJeu();
            AireJeu.Content = uc;
            uc.but_skip.Click += AfficheUCJeu;
        }

        private void AfficheUCJeu(object sender, RoutedEventArgs e)
        {
            UCJeu uc = new UCJeu();
            AireJeu.Content = uc;
            musiqueFond.Stop();
            

        }

        private void AfficheUCParametres(object sender, RoutedEventArgs e)
        {
            UCParametres uc = new UCParametres();
            AireJeu.Content = uc;
            uc.but_retour.Click += RetourUCDemarrage;
        }

        private void RetourUCDemarrage(object sender, RoutedEventArgs e)
        {
            UCDemarrage uc = new UCDemarrage();
            AireJeu.Content = uc;
            uc.butParametres.Click += AfficheUCParametres;
            uc.butJouer.Click += AfficheUCMaps;
        }

        public void AfficheUCGameOver()
        {
            UCGameOver uc = new UCGameOver();
            AireJeu.Content = uc;

        }
        public void AfficheUCRegle()
        {
            UCReglesJeu uc = new UCReglesJeu();
            AireJeu.Content = uc;

        }





        //GESTION DES MUSIQUES
        private void InitialiserMusique()
        {
            musiqueFond = new MediaPlayer();
            musiqueFond.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "RESSOURCES/MUSIQUES/musique_fond.mp3"));

            musiqueFond.Volume =0.5;

            musiqueFond.Play();
        }
        private void RelanceMusique(object? sender, EventArgs e)
        {
            musiqueFond.Position = TimeSpan.Zero;
            musiqueFond.Play();
        }

        public void ChangerVolume(double nouveauVolume)
        {
            if (musiqueFond != null)
            {
                musiqueFond.Volume = nouveauVolume;
            }
        }

        // Ajoute aussi ça pour que le Slider sache à combien on est au début
        public double GetVolumeActuel()
        {
            if (musiqueFond != null) return musiqueFond.Volume;
            return 0.5; // Valeur par défaut
        }
      
       
        



    }
}