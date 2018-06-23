using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualisationHeuristique.Tools
{
    /// <summary>
    /// Implemente une structure de graphe issue de la fusion de 2 graphes
    /// </summary>
    class CustomGraphMerged
    {
        /// <summary>
        /// Tous les noeuds composant le graphe
        /// </summary>
        public Dictionary<string, CustomNodeMerged> nodes { get; }

        private CustomNodeMerged root;

        /// <summary>
        /// Constructeur
        /// Prends 2 graphe et les fusionnent
        /// </summary>
        public CustomGraphMerged(CustomGraph graph1, CustomGraph graph2)
        {
            nodes = new Dictionary<string, CustomNodeMerged>();

            // On ajoute les noeuds du premier graphes
            this.addMergedNode(graph1.root, false);
            foreach(CustomNode node in graph1.nodes.Values)
            {
                this.addMergedNode(node, false);
            }

            // On ajoute les arcs du premier graphe
            foreach (CustomNode source in graph1.nodes.Values)
            {
                this.addMergedEdges(source, false);
            }

            // On ajoute les noeuds du deuxième graphe
            foreach (CustomNode source in graph2.nodes.Values)
            {
                this.addMergedNode(source, true);
            }

            foreach (CustomNode source in graph2.nodes.Values)
            {
                this.addMergedEdges(source, true);
            }
        }


        /// <summary>
        /// Ajoute le noeud passé en paramètre dans le graphe courrant
        /// Est utilisé par le constructeur du graph de fusion
        /// </summary>
        /// <param name="node">Node à ajouter au gaphe courrant</param>
        /// <param name="second_graph">Boolean qui indique si c'est un noeuds du second graphe</param>
        private void addMergedNode(CustomNode node, bool second_graph)
        {
            if (!nodes.ContainsKey(node.id))
            {
                nodes.Add(node.id, new CustomNodeMerged(node.id));
            }

            CustomNodeMerged insertedNode = nodes[node.id];
            insertedNode.initFromNode(node, second_graph);

            // Si c'est le premier noeud qu'on ajoute, c'est la racine
            if (nodes.Values.Count == 1)
            {
                root = insertedNode;
            }
        }


        /// <summary>
        /// Ajoute tous les arcs du noeuds passé en paramètre dans le graphe courrant
        /// Est utilisé par le constructeur du graph de fusion
        /// 
        /// TODO : Ajouté aussi les predécesseurs, pour l'instant n'ajoute que les successeurs
        /// </summary>
        /// <param name="source">Noeud dont on souhaite ajouter tous les arcs</param>
        /// <param name="second_graph">Boolean qui indique si l'arc provient deu deuxième graphe</param>
        private void addMergedEdges(CustomNode source, bool second_graph)
        {
            CustomNodeMerged insertedNode = nodes[source.id];

            // On ajoute les arcs partant du noeuds (via les successeurs)
            foreach (CustomEdge edge in source.successors.Values)
            {
                if (!insertedNode.successors.ContainsKey(edge.dest.id))
                {
                    CustomNodeMerged sourceNode = nodes[edge.source.id];
                    CustomNodeMerged destNode = nodes[edge.dest.id];

                    insertedNode.successors.Add(edge.dest.id, new CustomEdgeMerged() { name = edge.name, source = sourceNode, dest = destNode });
                }

                CustomEdgeMerged insertedEdge = (CustomEdgeMerged)insertedNode.successors[edge.dest.id];

                if (second_graph)
                {
                    insertedEdge.inSecondGraph = true;
                }
                else
                {
                    insertedEdge.inFirstGraph = true;
                }

            }
        }


        /// <summary>
        /// Retourne un graph MSAGL permattant l'affichage
        /// Appelle la bonne méthode en fonction des paramètres d'affichage
        /// </summary>
        /// <param name="only_visited">Afficher seulement les noeuds visités</param>
        /// <param name="grouped">Afficher les noeuds non visités de façon groupé</param>
        /// <param name="edge_name">Afficher les nom des arcs dans la visualisation</param>
        /// <param name="group_level">Niveau de groupement pour la visualisation groupée</param>
        /// <returns>Graph MSAGL</returns>
        public Graph getVisualGraph(bool only_visited, bool grouped, bool edge_name, int group_level = 0)
        {
            Graph graph;

            if(grouped)
            {
                graph = getVisualGraphGrouped(only_visited, edge_name, group_level);
            }
            else
            {
                graph = getVisualGraphClassic(only_visited, edge_name);
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
        /// <param name="edge_name">Afficher les nom des arcs dans la visualisation</param>
        /// <returns>Graph MSAGL pouvant être afficher dans la vue</returns>
        private Graph getVisualGraphClassic(bool only_visited, bool edge_name)
        {
            Graph graph = new Graph();

            ColorMap cmap = new ColorMap(new Color(255, 255, 210, 210), Color.Red, realValueMax(), realValueMin());
            ColorMap cmap_first = new ColorMap(new Color(255, 230, 255, 230), Color.Green, firstHeuristicMax(), firstheuristicMin());
            ColorMap cmap_second = new ColorMap(new Color(255, 240, 240, 255), Color.Blue, secondHeuristicMax(), secondHeuristicMin());

            foreach (CustomNodeMerged source in nodes.Values)
            {
                foreach (CustomEdge link in source.successors.Values)
                {
                    CustomNodeMerged dest = (CustomNodeMerged)link.dest;

                    // Si l'on ne veut afficher que les noeuds visités
                    if (only_visited && !dest.isVisited())
                    {
                        continue;
                    }

                    Edge msagl_edge = graph.AddEdge(source.id, dest.id);

                    Node msagl_source_node = msagl_edge.SourceNode;
                    Node msagl_dest_node = msagl_edge.TargetNode;

                    source.styleNodeMerged(msagl_source_node, cmap, cmap_first, cmap_second);
                    dest.styleNodeMerged(msagl_dest_node, cmap, cmap_first, cmap_second);

                    // On change le style de l'arc en fonction des noeuds de départs et d'arrivés
                    link.styleEdge(msagl_edge, edge_name);
                }
            }

            return graph;
        }


        /// <summary>
        /// Retourne un Graph MSAGL avec les noeuds qui ne sont pas dans selected_path groupés
        /// </summary>
        /// <param name="only_visited">Inclure seulement les noeuds visité dans le visuel</param>
        /// <param name="edge_name">Afficher les nom des arcs dans la visualisation</param>
        /// <param name="group_level">A partir de quel niveau de profondeur du selected path on doit grouper les noeuds</param>
        /// <returns>Graph MSAGL pouvant être afficher dans la vue</returns>
        private Graph getVisualGraphGrouped(bool only_visited, bool edge_name, int group_level)
        {
            Graph graph = new Graph();

            ColorMap cmap = new ColorMap(new Color(255, 255, 210, 210), Color.Red, realValueMax(), realValueMin());
            ColorMap cmap_first = new ColorMap(new Color(255, 230, 255, 230), Color.Green, firstHeuristicMax(), firstheuristicMin());
            ColorMap cmap_second = new ColorMap(new Color(255, 240, 240, 255), Color.Blue, secondHeuristicMax(), secondHeuristicMin());

            HashSet<string> visited = new HashSet<string>();

            // Parcours en largeur du graph pour afficher seulement les noeuds à un certains niveau de distance du selected path
            // Les autres noeuds sont groupés en un seul noeuds
            // L'entier dans le tuple correspond au niveau de distance du selected path
            Queue<Tuple<CustomNodeMerged, int>> queue = new Queue<Tuple<CustomNodeMerged, int>>();
            queue.Enqueue(new Tuple<CustomNodeMerged, int>(root, 0));

            while (queue.Any())
            {
                Tuple<CustomNodeMerged, int> tuple = queue.Dequeue();
                CustomNodeMerged actu = tuple.Item1;
                int profondeur = tuple.Item2;

                if (visited.Contains(actu.id)) { continue; }
                visited.Add(actu.id);

                Node msagl_source_node = graph.AddNode(actu.id);

                if(profondeur == group_level)
                {
                    actu.styleNodeMergedGrouped(msagl_source_node, cmap, cmap_first, cmap_second);
                }
                else
                {
                    actu.styleNodeMerged(msagl_source_node, cmap, cmap_first, cmap_second);
                }


                foreach (CustomEdge edge in actu.successors.Values)
                {
                    CustomNodeMerged dest = (CustomNodeMerged)edge.dest;

                    if (only_visited && !dest.isVisited()) { continue; }

                    // On ajoute que les noeuds du selected path ou avec un profondeur inférieur à la profondeur max
                    if (dest.inSelectedPath() || profondeur < group_level)
                    {
                        Edge msagl_edge = graph.AddEdge(actu.id, dest.id);

                        edge.styleEdge(msagl_edge, edge_name);
                        int next_profondeur = profondeur + 1;

                        if (dest.inSelectedPath())
                        {
                            next_profondeur = 0;
                        }

                        queue.Enqueue(new Tuple<CustomNodeMerged, int>(dest, next_profondeur));
                    }
                }
            }

            return graph;
        }


        /// <summary>
        /// Retourne la valeur maximale de l'heuristique des noeuds visités dans le premier graphe
        /// </summary>
        /// <returns>Valeur maximale de l'heuristique</returns>
        private float firstHeuristicMax()
        {
            return nodes.Values.Where(n => n.visited == true).Max(n => n.heuristic_value);
        }

        private float secondHeuristicMax()
        {
            return nodes.Values.Where(n => n.visited_second == true).Max(n => n.heuristic_value);
        }

        private float realValueMax()
        {
            return nodes.Values.Max(n => n.real_final_value);
        }

        /// <summary>
        /// Retourne la valeur minimale de l'heuristique des noeuds visités dans le premier graphe
        /// </summary>
        /// <returns>Valeur minimale de l'heuristique</returns>
        private float firstheuristicMin()
        {
            return nodes.Values.Where(n => n.visited == true).Min(n => n.heuristic_value);
        }

        private float secondHeuristicMin()
        {
            return nodes.Values.Where(n => n.visited_second == true).Min(n => n.heuristic_value);
        }

        private float realValueMin()
        {
            return nodes.Values.Min(n => n.real_final_value);
        }
    }
}
