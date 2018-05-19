using Microsoft.Msagl.Drawing;
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
        private List<string> options;

        public Visualisation()
        {
            InitializeComponent();

            options = new List<string>() { "option1", "option2" };
            optionCombobox.ItemsSource = options;
            //optionCombobox.SelectedIndex = 0; -> en avoir un select de base

            viewer = new GraphViewer();
            viewer.RunLayoutAsync = true;
       
            viewer.BindToPanel(this.grapheContainer);
        }

        private void grapheContainer_Loaded(object sender, RoutedEventArgs e)
        {
            CustomGraph g = new CustomGraph();

            g = JsonGraphProvider.loadGraphFromFile("traces_simple.json");

            viewer.Graph = g.getVisualGraph();
        }

        private void optionCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine(optionCombobox.SelectedItem);

            Graph g = new Graph();
            g.AddEdge("Patate", "Pomme de Terre");

            viewer.Graph = g;
        }
    }
}
