using Microsoft.Msagl.WpfGraphControl;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using VisualisationHeuristique.Tools;

namespace VisualisationHeuristique
{
    /// <summary>
    /// Interaction logic for Visualisation.xaml
    /// </summary>
    public partial class Visualisation : Window, INotifyPropertyChanged
    {
        private GraphViewer viewer1;
        private GraphViewer viewer2;

        private readonly string folderName = "traces";

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private bool secondaryVisible = false;
        public bool SecondaryVisible {
            get { return secondaryVisible; }
            private set { secondaryVisible = value; OnPropertyChanged("SecondaryVisible"); }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public Visualisation()
        {
            this.DataContext = this;
            InitializeComponent();

            viewer1 = new GraphViewer();
            viewer1.RunLayoutAsync = true;
            
            viewer1.BindToPanel(this.grapheContainer1);

            viewer2 = new GraphViewer();
            viewer2.RunLayoutAsync = true;
            viewer2.BindToPanel(this.grapheContainer2);
        }

        /// <summary>
        /// Fonction appelée quand la vue qui va contenir le graphe principal est chargée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grapheContainer_Loaded(object sender, RoutedEventArgs e)
        {
            selectedMainFile.ItemsSource = JsonGraphProvider.getJsonFileFromFolder(folderName);
            selectedMainFile.SelectedIndex = 0;

            selectedSecondaryFile.ItemsSource = JsonGraphProvider.getJsonFileFromFolder(folderName);
            selectedSecondaryFile.SelectedIndex = 0;

            // Les options sélectionnées par défauts
            oneGraph.IsChecked = true;

            mainGraphGroupNodes.IsChecked = true;
            mainGraphVisitedNodes.IsChecked = true;

            secondGraphGroupNodes.IsChecked = true;
            secondGraphVisitedNodes.IsChecked = true;

            secondGraph.Visibility = Visibility.Collapsed;

            selectedMainFile.Background = Brushes.Black;

            // Calculer le layout du graphe au lancement de l'application
            calculer_Clicked(null, null);
        }

        /// <summary>
        /// Fonction appelée quand on clique sur le bouton "Calculer"
        /// Recalcul le layout du ou des graphes avec les nouveaux paramètres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calculer_Clicked(object sender, RoutedEventArgs e)
        {
            SecondaryVisible = false;

            // Dans tous les cas on charge le graphe principal
            CustomGraph graphe1 = JsonGraphProvider.loadGraphFromFile(folderName + "\\" + selectedMainFile.SelectedItem.ToString());


            // Si on ne veut afficher que un seul graphe 
            if ((bool)oneGraph.IsChecked)
            {
                viewer1.Graph = graphe1.getVisualGraph((bool)mainGraphVisitedNodes.IsChecked, 
                                                       (bool)mainGraphGroupNodes.IsChecked, 
                                                       (bool)mainGraphEdgeName.IsChecked,
                                                       mainGraphGroupLevel.Value);
            }
            else if((bool)twoInOne.IsChecked) // Si on veut afficher deux graphe dans la même vue
            {
                CustomGraph graphe2 = JsonGraphProvider.loadGraphFromFile(folderName + "\\" + selectedSecondaryFile.SelectedItem.ToString());

                CustomGraphMerged merged_graph = new CustomGraphMerged(graphe1, graphe2);

                viewer1.Graph = merged_graph.getVisualGraph((bool)mainGraphVisitedNodes.IsChecked, 
                                                            (bool)mainGraphGroupNodes.IsChecked,
                                                            (bool)mainGraphEdgeName.IsChecked, 
                                                            mainGraphGroupLevel.Value);
            }
            else if((bool)twoGraphs.IsChecked) // Si on veut afficher 2 graphes cote à cote
            {
                SecondaryVisible = true;
                CustomGraph graphe2 = JsonGraphProvider.loadGraphFromFile(folderName + "\\" + selectedSecondaryFile.SelectedItem.ToString());

                viewer1.Graph = graphe1.getVisualGraph((bool)mainGraphVisitedNodes.IsChecked, 
                                                       (bool)mainGraphGroupNodes.IsChecked,
                                                       (bool)mainGraphEdgeName.IsChecked,
                                                       mainGraphGroupLevel.Value);

                viewer2.Graph = graphe2.getVisualGraph((bool)secondGraphVisitedNodes.IsChecked, 
                                                       (bool)secondGraphGroupNodes.IsChecked,
                                                       (bool)secondGraphEdgeName.IsChecked,
                                                       secondGraphhGroupLevel.Value);
            }
        }



        private void twoGraphs_Checked(object sender, RoutedEventArgs e)
        {
            secondGraph.Visibility = Visibility.Visible;
            radioSecond.Visibility = Visibility.Visible;
            principal.Foreground = Brushes.Black;
            secondaire.Foreground = Brushes.Black;
        }

        private void oneGraph_Checked(object sender, RoutedEventArgs e)
        {
            secondGraph.Visibility = Visibility.Collapsed;
            radioSecond.Visibility = Visibility.Collapsed;
            principal.Foreground = Brushes.Black;
            secondaire.Foreground = Brushes.Black;
        }

        private void twoInOne_Checked(object sender, RoutedEventArgs e)
        {
            secondGraph.Visibility = Visibility.Visible;
            radioSecond.Visibility = Visibility.Collapsed;
            principal.Foreground = Brushes.Green;
            secondaire.Foreground = Brushes.Blue;
        }

        private void mainGraphGroup_checked(object sender, RoutedEventArgs e)
        {
            if((bool)mainGraphGroupNodes.IsChecked)
                mainGraphGroupSelector.Visibility = Visibility.Visible;
            else
                mainGraphGroupSelector.Visibility = Visibility.Collapsed;
        }

        private void secondGraphGroup_checked(object sender, RoutedEventArgs e)
        {
            if ((bool)secondGraphGroupNodes.IsChecked)
                secondGraphGroupSelector.Visibility = Visibility.Visible;
            else
                secondGraphGroupSelector.Visibility = Visibility.Collapsed;
        }
    }
}
