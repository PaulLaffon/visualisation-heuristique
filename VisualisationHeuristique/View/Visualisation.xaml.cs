using Microsoft.Msagl.WpfGraphControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VisualisationHeuristique.Tools;

namespace VisualisationHeuristique
{
    /// <summary>
    /// Interaction logic for Visualisation.xaml
    /// </summary>
    public partial class Visualisation : Window
    {
        private GraphViewer viewer;
        private List<string> optionsTypeAffichage;

        private readonly string folderName = "traces";

        private CustomGraph graphe;

        /// <summary>
        /// Constructeur
        /// </summary>
        public Visualisation()
        {
            InitializeComponent();

            viewer = new GraphViewer();
            viewer.RunLayoutAsync = true;
            
            viewer.BindToPanel(this.grapheContainer);
        }

        /// <summary>
        /// Fonction appelée quand la vue qui va contenir le graphe est chargée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grapheContainer_Loaded(object sender, RoutedEventArgs e)
        {
            selectFile.ItemsSource = JsonGraphProvider.getJsonFileFromFolder(folderName);
            selectFile.SelectedIndex = 0;

            optionsTypeAffichage = new List<string>() { "Total", "Visité" };
            typeAffichage.ItemsSource = optionsTypeAffichage;
            typeAffichage.SelectedIndex = 0; // Total est selectionné par défault
        }


        /// <summary>
        /// Fonction appelée quand la valeur du combo box qui gère le type d'affichage change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void typeAffichage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (typeAffichage.SelectedIndex == 0)
            {
                viewer.Graph = graphe.getVisualGraph(false);
            }
            else if(typeAffichage.SelectedIndex == 1)
            {
                viewer.Graph = graphe.getVisualGraph(true);
            }
        }

        /// <summary>
        /// Fonction appelée quand le valeur du combo box qui gère le fichier change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            graphe = JsonGraphProvider.loadGraphFromFile(folderName + "\\" + selectFile.SelectedItem.ToString());

            typeAffichage_SelectionChanged(null, null);
        }
    }
}
