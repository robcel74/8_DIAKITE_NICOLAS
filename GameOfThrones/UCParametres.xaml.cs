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
        private MainWindow fenetre;
        private Button boutonEnCoursDeModification = null; // Pour savoir quel bouton on modifie
        public UCParametres()
        {
            InitializeComponent();

            this.Focusable = true;
            this.Loaded += (s, e) => {
                fenetre = (MainWindow)Window.GetWindow(this);
                MettreAJourTextesBoutons(); // Affiche les touches actuelles au chargement
            };
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

        // 1. Quand on clique sur un bouton pour changer une touche
        private void BtnChangerTouche_Click(object sender, RoutedEventArgs e)
        {
            boutonEnCoursDeModification = (Button)sender;
            boutonEnCoursDeModification.Content = "Appuyez sur une touche...";

            // On force le focus sur le UserControl pour capter la touche
            this.Focus();
            this.KeyDown += CapturerNouvelleTouche; // On s'abonne à l'événement
        }

        // 2. Quand on appuie sur la nouvelle touche
        private void CapturerNouvelleTouche(object sender, KeyEventArgs e)
        {
            if (boutonEnCoursDeModification != null)
            {
                // On regarde quel bouton était cliqué pour savoir quelle variable changer
                if (boutonEnCoursDeModification == BtnHaut) fenetre.ToucheHaut = e.Key;
                if (boutonEnCoursDeModification == BtnBas) fenetre.ToucheBas = e.Key;
                if (boutonEnCoursDeModification == BtnGauche) fenetre.ToucheGauche = e.Key;
                if (boutonEnCoursDeModification == BtnDroite) fenetre.ToucheDroite = e.Key;

                // On remet les textes à jour
                MettreAJourTextesBoutons();

                // On arrête d'écouter et on nettoie
                boutonEnCoursDeModification = null;
                this.KeyDown -= CapturerNouvelleTouche; // On se désabonne pour ne pas bugger
            }
        }

        private void MettreAJourTextesBoutons()
        {
            if (fenetre == null) return;
            BtnHaut.Content = "Haut : " + fenetre.ToucheHaut.ToString();
            BtnBas.Content = "Bas : " + fenetre.ToucheBas.ToString();
            BtnGauche.Content = "Gauche : " + fenetre.ToucheGauche.ToString();
            BtnDroite.Content = "Droite : " + fenetre.ToucheDroite.ToString();
        }
    }



}

