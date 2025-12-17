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

            // 2. GESTION DE LA DIFFICULTÉ (Nouveau)
            // On remet le ComboBox sur la difficulté mémorisée (0, 1 ou 2)
            ComboDifficulte.SelectedIndex = fenetre.NiveauDifficulte;
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

        private void ComboDifficulte_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow fenetre = (MainWindow)Window.GetWindow(this);

            // On vérifie fenetre != null ET que la sélection est valide (pas -1)
            if (fenetre != null && ComboDifficulte.SelectedIndex >= 0)
            {
                // On met à jour la variable globale dans MainWindow
                // 0 = Facile, 1 = Moyen, 2 = Difficile
                fenetre.NiveauDifficulte = ComboDifficulte.SelectedIndex;
            }
        }
    }
}
