using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VisualisationHeuristique.Tools
{
    /// <summary>
    /// Implemente la structure d'un noeud de CustomGraph
    /// </summary>
    class CustomNode
    {
        public string id { get; private set; }

        public bool visited { get; set; }
        public bool in_selected_path { get; set; }
        public float heuristic_value { get; set; }
        public float real_final_value { get; set; }

        public int order_visited { get; set; }
        public int order_discovered { get; set; }



        public Dictionary<string, CustomEdge> successors { get; set; }
        public Dictionary<string, CustomEdge> predecessors { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="id">Id du noeud à construire</param>
        public CustomNode(string id)
        {
            this.id = id;

            successors = new Dictionary<string, CustomEdge>();
            predecessors = new Dictionary<string, CustomEdge>();
        }


        /// <summary>
        /// Compte récursivement le nombre de fils de ce noeuds
        /// </summary>
        /// <param name="visited">Collection des noeuds déjà visités afin d'eviter les boucle infinies en cas de cylce</param>
        /// <returns>Nombre de successeur du noeuds</returns>
        protected int getNumberChildren(HashSet<string> visited)
        {
            int childs = 0;

            // Permet de contrer les cycles dans les graphes
            if(visited.Contains(this.id)) { return 0; }
            visited.Add(this.id);

            foreach(CustomEdge edge in successors.Values)
            {
                if(!edge.dest.inSelectedPath())
                {
                    childs += edge.dest.getNumberChildren(visited) + 1;
                }
            }

            return childs;
        }

        /// <summary>
        /// Compte récursivement le nombre de fils visités de ce noeud
        /// </summary>
        /// <param name="visited">Collection des noeuds déjà visités afin d'eviter les boucle infinies en cas de cylce</param>
        /// <returns>Nombre de successeur visité du noeuds</returns>
        protected int getNumberVisitedChildren(HashSet<string> visited)
        {
            int childs = 0;

            // Permet de contrer les cycles dans les graphes
            if (visited.Contains(this.id)) { return 0; }
            visited.Add(this.id);

            foreach (CustomEdge edge in successors.Values)
            {
                if (edge.dest.isVisited() && !edge.dest.inSelectedPath())
                {
                    childs += edge.dest.getNumberVisitedChildren(visited) + 1;
                }
            }

            return childs;
        }

        /// <summary>
        /// Recherche la valeur minimale de l'heuristique des noeuds fils
        /// </summary>
        /// <param name="visited">Collection des noeuds déjà visités afin d'eviter les boucle infinies en cas de cylce</param>
        /// <returns>Valeur minimale de l'heuristique</returns>
        protected float childsMinHeuristic(HashSet<string> visited)
        {
            float heuristicMin = heuristic_value;

            // Permet de contrer les cycles dans les graphes
            if (visited.Contains(this.id)) { return heuristicMin; }
            visited.Add(this.id);

            foreach (CustomEdge edge in successors.Values)
            {
                if(edge.dest.visited && !edge.dest.inSelectedPath())
                {
                    heuristicMin = Math.Min(edge.dest.childsMinHeuristic(visited), heuristicMin);
                }
            }

            return heuristicMin;
        }

        /// <summary>
        /// Recherche la valeur maximale de l'heurtisque des noeuds fils
        /// </summary>
        /// <param name="visited">Collection des noeuds déjà visités afin d'eviter les boucle infinies en cas de cylce</param>
        /// <returns></returns>
        protected float childsMaxHeuristic(HashSet<string> visited)
        {
            float heuristicMax = heuristic_value;

            // Permet de contrer les cycles dans les graphes
            if (visited.Contains(this.id)) { return heuristicMax; }
            visited.Add(this.id);

            foreach (CustomEdge edge in successors.Values)
            {
                if (edge.dest.visited && !edge.dest.inSelectedPath())
                {
                    heuristicMax = Math.Max(edge.dest.childsMaxHeuristic(visited), heuristicMax);
                }
            }

            return heuristicMax;
        }


        /// <summary>
        /// Style par default de tous les noeuds du graphe
        /// Une fois ce style par default appliqué, des surcharges peuvent être faites en fonction du type de noeuds
        /// </summary>
        /// <param name="node">Noeuds MSAGL correspondant au CustomNode dont on change le style</param>
        /// <param name="cmap">Colormap pour appliquer le stype au noeuds</param>
        protected void defaultNodeStyle(Node node, ColorMap cmap)
        {
            node.Attr.Shape = Shape.Circle;
            node.Attr.LabelMargin = 0;
            node.Attr.LineWidth = 0.2;
            node.LabelText = null;

            if (visited)
            {
                node.Attr.FillColor = cmap.getColor(heuristic_value);
            }
            else
            {
                node.Attr.FillColor = Color.White;
            }
        }


        /// <summary>
        /// Fonction permettant de styliser les noeuds MSAGL en fonction des attributs du noeuds
        /// </summary>
        /// <param name="node">Noeuds MSAGL à styliser</param>
        /// <param name="cmap">ColorMap à utiliser pour donner la couleur</param>
        /// <param name="grouped">Boolean qui indique que c'est un noeuds groupé</param>
        public void styleNode(Node node, ColorMap cmap, bool grouped = false)
        {
            defaultNodeStyle(node, cmap);

            if(grouped)
            {
                styleNodeGrouped(node);                    
            }
            else
            {
                styleNodeClassic(node, cmap);
            }
            
        }

        /// <summary>
        /// Style classique pour un noeuds MSAGL
        /// </summary>
        /// <param name="node">Noeuds MSAGL à styliser</param>
        protected virtual void styleNodeClassic(Node node, ColorMap cmap)
        {
            node.Attr.Tooltip = getTooltip();

            if (visited)
            {
                node.LabelText = order_visited.ToString();
            }
        }

        /// <summary>
        /// Style pour les noeuds grouper, ces noeuds contienent plusieurs noeuds
        /// </summary>
        /// <param name="node">Noeuds MSAGL à styliser</param>
        protected void styleNodeGrouped(Node node)
        {
            int nodeSize = getNumberChildren(new HashSet<string>()) + 1;

            string groupedTooltip = "Nombre de noeuds : " + nodeSize.ToString();
            groupedTooltip += "\nNombre de noeuds visités : " + (getNumberVisitedChildren(new HashSet<string>()) + 1).ToString();
            groupedTooltip += "\nHeuristique max : " + childsMaxHeuristic(new HashSet<string>()).ToString();
            groupedTooltip += "\nHeuristique min : " + childsMinHeuristic(new HashSet<string>()).ToString();

            node.Attr.Tooltip = groupedTooltip;

            node.LabelText = "";
            node.Attr.LabelMargin = (int)Math.Pow(nodeSize, 0.66) + 3;
        }

        /// <summary>
        /// Renvoie un texte décrivant le noeuds afin d'être utilisé dans le tooltip 
        /// </summary>
        /// <returns>string décrivant le noeud</returns>
        protected virtual string getTooltip()
        {
            string tooltip = "";

            tooltip += "Node id : " + id;

            if(visited)
            {
                tooltip += "\n" + "Heuristique value : " + heuristic_value.ToString();
                tooltip += "\n" + "Real value : " + real_final_value.ToString();
            }

            tooltip += "\n" + "Ordre de découverte : " + order_discovered.ToString();

            return tooltip;
        }

        /// <summary>
        /// Indique si le noeuds a été visité
        /// </summary>
        /// <returns>Booleen</returns>
        public virtual bool isVisited()
        {
            return visited;
        }

        /// <summary>
        /// Indique si le noeuds est dans le chemin final
        /// </summary>
        /// <returns></returns>
        public virtual bool inSelectedPath()
        {
            return in_selected_path;
        }
    }
}
