using Microsoft.Msagl.Drawing;

namespace VisualisationHeuristique.Tools
{
    class CustomEdgeMerged : CustomEdge
    {

        /// <summary>
        /// Booléen qui indique si cet arc est présent dans le premier graphe
        /// </summary>
        public bool inFirstGraph { get; set; }

        /// <summary>
        /// Booléen qui indique si cet arc est présent dans le deuxième graphe
        /// </summary>
        public bool inSecondGraph { get; set; }



        /// <summary>
        /// Style pour un arc d'un graphe fusionné depuis 2 graphes
        /// </summary>
        /// <param name="edge">Arc MSAGL dont l'on veut changer le style</param>
        protected override void styleEdgeClassic(Edge edge)
        {
            if (takenAnyGraph())
            {
                edge.LabelText = this.name;
            }

            if (inSelectedPath())
            {
                edge.Attr.LineWidth = 1.5;
            }

            if (selectedPathBothGraph())
            {
                edge.Attr.Color = Color.Red;
            }
            else if (inSelectedPathSecond())
            {
                edge.Attr.Color = Color.Blue;
            }
            else
            {
                edge.Attr.Color = Color.Green;
            }
        }

        /// <summary>
        /// Indique si cet arc est présent dans les 2 graphes
        /// </summary>
        /// <returns>Booleen</returns>
        protected bool inBothGraph()
        {
            return inFirstGraph && inSecondGraph;
        }

        /// <summary>
        /// Indique si l'arc a été parcouru par n'importe quel graphe
        /// </summary>
        /// <returns>Booleen</returns>
        protected bool takenAnyGraph()
        {
            return source.isVisited() && dest.isVisited();
        }

        /// <summary>
        /// Indique si l'arc est dans le chemin final de n'importe quel graphe
        /// </summary>
        /// <returns>Booleen</returns>
        protected override bool inSelectedPath()
        {
            CustomNodeMerged merged_source = (CustomNodeMerged)source;
            CustomNodeMerged merged_dest = (CustomNodeMerged)dest;

            return (merged_source.in_selected_path && merged_dest.in_selected_path) || 
                (merged_source.in_selected_path_second && merged_dest.in_selected_path_second);
        }

        /// <summary>
        /// Indique si cet arc est dans le chemin choisi des 2 graphes
        /// </summary>
        /// <returns>Booleen</returns>
        protected bool selectedPathBothGraph()
        {
            CustomNodeMerged merged_source = (CustomNodeMerged)source;
            CustomNodeMerged merged_dest = (CustomNodeMerged)dest;

            return inBothGraph() && merged_source.in_selected_path && merged_dest.in_selected_path
                && merged_source.in_selected_path_second && merged_dest.in_selected_path_second;
        }

        /// <summary>
        /// Indique si le graphe est dans le chemin choisi du 2ème graphe
        /// </summary>
        /// <returns></returns>
        protected bool inSelectedPathSecond()
        {
            CustomNodeMerged merged_source = (CustomNodeMerged)source;
            CustomNodeMerged merged_dest = (CustomNodeMerged)dest;

            return merged_source.in_selected_path_second && merged_dest.in_selected_path_second && inSecondGraph;
        }
    }
}
