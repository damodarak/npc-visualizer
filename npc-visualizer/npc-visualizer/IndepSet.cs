using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Msagl.Drawing;

namespace npc_visualizer
{
    class IndepSet
    {
        public static int[] Solve(Graph g, int setSize)
        {
            Graph flippedGraph = Utilities.FlipEdges(g);
            return Clique.Solve(flippedGraph, setSize);
        }
    }
}
