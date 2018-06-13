using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;

namespace VisualisationHeuristique.Tools
{
    /// <summary>
    /// Classe modélisant un arc entre 2 noeuds
    /// </summary>
    class CustomEdge
    {
        public CustomNode source { get; set; }
        public CustomNode dest { get; set; }

        /// <summary>
        /// Nom de l'action effectué par l'arc
        /// </summary>
        public string name { get; set; }

        public bool inSelectedPath()
        {
            return source.in_selected_path && dest.in_selected_path;
        }

        /// <summary>
        /// Indique si un arcs a vraiment été utilisé
        /// C'est à dire si sa source et destination ont été visités
        /// </summary>
        /// <returns></returns>
        public bool actuallyTaken()
        {
            return source.visited && dest.visited;
        }


        /// <summary>
        /// Fonction permettant de styliser les arcs MSAGL en fonction des attributs de l'arc
        /// </summary>
        /// <param name="edge">Arc MSAGL dont l'on veut changer le style</param>
        public void styleEdge(Edge edge)
        {
            defaultStyleEdge(edge);

            if (this.mergedEdge())
            {
                styleEdgeMerged(edge);
            }
            else
            {
                styleEdgeClassic(edge);
            }
        }

        /// <summary>
        /// Style par défault commun à tous les arcs
        /// </summary>
        /// <param name="edge">Arc MSAGL dont l'on veut changer le style</param>
        private void defaultStyleEdge(Edge edge)
        {
            edge.Attr.LineWidth = 0.1;
            edge.Attr.ArrowheadLength = 1;
        }

        /// <summary>
        /// Style classique pour les arcs du graphe
        /// </summary>
        /// <param name="edge">Arc MSAGL dont l'on veut changer le style</param>
        private void styleEdgeClassic(Edge edge)
        {
            if (this.inSelectedPath())
            {
                edge.Attr.LineWidth = 1.5;
                edge.Attr.Color = Color.Red;
                //edge.LabelText = (dest.heuristic_value - source.heuristic_value).ToString();
            }

            if (this.actuallyTaken())
            {
                edge.LabelText = this.name;
            }
        }


        /*
         *  Les attributs et méthodes ci-dessous ne sont utilisé quand dans le cas de la fusion entre 2 graphes
         */

        /// <summary>
        /// Style pour un arc d'un graphe fusionné depuis 2 graphes
        /// </summary>
        /// <param name="edge">Arc MSAGL dont l'on veut changer le style</param>
        private void styleEdgeMerged(Edge edge)
        {
            if (takenAnyGraph())
            {
                edge.LabelText = this.name;
            }

            if(selectedPathAnyGraph())
            {
                edge.Attr.LineWidth = 1.5;
            }

            if(selectedPathBothGraph())
            {
                edge.Attr.Color = Color.Red;
            }
            else if(inSelectedPathSecond())
            {
                edge.Attr.Color = Color.Blue;
            }
            else
            {
                edge.Attr.Color = Color.Green;
            }
        }

        /// <summary>
        /// Booléen qui indique si cet arc est présent dans le premier graphe
        /// </summary>
        public bool inFirstGraph { get; set; }

        /// <summary>
        /// Booléen qui indique si cet arc est présent dans le deuxième graphe
        /// </summary>
        public bool inSecondGraph { get; set; }

        public bool inBothGraph()
        {
            return inFirstGraph && inSecondGraph;
        }

        public bool mergedEdge()
        {
            return inFirstGraph || inSecondGraph;
        }

        /// <summary>
        /// Indique si l'arc a été parcouru par n'importe quel graphe
        /// </summary>
        /// <returns></returns>
        public bool takenAnyGraph()
        {
            return (source.visited && dest.visited) || (source.visited_second && dest.visited_second);
        }

        /// <summary>
        /// Indique si l'arc est dans le chemin final de n'importe quel graphe
        /// </summary>
        /// <returns></returns>
        public bool selectedPathAnyGraph()
        {
            return (source.in_selected_path && dest.in_selected_path) || (source.in_selected_path_second && dest.in_selected_path_second);
        }

        public bool selectedPathBothGraph()
        {
            return inBothGraph() &&  source.in_selected_path && dest.in_selected_path 
                && source.in_selected_path_second && dest.in_selected_path_second;
        }

        public bool inSelectedPathSecond()
        {
            return source.in_selected_path_second && dest.in_selected_path_second && inSecondGraph;
        }
    }
}
