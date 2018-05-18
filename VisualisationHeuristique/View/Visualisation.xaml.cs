using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using System;
using System.Windows;

namespace VisualisationHeuristique
{
    /// <summary>
    /// Interaction logic for Visualisation.xaml
    /// </summary>
    public partial class Visualisation : Window
    {
        private GraphViewer viewer;



        public Visualisation()
        {
            InitializeComponent();

            viewer = new GraphViewer();
            viewer.RunLayoutAsync = true;
       

            viewer.BindToPanel(this.grapheContainer);

            
        }

        private void grapheContainer_Loaded(object sender, RoutedEventArgs e)
        {
            Graph g = new Graph();

            g = JsonGraphProvider.GraphFromFile("traces_serieux_h3.json");

            Console.WriteLine("JSON fini !!!");

            viewer.Graph = g;
        }
    }
}
