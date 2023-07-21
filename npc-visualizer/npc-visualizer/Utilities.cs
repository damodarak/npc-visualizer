using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Msagl.Drawing;

namespace npc_visualizer
{
    class Utilities
    {
        public static void ClearVertexColor(Graph g)
        {
            foreach (Node node in g.Nodes)
            {
                node.Attr.FillColor = Color.White;
            }
        }
        public static Edge EdgeById(Graph g, string id)
        {
            IEnumerable<Edge> edges = g.Edges;

            string[] vertices = id.Split('_');

            foreach (Edge edge in edges)
            {
                if ((edge.Source == vertices[0] && edge.Target == vertices[1]) ||
                    (edge.Target == vertices[0] && edge.Source == vertices[1]))
                {
                    return edge;
                }
            }

            return null;
        }

        public static Tuple<int, int>[] FindMissingEdges(Graph g)
        {
            int missingCount = ((g.NodeCount * (g.NodeCount - 1)) / 2) - g.EdgeCount;
            Tuple<int, int>[] missingEdges = new Tuple<int, int>[missingCount];
            int index = 0;

            for (int i = 0; i < g.NodeCount; i++)
            {
                for (int j = i + 1; j < g.NodeCount; j++)
                {
                    if (EdgeById(g, i.ToString() + "_" + j.ToString()) == null)
                    {
                        missingEdges[index++] = new Tuple<int, int>(i, j);
                    }
                }
            }

            return missingEdges;
        }
    }
}

