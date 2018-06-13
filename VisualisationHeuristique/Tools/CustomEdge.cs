using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualisationHeuristique.Tools
{
    /// <summary>
    /// Classe modélisant un arc entre 2 noeuds
    /// </summary>
    class CustomEdge
    {
        public CustomNode source { get; set; }
        public CustomNode dest { get; set; }

        /// <summary>
        /// Nom de l'action effectué par l'arc
        /// </summary>
        public string name { get; set; }



        // Les attributs et méthodes ci-dessous ne sont utilisé quand dans le cas de la fusion entre 2 graphes

        /// <summary>
        /// Booléen qui indique si cet arc est présent dans le premier graphe
        /// </summary>
        public bool inFirstGraph { get; set; }

        /// <summary>
        /// Booléen qui indique si cet arc est présent dans le deuxième graphe
        /// </summary>
        public bool inSecondGraph { get; set; }

        public bool inBothGraph()
        {
            return inFirstGraph && inSecondGraph;
        }
    }
}
