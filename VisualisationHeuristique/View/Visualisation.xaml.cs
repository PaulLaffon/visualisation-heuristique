using Microsoft.Msagl.WpfGraphControl;
using System.ComponentModel;
using System.Windows;
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

            allNodes.IsChecked = true;
            oneGraph.IsChecked = true;

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
            // Si on ne veut afficher que un seul graphe 
            if((bool)oneGraph.IsChecked)
            {
                SecondaryVisible = false;
                CustomGraph graphe = JsonGraphProvider.loadGraphFromFile(folderName + "\\" + selectedMainFile.SelectedItem.ToString());
                viewer1.Graph = graphe.getVisualGraph((bool)visitedNodes.IsChecked);
            }
            else if((bool)twoInOne.IsChecked) // Si on veut afficher deux graphe dans la même vue
            {
                SecondaryVisible = false;
                CustomGraph graphe1 = JsonGraphProvider.loadGraphFromFile(folderName + "\\" + selectedMainFile.SelectedItem.ToString());
                CustomGraph graphe2 = JsonGraphProvider.loadGraphFromFile(folderName + "\\" + selectedSecondaryFile.SelectedItem.ToString());

                viewer1.Graph = graphe1.merge(graphe2).getVisualGraph((bool)visitedNodes.IsChecked);
            }
            else if((bool)twoGraphs.IsChecked) // Si on veut afficher 2 graphes cote à cote
            {
                SecondaryVisible = true;
                CustomGraph graphe1 = JsonGraphProvider.loadGraphFromFile(folderName + "\\" + selectedMainFile.SelectedItem.ToString());
                CustomGraph graphe2 = JsonGraphProvider.loadGraphFromFile(folderName + "\\" + selectedSecondaryFile.SelectedItem.ToString());

                viewer1.Graph = graphe1.getVisualGraph((bool)visitedNodes.IsChecked);
                viewer2.Graph = graphe2.getVisualGraph((bool)visitedNodes.IsChecked);
            }
        }
    }
}
