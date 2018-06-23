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

        protected virtual bool inSelectedPath()
        {
            return source.in_selected_path && dest.in_selected_path;
        }

        /// <summary>
        /// Indique si un arcs a vraiment été utilisé
        /// C'est à dire si sa source et destination ont été visités
        /// </summary>
        /// <returns>Booleen</returns>
        protected bool actuallyTaken()
        {
            return source.visited && dest.visited;
        }


        /// <summary>
        /// Fonction permettant de styliser les arcs MSAGL en fonction des attributs de l'arc
        /// </summary>
        /// <param name="edge">Arc MSAGL dont l'on veut changer le style</param>
        public void styleEdge(Edge edge, bool edge_name)
        {
            defaultStyleEdge(edge);

            styleEdgeClassic(edge, edge_name);
        }

        /// <summary>
        /// Style par défault commun à tous les arcs
        /// </summary>
        /// <param name="edge">Arc MSAGL dont l'on veut changer le style</param>
        protected void defaultStyleEdge(Edge edge)
        {
            edge.Attr.LineWidth = 0.1;
            edge.Attr.ArrowheadLength = 1;
        }

        /// <summary>
        /// Style classique pour les arcs du graphe
        /// </summary>
        /// <param name="edge">Arc MSAGL dont l'on veut changer le style</param>
        /// <param name="edge_name">Boolean qui indique si on doit afficher le nom de l'arc</param>
        protected virtual void styleEdgeClassic(Edge edge, bool edge_name)
        {
            if (this.inSelectedPath())
            {
                edge.Attr.LineWidth = 1.5;
                edge.Attr.Color = Color.Red;
            }

            if (this.actuallyTaken() && edge_name)
            {
                edge.LabelText = this.name;
            }
        }
    }
}
