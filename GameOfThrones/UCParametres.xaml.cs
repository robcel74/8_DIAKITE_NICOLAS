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
    /// Logique d'interaction pour UCParametres.xaml
    /// </summary>
    public partial class UCParametres : UserControl
    {
        public UCParametres()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Au chargement, on met le slider à la bonne position par rapport au volume actuel
            MainWindow fenetre = (MainWindow)Window.GetWindow(this);
            SliderVolume.Value = fenetre.GetVolumeActuel();
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // 1. On récupère la fenêtre principale
            // Window.GetWindow(this) trouve la fenêtre qui contient ce UserControl
            MainWindow fenetre = (MainWindow)Window.GetWindow(this);

            // 2. On appelle la méthode publique pour changer le son
            // On vérifie que la fenêtre n'est pas nulle (sécurité)
            if (fenetre != null)
            {
                fenetre.ChangerVolume(SliderVolume.Value);
            }
        }
    }
}
