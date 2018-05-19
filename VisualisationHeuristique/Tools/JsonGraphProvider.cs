using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using VisualisationHeuristique.Tools;

namespace VisualisationHeuristique
{
    class JsonGraphProvider
    {
        /*
        public static Graph GraphFromFile(string filename)
        {
            // Object graph utilisé par MSAGL
            Graph g = new Graph();
            
            // On récupère le graphe en désérializant le JSON
            JsonGraph jsonGraph = JsonConvert.DeserializeObject<JsonGraph>(File.ReadAllText(filename));

            // Dictionnaire des noeuds qui ont été exploré (ayant une valeure d'heuristique)
            SortedDictionary<string, Node> nodes = new SortedDictionary<string, Node>();

            // On rempli le dictionnaire des noeuds qui ont été exploré
            foreach(Node n in jsonGraph.NodeSelection)
            {
                nodes.Add(n.Id, new Node() { Id=n.Id, HeuristicValue = n.HeuristicValue, Order = n.Order});
            }


            // Valeur minimum et maximum de l'heuristique dans le graphe
            double min_value = jsonGraph.NodeSelection.Min(n => n.HeuristicValue);
            double max_value = jsonGraph.NodeSelection.Max(n => n.HeuristicValue);

            ColorMap cmap = new ColorMap(Color.Yellow, Color.Red, max_value, min_value);


            foreach (Edge e in jsonGraph.ExecutionNodes)
            {
                Microsoft.Msagl.Drawing.Edge edge = g.AddEdge(e.Source, e.Dest);

                Microsoft.Msagl.Drawing.Node source = edge.SourceNode;
                Microsoft.Msagl.Drawing.Node dest = edge.TargetNode;

                source.Attr.Shape = dest.Attr.Shape = Shape.Circle;
                source.LabelText = dest.LabelText = null;

                source.Attr.LabelMargin = dest.Attr.LabelMargin = 0;

                source.Attr.LineWidth = dest.Attr.LineWidth = 0.2;

                source.Attr.FillColor = dest.Attr.FillColor = Color.White;
              
                edge.Attr.LineWidth = 0.1;
                edge.Attr.ArrowheadLength = 1;

                source.Attr.Tooltip = "Patate";
                dest.Attr.Tooltip = "Patate";


                // Si le l'arc contient 2 noeuds dans le selected_path, alors c'est un arc dans le chemin final
                if (jsonGraph.SelectedPath.Select(n=>n.Id).Count(id=>id == source.Id || id == dest.Id) == 2)
                {
                    edge.Attr.LineWidth = 1;
                    edge.Attr.Color = Color.Red;
                }


                if(nodes.ContainsKey(source.Id))
                {
                    source.Attr.FillColor = cmap.getColor(nodes[source.Id].HeuristicValue);
                    source.LabelText = nodes[source.Id].Order.ToString();
                }

                if (nodes.ContainsKey(dest.Id))
                {
                    dest.Attr.FillColor = cmap.getColor(nodes[dest.Id].HeuristicValue);
                    dest.LabelText = nodes[dest.Id].Order.ToString();
                }
            }

            //g.LayoutAlgorithmSettings = new FastIncrementalLayoutSettings();

            g.Attr.MinNodeHeight = 1;
            g.Attr.MinNodeWidth = 1;

            return g;
        }
        */

        /// <summary>
        /// Retourne un object CustomGraph qui pourra être utilisé pour générer la visualisation
        /// Crée l'object à  partir d'un fichier contenant du JSON
        /// </summary>
        /// <param name="filename">Chemin relatif du fichier JSON</param>
        /// <returns>CustomGraph</returns>
        public static CustomGraph loadGraphFromFile(string filename)
        {
            string json_string = File.ReadAllText(filename);

            return loadGraphFromJson(json_string);
        }

        /// <summary>
        /// Retourne un object CustomGraph qui pourra être utilisé pour générer la visualisation 
        /// Crée l'object à partir d'une chaine de caractères JSON
        /// </summary>
        /// <param name="json_string">Chaine de caractères JSON</param>
        /// <returns>CustomGraph</returns>
        public static CustomGraph loadGraphFromJson(string json_string)
        {
            CustomGraph graph = new CustomGraph();

            // On converti la string JSON en object JsonGraph
            JsonGraph jsonGraph = JsonConvert.DeserializeObject<JsonGraph>(json_string);

            // On ajoute tous les arcs
            foreach (JsonEdge edge in jsonGraph.ExecutionNodes)
            {
                graph.addEdge(edge.Source, edge.Dest, edge.Time);
            }

            // On ajoute les informations comprises dans "node_selection"
            foreach(JsonNode node in jsonGraph.NodeSelection)
            {
                CustomNode graph_node = graph.nodes[node.Id];

                graph_node.visited = true;
                graph_node.heuristic_value = node.HeuristicValue;
                graph_node.order_visited = node.Order;
            }

            // On ajoute les informations comprises dans "selected_path"
            foreach(JsonNode node in jsonGraph.SelectedPath)
            {
                CustomNode graph_node = graph.nodes[node.Id];

                graph_node.in_selected_path = true;
            }

            return graph;
        }
    }

    /// <summary>
    /// Class interne utilisée pour charge le JSON en mémoire
    /// </summary>
    internal class JsonGraph
    {
        [JsonProperty(PropertyName = "execution_nodes")]
        public List<JsonEdge> ExecutionNodes { get; set; }

        [JsonProperty(PropertyName = "selected_path")]
        public List<JsonNode> SelectedPath { get; set; }

        [JsonProperty(PropertyName = "node_selection")]
        public List<JsonNode> NodeSelection { get; set; }
    }

    /// <summary>
    /// Classe interne utilisée pour charger les composantes d'un noeud en mémoire
    /// </summary>
    internal class JsonNode
    {
        [JsonProperty(PropertyName = "node_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "heuristic_value")]
        public float HeuristicValue { get; set; }

        public int Order { get; set; }
    }

    /// <summary>
    /// Classe interne utilisée pour charger les composantes d'un arc en mémoire
    /// </summary>
    internal class JsonEdge
    {
        public int Time { get; set; }

        public string Source { get; set; }
        public string Dest { get; set; }
    }
}
