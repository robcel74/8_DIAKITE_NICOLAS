using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
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
        Image imageDragon;
        double vitesseRoi = 5;    

        // 2. VARIABLES POUR L'ANIMATION
        int numeroImage = 1;          // Sera 1, 2 ou 3
        string direction = "bas";     // haut, bas, gauche, droite
        int compteurAnimation = 0;    // Pour ne pas changer d'image trop vite

        // Listes d'entités
        List<Ellipse> listeEnnemis = new List<Ellipse>();
        List<Ellipse> listeProjectiles = new List<Ellipse>();

        // Statistiques
        int pointsVieDonjon = 100;
        int compteurApparitionEnnemi = 0;
        bool estJeuEnCours = false;
        double vitesseEnnemiActuelle = 2;
        StackPanel panelVie;

        public UCJeu()
        {
            InitializeComponent();
            string nomFichierImage = $"pack://application:,,,/RESSOURCES/MAPS/Map{MainWindow.Map}.jpg";
            imgMap.Source = new BitmapImage(new Uri(nomFichierImage));

            // Configuration du minuteur (environ 60 images par seconde)
            minuteurJeu.Interval = TimeSpan.FromMilliseconds(16);
            minuteurJeu.Tick += BouclePrincipaleJeu;
        }        

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
            // On récupère la configuration depuis la fenêtre principale
            MainWindow fenetre = (MainWindow)Window.GetWindow(this);

            // On compare avec les variables de la fenêtre
            if (e.Key == fenetre.ToucheHaut)
            {
                allerHaut = true;
            }
            else if (e.Key == fenetre.ToucheBas)
            {
                allerBas = true;
            }
            else if (e.Key == fenetre.ToucheGauche)
            {
                allerGauche = true;
            }
            else if (e.Key == fenetre.ToucheDroite)
            {
                allerDroite = true;
            }
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            MainWindow fenetre = (MainWindow)Window.GetWindow(this);

            // On compare avec les variables de la fenêtre
            if (e.Key == fenetre.ToucheHaut)
            {
                allerHaut = false;
            }
            else if (e.Key == fenetre.ToucheBas)
            {
                allerBas = false;
            }
            else if (e.Key == fenetre.ToucheGauche)
            {
                allerGauche = false;
            }
            else if (e.Key == fenetre.ToucheDroite)
            {
                allerDroite = false;
            }
        }
       

        private void DemarrerPartie()
        {
            // 1. Nettoyage
            GameCanvas.Children.Clear();
            listeEnnemis.Clear();
            listeProjectiles.Clear();
            pointsVieDonjon = 100;

           
            // On récupère la fenêtre principale pour lire le niveau choisi
            MainWindow fenetre = (MainWindow)Window.GetWindow(this);

            if (fenetre != null)
            {
                // On récupère la variable : 0 (Facile), 1 (Moyen) ou 2 (Difficile)
                int niveau = fenetre.NiveauDifficulte;

                // On règle la vitesse des ennemis en conséquence
                if (niveau == 0) vitesseEnnemiActuelle = 2; // Facile : lent
                else if (niveau == 1) vitesseEnnemiActuelle = 4; // Moyen : rapide
                else vitesseEnnemiActuelle = 6; // Difficile : très rapide
            }           

            // 2. Réinitialisation de l'affichage        

            panelVie = new StackPanel { Orientation = Orientation.Horizontal };

            for (int i = 0; i < 5; i++)
            {
                Image coeur = new Image { Width = 30, Height = 30 };
                // Assure-toi que "vie.png" est bien dans le dossier RESSOURCES
                coeur.Source = new BitmapImage(new Uri("pack://application:,,,/RESSOURCES/vie.png"));

                panelVie.Children.Add(coeur);
            }

            // On place le tout en haut à gauche
            Canvas.SetLeft(panelVie, 10);
            Canvas.SetTop(panelVie, 10);
            GameCanvas.Children.Add(panelVie);

            // 3. Calcul du centre
            // Attention : Au tout premier lancement, ActualWidth peut être 0.
            // Petite sécurité pour éviter que tout apparaisse dans le coin en haut à gauche
            double centreX = (this.ActualWidth > 0) ? this.ActualWidth / 2 : 400;
            double centreY = (this.ActualHeight > 0) ? this.ActualHeight / 2 : 225;

            // 4. Création du Donjon
            formeDonjon = new Rectangle { Width = 100, Height = 100, Fill = Brushes.Transparent, Stroke = Brushes.Transparent };
            Canvas.SetLeft(formeDonjon, centreX - 50);
            Canvas.SetTop(formeDonjon, centreY - 50);
            GameCanvas.Children.Add(formeDonjon);

            // 5. Création du Roi (Dragon)
            imageDragon = new Image();
            imageDragon.Width = 102;
            imageDragon.Height = 102;

            MettreAJourSprite(); // Charge l'image initiale

            Canvas.SetLeft(imageDragon, centreX);
            Canvas.SetTop(imageDragon, centreY + 50);
            GameCanvas.Children.Add(imageDragon);

            // 6. Lancement du jeu
            minuteurJeu.Start();
        }
        private void MettreAJourCoeurs()
        {
            // 100 PV divisé par 20 = 5 coeurs. 
            // Si on a 80 PV, ça fait 4.
            int nombreCoeursVisibles = pointsVieDonjon / 20;

            for (int i = 0; i < panelVie.Children.Count; i++)
            {
                // Si le coeur est dans la limite des PV restants, on l'affiche, sinon on le cache
                if (i < nombreCoeursVisibles)
                {
                    panelVie.Children[i].Visibility = Visibility.Visible;
                }
                else
                {
                    panelVie.Children[i].Visibility = Visibility.Hidden;
                }
            }
        }
        private void MettreAJourSprite()
        {
            try
            {
                // On construit le nom : "dragon_" + "haut" + "1" + ".png"
                // Assure-toi que tes images sont bien en .png ou .jpg
                string cheminImage = $"pack://application:,,,/RESSOURCES/DRAGON/dragon_{direction}{numeroImage}.png";

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(cheminImage);
                bitmap.EndInit();

                imageDragon.Source = bitmap;
            }
            catch
            {
                // Si l'image n'est pas trouvée, ça évite le crash
            }
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
            double posX = Canvas.GetLeft(imageDragon); // On utilise imageDragon
            double posY = Canvas.GetTop(imageDragon);

            bool enMouvement = false;

            // Détection du mouvement et de la direction
            if (allerHaut && posY > 0)
            {
                posY -= vitesseRoi;
                direction = "haut";
                enMouvement = true;
            }
            else if (allerBas && posY < this.ActualHeight - imageDragon.Height)
            {
                posY += vitesseRoi;
                direction = "face";
                enMouvement = true;
            }

            // Note : j'utilise 'else if' pour éviter les diagonales bizarres, 
            // mais tu peux garder des 'if' simples si tu préfères.
            if (allerGauche && posX > 0)
            {
                posX -= vitesseRoi;
                direction = "gauche";
                enMouvement = true;
            }
            else if (allerDroite && posX < this.ActualWidth - imageDragon.Width)
            {
                posX += vitesseRoi;
                direction = "droite";
                enMouvement = true;
            }

            // Gestion de l'animation
            if (enMouvement)
            {
                compteurAnimation++;

                // On change d'image tous les 5 "tours" de boucle (pour ralentir l'animation)
                // Sinon ça clignote trop vite
                if (compteurAnimation > 5)
                {
                    numeroImage++;
                    if (numeroImage > 3) numeroImage = 1; // Retour à 1 après 3

                    MettreAJourSprite(); // On charge la nouvelle image
                    compteurAnimation = 0; // Reset du compteur
                }
            }
            else
            {
                // Optionnel : Si le joueur s'arrête, on remet l'image 1 (position statique)
                if (numeroImage != 1)
                {
                    numeroImage = 1;
                    MettreAJourSprite();
                }
            }

            Canvas.SetLeft(imageDragon, posX);
            Canvas.SetTop(imageDragon, posY);
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
                    // 1. On retire 20 PV (pour qu'un coeur entier disparaisse d'un coup)
                    pointsVieDonjon -= 10;

                    // 2. Au lieu de changer le texte, on met à jour les images
                    MettreAJourCoeurs();

                    // Le reste ne change pas
                    GameCanvas.Children.Remove(ennemiCourant);
                    listeEnnemis.RemoveAt(i);

                    if (pointsVieDonjon <= 0) DeclencherFinDePartie();
                }
            }
        }

        private void CalculerMouvementEnnemi(Ellipse ennemi)
        {
            double vitesseEnnemi = vitesseEnnemiActuelle;
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

            double departX = Canvas.GetLeft(imageDragon) + 25; // +25 car l'image fait 50 de large (pour centrer)
            double departY = Canvas.GetTop(imageDragon) + 25;

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
            UCGameOver uc = new UCGameOver();
            ((MainWindow)Application.Current.MainWindow).AireJeu.Content = uc;
            estJeuEnCours = false;
            DemarrerPartie();
         
        }  
    }
}

