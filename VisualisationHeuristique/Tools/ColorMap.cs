using Microsoft.Msagl.Drawing;

namespace VisualisationHeuristique.Tools
{   
    /// <summary>
    /// Classe permettant de générer des colormap à partir des valeurs d'heuristique
    /// </summary>
    internal class ColorMap
    {
        private Color start;
        private Color end;
        private double heuristic_max;
        private double heuristic_min;
        private double diff_heuristic;


        /// <summary>
        ///  Constructeur
        /// </summary>
        /// <param name="start">Couleur initiale de la color map</param>
        /// <param name="end">Couleur cible de la color map</param>
        /// <param name="heuristic_max">Valeur maximale de l'heuristique dans le graphe</param>
        /// <param name="heuristic_min">Valeur minimale de l'heuristique dans le graphe</param>
        public ColorMap(Color start, Color end, double heuristic_max, double heuristic_min)
        {
            this.start = start;
            this.end = end;
            this.heuristic_max = heuristic_max;
            this.heuristic_min = heuristic_min;
            diff_heuristic = heuristic_max - heuristic_min;
        }


        /// <summary>
        ///  Retourne la couleur associé à la valeur de l'heuristique données
        /// </summary>
        /// <param name="heuristic_value">Valeur cible</param>
        /// <returns>Couleur du noeud</returns>
        public Color getColor(double heuristic_value)
        {
            double proche_debut = (heuristic_value - heuristic_min) / diff_heuristic;
            double proche_fin = (heuristic_max - heuristic_value) / diff_heuristic;

            byte r = (byte)(start.R * proche_debut + end.R * proche_fin);
            byte g = (byte)(start.G * proche_debut + end.G * proche_fin);
            byte b = (byte)(start.B * proche_debut + end.B * proche_fin);
            byte a = (byte)(start.A * proche_debut + end.A * proche_fin);

            Color c = new Color(a, r, g, b);

            return c;
        }
    }
}
