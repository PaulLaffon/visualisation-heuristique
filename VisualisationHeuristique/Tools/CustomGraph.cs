using Microsoft.Msagl.Drawing;
using System;
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

        public CustomNode root { get; private set; } 

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
        /// Retourne un graph MSAGL permattant l'affichage
        /// Appelle la bonne méthode en fonction des paramètres d'affichage
        /// </summary>
        /// <param name="only_visited">Affiché seulement les noeuds visités</param>
        /// <param name="grouped">Affiché les noeuds non visités de façon groupé</param>
        /// <returns>Graph MSAGL</returns>
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
                    if (only_visited && !dest.isVisited())
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
                    link.styleEdge(msagl_edge);
                }
            }

            return graph;
        }


        /// <summary>
        /// Retourne un Graph MSAGL avec les noeuds qui ne sont pas dans selected_path groupés
        /// </summary>
        /// <param name="only_visited">Inclure seulement les noeuds visité dans le visuel</param>
        /// <param name="group_level">A partir de quel niveau de profondeur du selected path on doit grouper les noeuds</param>
        /// <returns>Graph MSAGL pouvant être afficher dans la vue</returns>
        private Graph getVisualGraphGrouped(bool only_visited, int group_level = 0)
        {
            Graph graph = new Graph();
            ColorMap cmap = new ColorMap(Color.Yellow, Color.Red, heuristicMax(), heuristicMin());


            HashSet<string> visited = new HashSet<string>();

            // Parcours en largeur du graph pour afficher seulement les noeuds à un certains niveau de distance du selected path
            // Les autres noeuds sont groupés en un seul noeuds
            // L'entier dans le tuple correspond au niveau de distance du selected path
            Queue<Tuple<CustomNode, int> > queue = new Queue<Tuple<CustomNode, int> >();
            queue.Enqueue(new Tuple<CustomNode, int>(root, 0));

            while(queue.Any())
            {
                Tuple<CustomNode, int> tuple = queue.Dequeue();
                CustomNode actu = tuple.Item1;
                int profondeur = tuple.Item2;

                if(visited.Contains(actu.id)) { continue; }
                visited.Add(actu.id);

                Node msagl_source_node = graph.AddNode(actu.id);

                actu.styleNode(msagl_source_node, cmap, profondeur == group_level);


                foreach(CustomEdge edge in actu.successors.Values)
                {
                    CustomNode dest = edge.dest;

                    if(only_visited && !dest.isVisited()) { continue; }

                    // On ajoute que les noeuds du selected path ou avec un profondeur inférieur à la profondeur max
                    if(dest.in_selected_path || profondeur < group_level)
                    {
                        Edge msagl_edge = graph.AddEdge(actu.id, dest.id);

                        edge.styleEdge(msagl_edge);
                        int next_profondeur = profondeur + 1;

                        if(dest.in_selected_path)
                        {
                            next_profondeur = 0;
                        }

                        queue.Enqueue(new Tuple<CustomNode, int>(dest, next_profondeur));
                    }
                }
            }

            return graph;
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
