using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    static class GraphUtilities
    {
        public static void ClearVertexColorAndEdgeStyle(Graph g)
        {
            foreach (Node node in g.Nodes)
            {
                node.Attr.FillColor = Color.White;
                node.Attr.LineWidth = 1;
            }

            foreach (Edge edge in g.Edges)
            {
                edge.Attr.ArrowheadAtTarget = ArrowStyle.None;
                edge.Attr.ArrowheadAtSource = ArrowStyle.None;
                edge.Attr.Color = Color.Black;
            }
        }

        public static void RemoveNode(ref Graph g, string nodeId)
        {
            Graph new_g = new Graph();
            new_g.Directed = false;
            int nodeNum = int.Parse(nodeId);

            foreach (Node node in g.Nodes)
            {
                if (node.Id != nodeId)
                {
                    if (int.Parse(node.Id) > nodeNum)
                    {
                        new_g.AddNode((int.Parse(node.Id) - 1).ToString()).Attr.Shape = Shape.Circle;
                    }
                    else
                    {
                        new_g.AddNode(node.Id).Attr.Shape = Shape.Circle;
                    }
                }
            }

            foreach (Edge edge in g.Edges)
            {
                if (edge.Source  == nodeId || edge.Target == nodeId)
                {
                    continue;
                }

                if (int.Parse(edge.Target) < nodeNum)
                {
                    Edge e = new_g.AddEdge(edge.SourceNode.Id, edge.TargetNode.Id);
                    e.Attr.ArrowheadAtTarget = ArrowStyle.None;
                    e.Attr.Id = edge.SourceNode.Id + "_" + edge.TargetNode.Id;
                }
                else if (int.Parse(edge.Source) < nodeNum && int.Parse(edge.Target) > nodeNum)
                {
                    Edge e = new_g.AddEdge(edge.SourceNode.Id, (int.Parse(edge.TargetNode.Id) - 1).ToString());
                    e.Attr.ArrowheadAtTarget = ArrowStyle.None;
                    e.Attr.Id = edge.SourceNode.Id + "_" + (int.Parse(edge.TargetNode.Id) - 1).ToString();
                }
                else
                {
                    Edge e = new_g.AddEdge((int.Parse(edge.SourceNode.Id) - 1).ToString(), (int.Parse(edge.TargetNode.Id) - 1).ToString());
                    e.Attr.ArrowheadAtTarget = ArrowStyle.None;
                    e.Attr.Id = (int.Parse(edge.SourceNode.Id) - 1).ToString() + "_" + (int.Parse(edge.TargetNode.Id) - 1).ToString();
                }
            }

            g = new_g;
        }

        public static Graph CopyGraph(Graph g)
        {
            Graph copy = new Graph();
            copy.Directed = false;
            foreach (Node node in g.Nodes)
            {
                copy.AddNode(node.Id).Attr.Shape = Shape.Circle;
            }

            foreach (Edge edge in g.Edges)
            {
                Edge e = copy.AddEdge(edge.Source, edge.Target);
                e.Attr.ArrowheadAtTarget = ArrowStyle.None;
                e.Attr.Id = edge.Attr.Id;
            }

            return copy;
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
            flippedGraph.Directed = false;

            foreach (Node node in g.Nodes)
            {
                flippedGraph.AddNode(node.Id).Attr.Shape = Shape.Circle;
            }

            Tuple<int, int>[] missingEdges = FindMissingEdges(g);
            Edge newEdge;
            for (int i = 0; i < missingEdges.Length; i++)
            {
                newEdge = flippedGraph.AddEdge(missingEdges[i].Item1.ToString(), missingEdges[i].Item2.ToString());
                newEdge.Attr.Id = missingEdges[i].Item1.ToString() + "_" + missingEdges[i].Item2.ToString();
                newEdge.Attr.ArrowheadAtTarget = ArrowStyle.None;
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

            int[] adjacent = new int[adjacentCount];

            int index = 0;
            foreach (Edge edge in node.Edges)
            {
                adjacent[index++] = edge.Source == node.Id ? int.Parse(edge.Target) : int.Parse(edge.Source);
            }

            return adjacent;
        }
    }
}