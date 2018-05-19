using Microsoft.Msagl.Drawing;
using System.Collections.Generic;

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

        public int order_visited { get; set; }
        public int order_discovered { get; set; }
        

        public Dictionary<string, CustomNode> successors { get; set; }
        public Dictionary<string, CustomNode> predecessors { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="id">Id du noeud à construire</param>
        public CustomNode(string id)
        {
            this.id = id;

            successors = new Dictionary<string, CustomNode>();
            predecessors = new Dictionary<string, CustomNode>();
        }


        /// <summary>
        /// Change le style d'un noeud MSAGL en fonction des différents attributs du noeud en mémoire
        /// </summary>
        /// <param name="node">Noeud MSAGL correspondant au CustomNode</param>
        public void style_node(Node node)
        {
            node.Attr.Shape = Shape.Circle;
            node.Attr.LabelMargin = 0;
            node.Attr.LineWidth = 0.2;
            node.Attr.Tooltip = "Patate";

            if (visited)
            {
                node.Attr.FillColor = Color.Blue;
                node.LabelText = order_visited.ToString();
            }
            else
            {
                node.Attr.FillColor = Color.White;
                node.LabelText = null;
            }
        }
    }
}
