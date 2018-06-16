using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualisationHeuristique.Tools
{
    /// <summary>
    /// Implemente une structure de graphe issue de la fusion de 2 graphe
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
        /// <param name="only_visited">Affiché seulement les noeuds visités</param>
        /// <param name="grouped">Affiché les noeuds non visités de façon groupé</param>
        /// <returns>Graph MSAGL</returns>
        public Graph getVisualGraph(bool only_visited, bool grouped)
        {
            Graph graph;

            if(grouped)
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

            foreach (CustomNode source in nodes.Values)
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
            HashSet<string> visited = new HashSet<string>();
            Queue<CustomNodeMerged> queue = new Queue<CustomNodeMerged>();
            queue.Enqueue(root);

            while (queue.Any())
            {
                CustomNodeMerged actu = queue.Dequeue();

                if (visited.Contains(actu.id)) { continue; }
                visited.Add(actu.id);

                foreach (CustomEdge edge in actu.successors.Values)
                {
                    CustomNodeMerged dest = (CustomNodeMerged)edge.dest;

                    // Si l'on ne veut afficher que les noeuds visités
                    if (only_visited && !dest.isVisited())
                    {
                        continue;
                    }

                    Edge msagl_edge = graph.AddEdge(actu.id, dest.id);

                    Node msagl_source_node = msagl_edge.SourceNode;
                    Node msagl_dest_node = msagl_edge.TargetNode;

                    actu.styleNode(msagl_source_node, cmap);

                    if (dest.inSelectedPath())
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
