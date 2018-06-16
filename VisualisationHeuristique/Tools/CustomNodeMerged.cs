using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualisationHeuristique.Tools
{
    /// <summary>
    /// Classe qui correspond à un noeuds d'un graphe issu de la fusion de 2 graphes
    /// </summary>
    class CustomNodeMerged : CustomNode
    {
        public bool in_first_graph { get; set; }
        public bool in_second_graph { get; set; }

        public bool visited_second { get; set; }
        public bool in_selected_path_second { get; set; }
        public float heuristic_value_second { get; set; }

        public int order_visited_second { get; set; }
        public int order_discovered_second { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="id">Id du noeuds à créer</param>
        public CustomNodeMerged(string id) : base(id)
        {
        }

        /// <summary>
        /// Initialize un noeuds de fusion par rapport à un neuds du premier au second graphe
        /// </summary>
        /// <param name="node">Noeuds du graphe</param>
        /// <param name="second_graph">Booleen qui indique si c'est un noeuds du second graphe</param>
        public void initFromNode(CustomNode node, bool second_graph)
        {
            real_final_value = node.real_final_value;

            if (!second_graph)
            {
                in_first_graph = true;
                visited = node.visited;
                in_selected_path = node.in_selected_path;
                heuristic_value = node.heuristic_value;
                order_visited = node.order_visited;
                order_discovered = node.order_discovered;
            }
            else
            {
                in_second_graph = true;
                visited_second = node.visited;
                in_selected_path_second = node.in_selected_path;
                heuristic_value_second = node.heuristic_value;
                order_discovered_second = node.order_discovered;
                order_visited_second = node.order_visited;
            }
        }

        /// <summary>
        /// Surcharge la méthode de visite
        /// Renvoi vrai si le noeuds à été visité dans au moins un graphe des 2 graphes de fusion
        /// </summary>
        /// <returns>Booleen</returns>
        public override bool isVisited()
        {
            return visited || visited_second;
        }

        /// <summary>
        /// Surchage de la méthode mère
        /// Renvoi vrai si le noeuds est dans au moins un chemin choisi parmi les 2
        /// </summary>
        /// <returns></returns>
        public override bool inSelectedPath()
        {
            return in_selected_path || in_selected_path_second;
        }

        /// <summary>
        ///  Si le noeuds à été visité par les 2 graphes de fusion
        /// </summary>
        /// <returns>Booleen</returns>
        private bool isVisitedBoth()
        {
            return visited && visited_second;
        }


        /// <summary>
        /// Surcharge la fonction de style classic d'un noeuds
        /// L'affichage d'un noeuds de fusion est différent
        /// </summary>
        /// <param name="node">Noeuds MSAGL à styliser</param>
        protected override void styleNodeClassic(Node node)
        {
            node.Attr.Tooltip = getTooltip();

            if (this.isVisited())
            {
                if (isVisitedBoth())
                {
                    node.LabelText = order_visited.ToString() + "-" + order_visited_second.ToString();
                }
                else if (visited)
                {
                    node.LabelText = order_visited.ToString();
                }
                else
                {
                    node.LabelText = order_visited_second.ToString();
                }
            }
        }

        /// <summary>
        /// Surcharge la Tooltip d'un noeuds de fusion
        /// </summary>
        /// <returns>String à afficher dans la tooltip</returns>
        protected override string getTooltip()
        {
            string tooltip = "";
            tooltip += "Node id : " + id;

            tooltip += "\n";

            if (visited)
            {
                tooltip += "\n" + "Heuristique value 1 : " + heuristic_value.ToString();
                tooltip += "\n" + "Ordre visite 1 : " + order_visited.ToString();
            }
            else
            {
                tooltip += "\n" + "Non visité graphe 1";
            }

            tooltip += "\n";

            if (visited_second)
            {
                tooltip += "\n" + "Heuristique value 2 : " + heuristic_value_second.ToString();
                tooltip += "\n" + "Ordre visite 2 : " + order_visited_second.ToString();
            }
            else
            {
                tooltip += "\n" + "Non visité graphe 2";
            }


            return tooltip;
        }

    }
}
