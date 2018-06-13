using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VisualisationHeuristique.Tools
{
    /// <summary>
    /// Implemente la structure d'un noeud de CustomGraph
    /// </summary>
    class CustomNode
    {
        public string id { get; private set; }

        public bool visited { get; set; }
        public bool in_selected_path { get; set; }
        public float heuristic_value { get; set; }
        public float real_final_value { get; set; }

        public int order_visited { get; set; }
        public int order_discovered { get; set; }



        public Dictionary<string, CustomEdge> successors { get; set; }
        public Dictionary<string, CustomEdge> predecessors { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="id">Id du noeud à construire</param>
        public CustomNode(string id)
        {
            this.id = id;

            successors = new Dictionary<string, CustomEdge>();
            predecessors = new Dictionary<string, CustomEdge>();
        }


        /// <summary>
        /// Compte récursivement le nombre de fils de ce noeuds
        /// </summary>
        /// <returns>Nombre de successeur du noeuds</returns>
        private int getNumberChildren()
        {
            int childs = 0;
            
            foreach(CustomEdge edge in successors.Values)
            {
                childs += edge.dest.getNumberChildren() + 1;
            }

            return childs;
        }

        /// <summary>
        /// Recherche la valeur minimale de l'heuristique des noeuds fils
        /// </summary>
        /// <returns>Valeur minimale de l'heuristique</returns>
        private float childsMinHeuristic()
        {
            float heuristicMin = heuristic_value;

            foreach(CustomEdge edge in successors.Values)
            {
                if(edge.dest.visited)
                {
                    heuristicMin = Math.Min(edge.dest.childsMinHeuristic(), heuristicMin);
                }
            }

            return heuristicMin;
        }

        /// <summary>
        /// Recherche la valeur maximale de l'heurtisque des noeuds fils
        /// </summary>
        /// <returns></returns>
        private float childsMaxHeuristic()
        {
            float heuristicMax = heuristic_value;

            foreach (CustomEdge edge in successors.Values)
            {
                if (edge.dest.visited)
                {
                    heuristicMax = Math.Max(edge.dest.childsMaxHeuristic(), heuristicMax);
                }
            }

            return heuristicMax;
        }


        /// <summary>
        /// Style par default de tous les noeuds du graphe
        /// Une fois ce style par default appliqué, des surcharges peuvent être faites en fonction du type de noeuds
        /// </summary>
        /// <param name="node">Noeuds MSAGL correspondant au CustomNode dont on change le style</param>
        /// <param name="cmap">Colormap pour appliquer le stype au noeuds</param>
        private void defaultNodeStyle(Node node, ColorMap cmap)
        {
            node.Attr.Shape = Shape.Circle;
            node.Attr.LabelMargin = 0;
            node.Attr.LineWidth = 0.2;

            if(visited)
            {
                node.Attr.FillColor = cmap.getColor(heuristic_value);
            }
            else
            {
                node.Attr.FillColor = Color.White;
                node.LabelText = null;
            }
        }

        
        /// <summary>
        /// Style classique pour les noeuds du graphe
        /// </summary>
        /// <param name="node">Noeuds MSAGL à styliser</param>
        /// <param name="cmap"></param>
        public void styleNode(Node node, ColorMap cmap)
        {
            defaultNodeStyle(node, cmap);
            node.Attr.Tooltip = getTooltip();

            if (visited)
            {
                node.LabelText = order_visited.ToString();
            }
        }

        /// <summary>
        /// Style pour les noeuds grouper, ces noeuds contienent plusieurs noeuds
        /// </summary>
        /// <param name="node">Noeuds MSAGL à styliser</param>
        /// <param name="cmap"></param>
        /// <param name="nodeSize">Nombre de noeuds dans ce noeuds grouper</param>
        public void styleNodeGrouped(Node node, ColorMap cmap)
        {
            int nodeSize = getNumberChildren() + 1;

            defaultNodeStyle(node, cmap);

            string groupedTooltip = "Nombre de noeuds : " + nodeSize.ToString();
            groupedTooltip += "\nHeuristique max : " + childsMaxHeuristic().ToString();
            groupedTooltip += "\nHeuristique min : " + childsMinHeuristic().ToString();

            node.Attr.Tooltip = groupedTooltip;

            node.LabelText = "";
            node.Attr.LabelMargin = (int)Math.Pow(nodeSize, 0.66) + 3;
        }

        /// <summary>
        /// Renvoie un texte décrivant le noeuds afin d'être utilisé dans le tooltip 
        /// </summary>
        /// <returns>string décrivant le noeud</returns>
        private string getTooltip()
        {
            string tooltip = "";

            tooltip += "Node id : " + id;

            if(visited)
            {
                tooltip += "\n" + "Heuristique value : " + heuristic_value.ToString();
            }

            tooltip += "\n" + "Ordre de découverte : " + order_discovered.ToString();

            return tooltip;
        }



        /// <summary>
        /// Les attributs suivants ne sont utilisé que dans le cas d'une fusion entre 2 graphes
        /// </summary>
        public bool in_first_graph { get; set; }
        public bool in_second_graph { get; set; }

        public bool visited_second { get; set; }
        public bool in_selected_path_second { get; set; }
        public float heuristic_value_second { get; set; }

        public int order_visited_second { get; set; }
        public int order_discovered_second { get; set; }
    }
}
