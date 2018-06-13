using Microsoft.Msagl.Drawing;
using System.Collections.Generic;
using System.Linq;

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

        private CustomNode root; 

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
            // Si la source n'est pas présente, on l'ajoute
            // Dans ce cas cela veut dire que l'on est sur le racine de l'arbre
            if(!nodes.ContainsKey(source_id))
            {
                nodes.Add(source_id, new CustomNode(source_id));
                root = nodes[source_id];
            }

            // Si la destination est déjà présente dans les noeuds, cela veut dire que ce n'est pas la première fois que ce noeuds est découvert
            // Ainsi on n'ajoute pas l'arc pour éviter les cycles
            if(nodes.ContainsKey(dest_id))
            {
                return;
            }

            nodes.Add(dest_id, new CustomNode(dest_id));

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

            source.visited = true;

            if (!source.successors.ContainsKey(dest_id))
            {
                source.successors.Add(dest_id, new CustomEdge() { name = edge_name, source = source, dest = dest, inSecondGraph = true });
            }
            else
            {
                //source.successors[dest_id].inBothGraph = true;
            }
            
            if(!dest.predecessors.ContainsKey(source_id))
            {
                dest.predecessors.Add(source_id, new CustomEdge() { name = edge_name, source = source, dest = dest, inSecondGraph = true });
            }
            else
            {
                //dest.predecessors[source_id].inBothGraph = true;
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
        /// 
        /// </summary>
        /// <param name="only_visited"></param>
        /// <param name="grouped"></param>
        /// <returns></returns>
        public Graph getVisualGraph(bool only_visited, bool grouped)
        {
            Graph graph;

            if (grouped)
            {
                graph = getVisualGraphGrouped(only_visited);
            }
            else
            {
                graph = getVisualGraphClassic(only_visited);
            }

            graph.LayoutAlgorithmSettings.EdgeRoutingSettings.EdgeRoutingMode = Microsoft.Msagl.Core.Routing.EdgeRoutingMode.StraightLine;
            graph.Attr.MinNodeHeight = 6;
            graph.Attr.MinNodeWidth = 6;

            return graph;
        }


        /// <summary>
        /// Retourne un Graph MSAGL comprenant tous les noeuds du graph, ou seulement les noeuds visités
        /// Le Graph MSAGL peut ensuite être affiché dans la vue
        /// </summary>
        /// <param name="only_visited">Inclure seulement les noeuds visité dans le visuel</param>
        /// <returns>Graph MSAGL pouvant être afficher dans la vue</returns>
        private Graph getVisualGraphClassic(bool only_visited = false)
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

                    // On change le style des noeuds de départs et d'arrivés
                    source.styleNode(msagl_source_node, cmap);
                    dest.styleNode(msagl_dest_node, cmap);

                    // On change le style de l'arc en fonction des noeuds de départs et d'arrivés
                    styleEdge(link, msagl_edge);
                }
            }

            return graph;
        }


        /// <summary>
        /// Retourne un Graph MSAGL avec les noeuds qui ne sont pas dans selected_path groupés
        /// </summary>
        /// <param name="only_visited">Inclure seulement les noeuds visité dans le visuel</param>
        /// <returns>Graph MSAGL pouvant être afficher dans la vue</returns>
        private Graph getVisualGraphGrouped(bool only_visited)
        {
            Graph graph = new Graph();
            ColorMap cmap = new ColorMap(Color.Yellow, Color.Red, heuristicMax(), heuristicMin());

            // Parcours en largeur du graph pour afficher seulement les noeuds dans le selected path
            // Les autres noeuds sont groupés en un seul noeuds
            Queue<CustomNode> queue = new Queue<CustomNode>();
            queue.Enqueue(root);

            while(queue.Any())
            {
                CustomNode actu = queue.Dequeue();

                foreach(CustomEdge edge in actu.successors.Values)
                {
                    CustomNode dest = edge.dest;

                    // Si l'on ne veut afficher que les noeuds visités
                    if (only_visited && !dest.visited)
                    {
                        continue;
                    }

                    Edge msagl_edge = graph.AddEdge(actu.id, dest.id);

                    Node msagl_source_node = msagl_edge.SourceNode;
                    Node msagl_dest_node = msagl_edge.TargetNode;

                    actu.styleNode(msagl_source_node, cmap);

                    if (dest.in_selected_path)
                    {
                        dest.styleNode(msagl_dest_node, cmap);
                        styleEdge(edge, msagl_edge);

                        queue.Enqueue(dest); // On ajoute le noeuds dans la file pour le traiter par la suite
                    }
                    else
                    {
                        dest.styleNodeGrouped(msagl_dest_node, cmap);
                        styleEdge(edge, msagl_edge);
                    }
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
            }

            if(cedge.source.visited && cedge.dest.visited)
            {
                edge.LabelText = cedge.name;
            }
            
            if(cedge.inBothGraph)
            {
                edge.Attr.Color = Color.Green;
            }
        }

        /// <summary>
        /// Retourne la valeur maximale de l'heuristique des noeuds visités dans le graphe
        /// </summary>
        /// <returns>Valeur maximale de l'heuristique</returns>
        private float heuristicMax()
        {
            return nodes.Values.Max(n => n.heuristic_value);
        }

        /// <summary>
        /// Retourne la valeur minimale de l'heuristique des noeuds visités dans le graphe
        /// </summary>
        /// <returns>Valeur minimale de l'heuristique</returns>
        private float heuristicMin()
        {
            return nodes.Values.Min(n => n.heuristic_value);
        }
    }
}
