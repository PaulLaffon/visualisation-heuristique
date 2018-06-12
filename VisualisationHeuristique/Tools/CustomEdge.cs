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

        public string name { get; set; }
    }
}
