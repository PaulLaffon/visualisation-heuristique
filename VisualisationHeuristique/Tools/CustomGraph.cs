using Microsoft.Msagl.Drawing;
using System.Collections.Generic;

namespace VisualisationHeuristique.Tools
{
    /// <summary>
    /// Implemente une structure de graphe afin de faciliter les opérations sur le graphe
    /// </summary>
    class CustomGraph
    {
        /// <summary>
        /// Tous les noeuds composant le graphe
        /// </summary>
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
        public void addEdge(string source_id, string dest_id, int order_discovered, string edge_name)
        {
            if(!nodes.ContainsKey(source_id))
            {
                nodes.Add(source_id, new CustomNode(source_id));
            }

            if(!nodes.ContainsKey(dest_id))
            {
                nodes.Add(dest_id, new CustomNode(dest_id));
            }
            else
            {
                return;
            }

            CustomNode source = nodes[source_id];
            CustomNode dest = nodes[dest_id];

            dest.order_discovered = order_discovered;

            source.successors.Add(dest_id, new CustomEdge() { name = edge_name, source = source, dest = dest });
            dest.predecessors.Add(source_id, new CustomEdge() { name = edge_name, source = source, dest = dest });
        }

        /// <summary>
        /// Ajoute un noeuds dans le cas de la fusion de 2 graphes
        /// </summary>
        /// <param name="source_id">Id du noeuds de départ</param>
        /// <param name="dest_id">Id du noeuds d'arrivé</param>
        /// <param name="edge_name">Nom de l'arc</param>
        private void addMergeEdge(string source_id, string dest_id, string edge_name)
        {
            if (!nodes.ContainsKey(source_id))
            {
                nodes.Add(source_id, new CustomNode(source_id));
            }

            if (!nodes.ContainsKey(dest_id))
            {
                nodes.Add(dest_id, new CustomNode(dest_id));
            }

            CustomNode source = nodes[source_id];
            CustomNode dest = nodes[dest_id];

            //dest.order_discovered = order_discovered;

            if (!source.successors.ContainsKey(dest_id))
            {
                source.successors.Add(dest_id, new CustomEdge() { name = edge_name, source = source, dest = dest });
            }
            if(!dest.predecessors.ContainsKey(source_id))
            {
                dest.predecessors.Add(source_id, new CustomEdge() { name = edge_name, source = source, dest = dest });
            }
        }

        /// <summary>
        /// Fusionne 2 graphe en un seul afin de pouvoir comparer ces deux graphes dans un seul
        /// </summary>
        /// <param name="graph">Graphe avec lequel fusionner</param>
        /// <returns>CustomGraph résultant de la fusion des 2 graphes</returns>
        public CustomGraph merge(CustomGraph graph)
        {
            CustomGraph merge_result = this;

            foreach(CustomNode source in graph.nodes.Values)
            {
                foreach(CustomEdge link in source.successors.Values)
                {
                    merge_result.addMergeEdge(link.source.id, link.dest.id, link.name);
                }
            }

            return merge_result;
        }


        /// <summary>
        /// Retourne un Graph MSAGL qui peut être qui peut être affichier dans la vue
        /// </summary>
        /// <param name="only_visited">Inclure seulement les noeuds visité dans le visuel</param>
        /// <returns>Graph MSAGL pouvant être afficher dans la vue</returns>
        public Graph getVisualGraph(bool only_visited = false)
        {
            Graph graph = new Graph();

            ColorMap cmap = new ColorMap(Color.Yellow, Color.Red, heuristicMax(), heuristicMin());

            foreach(CustomNode source in nodes.Values)
            {
                foreach(CustomEdge link in source.successors.Values)
                {
                    CustomNode dest = link.dest;

                    // Si l'on ne veut afficher que les noeuds visités
                    if (only_visited && !dest.visited)
                    {
                        continue;
                    }

                    Edge msagl_edge = graph.AddEdge(source.id, dest.id);

                    Node msagl_source_node = msagl_edge.SourceNode;
                    Node msagl_dest_node = msagl_edge.TargetNode;

                    // On change le style des noeuds de départ et d'arrivée
                    source.styleNode(msagl_source_node, cmap);
                    dest.styleNode(msagl_dest_node, cmap);

                    // On change le style de l'arc en fonction des noeuds de départ et d'arrivée
                    styleEdge(link, msagl_edge);
                }
            }

            graph.Attr.MinNodeHeight = 6;
            graph.Attr.MinNodeWidth = 6;

            return graph;
        }

        /// <summary>
        /// Change le style d'un arc MSAGL en fonction des deux noeuds qu'il relie
        /// </summary>
        /// <param name="source">Noeud de départ de l'arc</param>
        /// <param name="dest">Noeud de destination de l'arc</param>
        /// <param name="edge">Arc MSAGL dont l'on veut changer le style</param>
        private void styleEdge(CustomEdge cedge, Edge edge)
        {
            edge.Attr.LineWidth = 0.1;
            edge.Attr.ArrowheadLength = 1;

            // Si les 2 noeuds de l'arc font parti du chemin final
            if(cedge.source.in_selected_path && cedge.dest.in_selected_path)
            {
                edge.Attr.LineWidth = 1.5;
                edge.Attr.Color = Color.Red;
                //edge.LabelText = (dest.heuristic_value - source.heuristic_value).ToString();
                edge.LabelText = cedge.name;
                
            }
        }

        /// <summary>
        /// Retourne la valeur maximale de l'heuristique des noeuds visités dans le graphe
        /// </summary>
        /// <returns>Valeur maximale de l'heuristique</returns>
        private float heuristicMax()
        {
            float maxi = float.MinValue;

            foreach (CustomNode n in nodes.Values)
            {
                if (n.visited && n.heuristic_value > maxi)
                {
                    maxi = n.heuristic_value;
                }
            }
            return maxi;
        }

        /// <summary>
        /// Retourne la valeur minimale de l'heuristique des noeuds visités dans le graphe
        /// </summary>
        /// <returns>Valeur minimale de l'heuristique</returns>
        private float heuristicMin()
        {
            float mini = float.MaxValue;

            foreach(CustomNode n in nodes.Values)
            {
                if(n.visited && n.heuristic_value < mini)
                {
                    mini = n.heuristic_value;
                }
            }

            return mini;
        }
    }
}
