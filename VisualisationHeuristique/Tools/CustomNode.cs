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
        /// Change le style d'un noeud MSAGL en fonction des différents attributs du noeud en mémoire
        /// </summary>
        /// <param name="node">Noeud MSAGL correspondant au CustomNode</param>
        public void styleNode(Node node, ColorMap cmap)
        {
            node.Attr.Shape = Shape.Circle;
            node.Attr.LabelMargin = 0;
            node.Attr.LineWidth = 0.2;
            node.Attr.Tooltip = getTooltip();

            if (visited)
            {
                node.Attr.FillColor = cmap.getColor(heuristic_value);
                node.LabelText = order_visited.ToString();
            }
            else
            {
                node.Attr.FillColor = Color.White;
                node.LabelText = null;
            }
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
            tooltip += "\n" + "Nombre de fils : " + successors.Count;
            tooltip += "\n" + "Nombre de pères : " + predecessors.Count;

            return tooltip;
        }
    }
}
