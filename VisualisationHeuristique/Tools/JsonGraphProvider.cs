using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using VisualisationHeuristique.Tools;

namespace VisualisationHeuristique
{
    class JsonGraphProvider
    {
        /// <summary>
        /// Retorne la liste des fichiers JSON d'un dossier
        /// </summary>
        /// <param name="folder_name">Nom du dossier ou chercher les fichiers</param>
        /// <returns>Liste des noms des fichiers JSON présent dans le dossier</returns>
        public static List<string> getJsonFileFromFolder(string folder_name)
        {
            List<string> file_names = new List<string>();

            DirectoryInfo d = new DirectoryInfo(folder_name);
            FileInfo[] files = d.GetFiles("*.json");

            foreach (FileInfo file in files)
            {
                file_names.Add(file.Name);
            }

            return file_names;
        }


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
                graph.addEdge(edge.Source, edge.Dest, edge.Time, edge.Name);
            }

            // On ajoute les informations comprises dans "node_selection"
            foreach(JsonNode node in jsonGraph.NodeSelection)
            {
                CustomNode graph_node = graph.nodes[node.Id];

                graph_node.visited = true;
                graph_node.heuristic_value = node.HeuristicValue;
                graph_node.real_final_value = node.RealFinalValue;
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

        [JsonProperty(PropertyName = "real_final_value")]
        public float RealFinalValue { get; set; }

        public int Order { get; set; }
    }

    /// <summary>
    /// Classe interne utilisée pour charger les composantes d'un arc en mémoire
    /// </summary>
    internal class JsonEdge
    {
        public int Time { get; set; }
        public string Name { get; set; }

        public string Source { get; set; }
        public string Dest { get; set; }
    }
}
