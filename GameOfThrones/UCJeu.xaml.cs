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
using System.Windows.Threading;

namespace GameOfThrones
{
    /// <summary>
    /// Logique d'interaction pour UCJeu.xaml
    /// </summary>
    public partial class UCJeu : UserControl
    {
        
        DispatcherTimer minuteurJeu = new DispatcherTimer();
        
        bool allerHaut, allerBas, allerGauche, allerDroite;

        // Éléments du jeu
        Rectangle formeDonjon;
        Ellipse formeRoi;
        double vitesseRoi = 5;

        // Listes d'entités
        List<Ellipse> listeEnnemis = new List<Ellipse>();
        List<Ellipse> listeProjectiles = new List<Ellipse>();

        // Statistiques
        int pointsVieDonjon = 100;
        int compteurApparitionEnnemi = 0;
        bool estJeuEnCours = false;

        public UCJeu()
        {
            InitializeComponent();

            // Configuration du minuteur (environ 60 images par seconde)
            minuteurJeu.Interval = TimeSpan.FromMilliseconds(16);
            minuteurJeu.Tick += BouclePrincipaleJeu;
        }

        // --- Événements Liés au XAML ---

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!estJeuEnCours)
            {
                this.Focusable = true;
                this.Focus();
                DemarrerPartie();
                estJeuEnCours = true;
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus(); // S'assurer qu'on a le focus pour le clavier

            if (e.ChangedButton == MouseButton.Left)
            {
                CreerProjectile(e.GetPosition(GameCanvas));
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z) allerHaut = true;
            if (e.Key == Key.S) allerBas = true;
            if (e.Key == Key.Q) allerGauche = true;
            if (e.Key == Key.D) allerDroite = true;
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z) allerHaut = false;
            if (e.Key == Key.S) allerBas = false;
            if (e.Key == Key.Q) allerGauche = false;
            if (e.Key == Key.D) allerDroite = false;
        }

        // --- Méthodes Logiques (PascalCase) ---

        private void DemarrerPartie()
        {
            GameCanvas.Children.Clear();
            listeEnnemis.Clear();
            listeProjectiles.Clear();
            pointsVieDonjon = 100;

            // Réinitialisation de l'affichage
            StatusText.Text = "Vie Donjon: 100";
            if (!GameCanvas.Children.Contains(StatusText))
            {
                GameCanvas.Children.Add(StatusText);
            }

            // Calcul du centre
            double centreX = this.ActualWidth / 2;
            double centreY = this.ActualHeight / 2;

            // Création du Donjon
            formeDonjon = new Rectangle { Width = 60, Height = 60, Fill = Brushes.Gray, Stroke = Brushes.White };
            Canvas.SetLeft(formeDonjon, centreX - 30);
            Canvas.SetTop(formeDonjon, centreY - 30);
            GameCanvas.Children.Add(formeDonjon);

            // Création du Roi
            formeRoi = new Ellipse { Width = 20, Height = 20, Fill = Brushes.CornflowerBlue };
            Canvas.SetLeft(formeRoi, centreX);
            Canvas.SetTop(formeRoi, centreY + 50);
            GameCanvas.Children.Add(formeRoi);

            minuteurJeu.Start();
        }

        private void BouclePrincipaleJeu(object sender, EventArgs e)
        {
            DeplacerRoi();
            GererApparitionEnnemis();
            DeplacerEnnemisEtVerifierCollisions();
            DeplacerProjectiles();
        }

        private void DeplacerRoi()
        {
            double posX = Canvas.GetLeft(formeRoi);
            double posY = Canvas.GetTop(formeRoi);

            if (allerHaut && posY > 0) posY -= vitesseRoi;
            if (allerBas && posY < this.ActualHeight - 20) posY += vitesseRoi;
            if (allerGauche && posX > 0) posX -= vitesseRoi;
            if (allerDroite && posX < this.ActualWidth - 20) posX += vitesseRoi;

            Canvas.SetLeft(formeRoi, posX);
            Canvas.SetTop(formeRoi, posY);
        }

        private void GererApparitionEnnemis()
        {
            compteurApparitionEnnemi--;
            if (compteurApparitionEnnemi < 0)
            {
                CreerEnnemi();
                compteurApparitionEnnemi = 60; // Réinitialise le compteur
            }
        }

        private void CreerEnnemi()
        {
            if (this.ActualWidth == 0) return;

            Ellipse nouvelEnnemi = new Ellipse { Width = 20, Height = 20, Fill = Brushes.Red };
            Random generateurAleatoire = new Random();

            // Apparition en haut aléatoire
            Canvas.SetLeft(nouvelEnnemi, generateurAleatoire.Next(0, (int)this.ActualWidth));
            Canvas.SetTop(nouvelEnnemi, 0);

            listeEnnemis.Add(nouvelEnnemi);
            GameCanvas.Children.Add(nouvelEnnemi);
        }

        private void DeplacerEnnemisEtVerifierCollisions()
        {
            Rect hitboxDonjon = new Rect(Canvas.GetLeft(formeDonjon), Canvas.GetTop(formeDonjon), formeDonjon.Width, formeDonjon.Height);

            for (int i = listeEnnemis.Count - 1; i >= 0; i--)
            {
                Ellipse ennemiCourant = listeEnnemis[i];
                CalculerMouvementEnnemi(ennemiCourant);

                Rect hitboxEnnemi = new Rect(Canvas.GetLeft(ennemiCourant), Canvas.GetTop(ennemiCourant), ennemiCourant.Width, ennemiCourant.Height);

                // Collision avec le donjon
                if (hitboxEnnemi.IntersectsWith(hitboxDonjon))
                {
                    pointsVieDonjon -= 10;
                    StatusText.Text = "Vie Donjon: " + pointsVieDonjon;

                    GameCanvas.Children.Remove(ennemiCourant);
                    listeEnnemis.RemoveAt(i);

                    if (pointsVieDonjon <= 0) DeclencherFinDePartie();
                }
            }
        }

        private void CalculerMouvementEnnemi(Ellipse ennemi)
        {
            double vitesseEnnemi = 2;
            double posX = Canvas.GetLeft(ennemi);
            double posY = Canvas.GetTop(ennemi);

            // Cible : Centre du donjon
            double cibleX = Canvas.GetLeft(formeDonjon) + 30;
            double cibleY = Canvas.GetTop(formeDonjon) + 30;

            double vecteurX = cibleX - posX;
            double vecteurY = cibleY - posY;

            // Normalisation du vecteur
            double longueur = Math.Sqrt(vecteurX * vecteurX + vecteurY * vecteurY);
            if (longueur > 0)
            {
                vecteurX /= longueur;
                vecteurY /= longueur;
            }

            Canvas.SetLeft(ennemi, posX + vecteurX * vitesseEnnemi);
            Canvas.SetTop(ennemi, posY + vecteurY * vitesseEnnemi);
        }

        private void CreerProjectile(Point positionSouris)
        {
            Ellipse projectile = new Ellipse { Width = 10, Height = 10, Fill = Brushes.Yellow };

            double departX = Canvas.GetLeft(formeRoi) + 10;
            double departY = Canvas.GetTop(formeRoi) + 10;

            Canvas.SetLeft(projectile, departX);
            Canvas.SetTop(projectile, departY);

            // Calcul trajectoire
            double vecteurX = positionSouris.X - departX;
            double vecteurY = positionSouris.Y - departY;
            double longueur = Math.Sqrt(vecteurX * vecteurX + vecteurY * vecteurY);
            double vitesseProjectile = 10;

            // Stockage de la vélocité dans le Tag
            projectile.Tag = new Point((vecteurX / longueur) * vitesseProjectile, (vecteurY / longueur) * vitesseProjectile);

            listeProjectiles.Add(projectile);
            GameCanvas.Children.Add(projectile);
        }

        private void DeplacerProjectiles()
        {
            for (int i = listeProjectiles.Count - 1; i >= 0; i--)
            {
                Ellipse projectileCourant = listeProjectiles[i];
                Point velocite = (Point)projectileCourant.Tag;

                Canvas.SetLeft(projectileCourant, Canvas.GetLeft(projectileCourant) + velocite.X);
                Canvas.SetTop(projectileCourant, Canvas.GetTop(projectileCourant) + velocite.Y);

                Rect hitboxProjectile = new Rect(Canvas.GetLeft(projectileCourant), Canvas.GetTop(projectileCourant), projectileCourant.Width, projectileCourant.Height);

                // Vérification sortie écran
                if (Canvas.GetLeft(projectileCourant) < 0 || Canvas.GetLeft(projectileCourant) > this.ActualWidth ||
                    Canvas.GetTop(projectileCourant) < 0 || Canvas.GetTop(projectileCourant) > this.ActualHeight)
                {
                    GameCanvas.Children.Remove(projectileCourant);
                    listeProjectiles.RemoveAt(i);
                    continue;
                }

                // Vérification collision avec ennemis
                bool aTouche = false;
                for (int j = listeEnnemis.Count - 1; j >= 0; j--)
                {
                    Ellipse ennemi = listeEnnemis[j];
                    Rect hitboxEnnemi = new Rect(Canvas.GetLeft(ennemi), Canvas.GetTop(ennemi), ennemi.Width, ennemi.Height);

                    if (hitboxProjectile.IntersectsWith(hitboxEnnemi))
                    {
                        GameCanvas.Children.Remove(ennemi);
                        listeEnnemis.RemoveAt(j);
                        GameCanvas.Children.Remove(projectileCourant);
                        listeProjectiles.RemoveAt(i);
                        aTouche = true;
                        break;
                    }
                }
                if (aTouche) continue;
            }
        }

        private void DeclencherFinDePartie()
        {
            minuteurJeu.Stop();
            MessageBox.Show("Le Donjon est détruit !");
            estJeuEnCours = false;
            DemarrerPartie();
        }
    }
}

