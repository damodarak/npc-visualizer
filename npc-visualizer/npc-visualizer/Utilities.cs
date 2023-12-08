using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

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
        public static Graph FlipEdges(Graph g)
        {
            Graph flippedGraph = new Graph("flippedGraph");

            foreach (Node node in g.Nodes)
            {
                flippedGraph.AddNode(node.Id);
            }

            Tuple<int, int>[] missingEdges = FindMissingEdges(g);
            Edge new_e;
            for (int i = 0; i < missingEdges.Length; i++)
            {
                new_e = flippedGraph.AddEdge(missingEdges[i].Item1.ToString(), missingEdges[i].Item2.ToString());
                new_e.Attr.Id = missingEdges[i].Item1.ToString() + "_" + missingEdges[i].Item2.ToString();
            }

            return flippedGraph;
        }
        public static void DrawSolution(Graph g, int[] solution)
        {
            for (int i = 0; i < solution.Length; i++)
            {
                if (solution[i] != -1)
                {
                    g.FindNode(solution[i].ToString()).Attr.FillColor = Color.Purple;
                }       
            }
        }
        public static void CreateMapping(int[] satVarToVertex, int[,] indexToSatVar, int nodeCount, int param)
        {
            int satVar = 0;

            for (int i = 1; i < param + 1; i++)
            {
                for (int vertexNum = 0; vertexNum < nodeCount; vertexNum++)
                {
                    indexToSatVar[vertexNum, i] = satVar;
                    satVarToVertex[satVar++] = vertexNum;
                }
            }
        }
        public static int[] SatSolutionToVertices(IEnumerable<SatSolution> solutions, int solutionSize, int[] satVarToVertex)
        {
            foreach (SatSolution solution in solutions)
            {
                //if inside, then there is a solution
                IEnumerable<int> positive = solution.Pos;
                int[] vertices = new int[solutionSize];
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = -1;
                }
                int index = 0;
                foreach (int pos in positive)
                {
                    if (index < solutionSize)
                    {
                        vertices[index++] = satVarToVertex[pos];
                    }
                    else
                    {
                        break;
                    }
                }
                return vertices;
            }

            return new int[] { };
        }
        public static int[] AdjacentNodes(Node node, Graph g)
        {
            int adjacentCount = 0;
            foreach (Edge edge in node.Edges)
            {
                adjacentCount++;
            }

            int[] adjacent = new int[adjacentCount + 1];


            int index = 0;
            adjacent[index++] = int.Parse(node.Id);
            foreach (Edge edge in node.Edges)
            {
                adjacent[index++] = edge.Source == node.Id ? int.Parse(edge.Target) : int.Parse(edge.Source);
            }

            return adjacent;
        }
    }
}