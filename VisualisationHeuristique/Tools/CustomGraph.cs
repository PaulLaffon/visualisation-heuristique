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
/*
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
        */

        /// <summary>
        /// Ajoute le noeud passé en paramètre dans le graphe courrant
        /// Est utilisé dans le cas de la fusion entre 2 graphes
        /// </summary>
        /// <param name="node">Node à ajouter au gaphe courrant</param>
        private void addMergedNode(CustomNode node)
        {
            if(!nodes.ContainsKey(node.id))
            {
                nodes.Add(node.id, new CustomNode(node.id));
            }

            CustomNode insertedNode = nodes[node.id];
            insertedNode.in_second_graph = true;
            insertedNode.visited_second = node.visited;
            insertedNode.in_selected_path_second = node.in_selected_path;
            insertedNode.heuristic_value_second = node.heuristic_value;
            insertedNode.order_discovered_second = node.order_discovered;
            insertedNode.order_visited_second = node.order_visited;
        }


        /// <summary>
        /// Ajoute tous les arcs du noeuds passé en paramètre dans le graphe courrant
        /// Est utilisé dans le cas de la fusion entre 2 graphes
        /// 
        /// TODO : Ajouté aussi les predécesseurs, pour l'instant n'ajoute que les successeurs
        /// </summary>
        /// <param name="source">Noeud dont on souhaite ajouter tous les arcs</param>
        private void addMergedEdges(CustomNode source)
        {
            CustomNode insertedNode = nodes[source.id];

            // On ajoute les arcs partant du noeuds (via les successeurs)
            foreach(CustomEdge edge in source.successors.Values)
            {
                if (!insertedNode.successors.ContainsKey(edge.dest.id))
                {
                    CustomNode sourceNode = nodes[edge.source.id];
                    CustomNode destNode = nodes[edge.dest.id];

                    insertedNode.successors.Add(edge.dest.id, new CustomEdge() { name = edge.name, source = sourceNode, dest = destNode });
                }

                CustomEdge insertedEdge = insertedNode.successors[edge.dest.id];

                insertedEdge.inSecondGraph = true;
            }
        }


        /// <summary>
        /// Fusionne 2 graphe en un seul afin de pouvoir comparer ces deux graphes dans un seul
        /// 
        /// On part du premier graphe dans lequel on ajoutes les noeuds et les arcs du second graphe (passé en paramètres)
        /// </summary>
        /// <param name="graph">Graphe avec lequel fusionner</param>
        /// <returns>CustomGraph résultant de la fusion des 2 graphes</returns>
        public CustomGraph merge(CustomGraph graph)
        {
            CustomGraph merge_result = this;

            // On indique que tous ces attributs sont présents dans le premier graphe
            foreach(CustomNode node in merge_result.nodes.Values)
            {
                node.in_first_graph = true;

                foreach(CustomEdge edge in node.successors.Values)
                {
                    edge.inFirstGraph = true;
                }
                foreach(CustomEdge edge in node.predecessors.Values)
                {
                    edge.inFirstGraph = true;
                }
            }

            // On ajoute tous les noeuds du graphe passé en paramètre dans le graphe résultant de la fusion
            foreach(CustomNode source in graph.nodes.Values)
            {
                merge_result.addMergedNode(source);
            }

            // On ajoutes tous les arcs de charque noeuds dans le graphe résultant de la fusion
            // On ajoute les arcs après afin de s'assurer que les noeuds soient bien tous présent dans le graphe
            foreach(CustomNode source in graph.nodes.Values)
            {
                merge_result.addMergedEdges(source);
            }

            return merge_result;
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
                    if (only_visited && !dest.visited && !dest.visited_second)
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
                        edge.styleEdge(msagl_edge);

                        queue.Enqueue(dest); // On ajoute le noeuds dans la file pour le traiter par la suite
                    }
                    else
                    {
                        dest.styleNode(msagl_dest_node, cmap, true);
                        edge.styleEdge(msagl_edge);
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
