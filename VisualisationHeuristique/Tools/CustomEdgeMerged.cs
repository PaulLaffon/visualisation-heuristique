using Microsoft.Msagl.Drawing;

namespace VisualisationHeuristique.Tools
{

    /// <summary>
    /// Classe qui hérite des arcs classique 
    /// Implémente les arcs issuent de la fusion entre 2 graphes
    /// </summary>
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
        /// <param name="edge_name">Booleen qui indique si on doit afficher le nom de l'arc</param>
        protected override void styleEdgeClassic(Edge edge, bool edge_name)
        {
            if (takenAnyGraph() && edge_name)
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
            else if(inSelectedPathFirst())
            {
                edge.Attr.Color = Color.Green;
            }
            else if(inBothGraph())
            {
                edge.Attr.Color = Color.Red;
            }
            else if(inSecondGraph)
            {
                edge.Attr.Color = Color.Blue;
            }
            else if(inFirstGraph)
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

            return (merged_source.in_selected_path && merged_dest.in_selected_path && inFirstGraph) || 
                (merged_source.in_selected_path_second && merged_dest.in_selected_path_second && inSecondGraph);
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

        /// <summary>
        /// Indique si le graphe est dans le chemin choisi du 1er graphe
        /// </summary>
        /// <returns></returns>
        protected bool inSelectedPathFirst()
        {
            CustomNodeMerged merged_source = (CustomNodeMerged)source;
            CustomNodeMerged merged_dest = (CustomNodeMerged)dest;

            return merged_source.in_selected_path && merged_dest.in_selected_path && inFirstGraph;
        }
    }
}
