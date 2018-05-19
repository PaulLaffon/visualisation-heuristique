using Microsoft.Msagl.Drawing;
using System.Collections.Generic;

namespace VisualisationHeuristique.Tools
{
    /// <summary>
    /// Implemente une structure de graphe afin de faciliter les opérations sur le graphe
    /// </summary>
    class CustomGraph
    {
        public Dictionary<string, CustomNode> nodes { get; }

        /// <summary>
        /// Constructeur
        /// </summary>
        public CustomGraph()
        {
            nodes = new Dictionary<string, CustomNode>();
        }
        
        /// <summary>
        /// Ajoute un arc au graphe
        /// Ajoute les noeuds de départ et d'arrivé s'ils ne sont pas présent dans le graphe
        /// </summary>
        /// <param name="source_id">Id du noeud de départ</param>
        /// <param name="dest_id">Id du noeud de destination</param>
        public void addEdge(string source_id, string dest_id, int order_discovered)
        {
            if(!nodes.ContainsKey(source_id))
            {
                nodes.Add(source_id, new CustomNode(source_id));
            }

            if(!nodes.ContainsKey(dest_id))
            {
                nodes.Add(dest_id, new CustomNode(dest_id));
            }

            CustomNode source = nodes[source_id];
            CustomNode dest = nodes[dest_id];

            dest.order_discovered = order_discovered;

            source.successors.Add(dest_id, dest);
            dest.predecessors.Add(source_id, source);
        }

        /// <summary>
        /// Retourne un Graph MSAGL qui peut être qui peut être affichier dans la vue
        /// </summary>
        /// <param name="only_visited">Inclure seulement les noeuds visité dans le visuel</param>
        /// <returns>Graph MSAGL pouvant être afficher dans la vue</returns>
        public Graph getVisualGraph(bool only_visited = false)
        {
            Graph graph = new Graph();

            foreach(CustomNode source in nodes.Values)
            {
                foreach(CustomNode dest in source.successors.Values)
                {
                    Edge msagl_edge = graph.AddEdge(source.id, dest.id);

                    Node msagl_source_node = msagl_edge.SourceNode;
                    Node msagl_dest_node = msagl_edge.TargetNode;

                    // On change le style des noeuds de départ et d'arrivée
                    source.style_node(msagl_source_node);
                    dest.style_node(msagl_dest_node);

                    // On change le style de l'arc en fonction des noeuds de départ et d'arrivée
                    style_edge(source, dest, msagl_edge);
                }
            }

            return graph;
        }

        /// <summary>
        /// Change le style d'un arc MSAGL en fonction des deux noeuds qu'il relie
        /// </summary>
        /// <param name="source">Noeud de départ de l'arc</param>
        /// <param name="dest">Noeud de destination de l'arc</param>
        /// <param name="edge">Arc MSAGL dont l'on veut changer le style</param>
        private void style_edge(CustomNode source, CustomNode dest, Edge edge)
        {
            edge.Attr.LineWidth = 0.1;
            edge.Attr.ArrowheadLength = 1;

            // Si les 2 noeuds de l'arc font parti du chemin final
            if(source.in_selected_path && dest.in_selected_path)
            {
                edge.Attr.LineWidth = 1.5;
                edge.Attr.Color = Color.Red;
            }
        }
    }
}
