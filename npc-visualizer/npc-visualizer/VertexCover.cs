using System;
using System.Collections.Generic;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    class VertexCover : GraphProblem
    {
        public VertexCover(Graph g, int param)
        {
            this.G = g;
            this.Param = param;
        }
        public override Literal[][] ToSat()
        {
            satVarToVertex = new int[Param * G.NodeCount];
            indexToSatVar = new int[G.NodeCount, Param + 1];

            GraphUtilities.CreateMapping(satVarToVertex, indexToSatVar, G.NodeCount, Param);
            ClauseCount();

            sat = new Literal[clauseCount][];
            DefineClauses();

            return sat;
        }
        public override int[] Solve()
        {
            ToSat();
            int varLim = G.NodeCount * Param;
            IEnumerable<SatSolution> solutions = SatSolver.Solve(new SatSolverParams(), varLim, sat);

            solution = GraphUtilities.SatSolutionToVertices(solutions, Param, satVarToVertex);

            return solution;
        }

        void DefineClauses()
        {
            int clauseIndex = 0;

            // At most K vertices are chosen
            for (int i = 1; i < Param + 1; i++)
            {
                for (int vertexNum1 = 0; vertexNum1 < G.NodeCount; vertexNum1++)
                {
                    for (int vertexNum2 = vertexNum1 + 1; vertexNum2 < G.NodeCount; vertexNum2++)
                    {
                        sat[clauseIndex++] = new Literal[2]
                        {
                            new Literal(indexToSatVar[vertexNum1, i], false),
                            new Literal(indexToSatVar[vertexNum2, i], false)
                        };
                    }                    
                }
            }

            // The chosen vertex-set is a vertex cover indeed
            foreach (Edge e in G.Edges)
            {
                sat[clauseIndex] = new Literal[2 * Param];
                int literal = 0;
                for (int i = 1; i < Param + 1; i++)
                {
                    sat[clauseIndex][literal++] = new Literal(indexToSatVar[int.Parse(e.Source), i], true);
                    sat[clauseIndex][literal++] = new Literal(indexToSatVar[int.Parse(e.Target), i], true);                   
                }
                clauseIndex++;
            }
        }

        void ClauseCount()
        {
            int nodeCount = G.NodeCount;
            int edgeCount = G.EdgeCount;

            int seriesSum = (nodeCount - 1) * nodeCount / 2;

            clauseCount =  (Param * seriesSum) + edgeCount;
        }
        public override Clique ToClique()
        {
            Graph flippedGraph = GraphUtilities.FlipEdges(G);
            return new Clique(flippedGraph, G.NodeCount - Param);
        }

        public override Colorability ToColorability()
        {
            ToSat();
            _3Sat reduction3Sat = new _3Sat(this.sat);
            return reduction3Sat.ToColorability();
        }

        public override DominatingSet ToDominatingSet()
        {
            Graph reduction = GraphUtilities.CopyGraph(G);

            // Find and remove singletons
            List<string> removeNodes = new List<string>();
            int deletedNodes = 0;
            foreach (Node node in reduction.Nodes)
            {
                bool hasEdges = node.Edges.GetEnumerator().MoveNext();

                if (!hasEdges)
                {
                    removeNodes.Add((int.Parse(node.Id) - deletedNodes++).ToString());
                }
            }

            foreach (string node in removeNodes)
            { 
                GraphUtilities.RemoveNode(ref reduction, node);
            }

            Edge[] edges = GraphUtilities.CopyEdges(reduction);

            foreach (Edge edge in edges)
            {
                string newNode = reduction.NodeCount.ToString();
                GraphUtilities.AddEdge(reduction, edge.Source, newNode);
                GraphUtilities.AddEdge(reduction, edge.Target, newNode);
            }

            return new DominatingSet(reduction, Param);
        }

        public override HamilCycle ToHamilCycle()
        {
            Graph reduction = new Graph();
            reduction.Directed = false;
            const int param = -1; // Isn't relevant in this NP-Complete problem

            // One of these figure equals zero then the condition equals zero
            if (G.EdgeCount * G.NodeCount == 0)
            {
                return new HamilCycle(reduction, param);
            }

            var inVertices = new List<Node>[G.NodeCount];
            var outVertices = new List<Node>[G.NodeCount];

            for (int i = 0; i < G.NodeCount; i++)
            {
                inVertices[i] = new List<Node>();
                outVertices[i] = new List<Node>();
            }

            // Create gadget for each edge in graph
            foreach (Edge edge in G.Edges)
            {
                Node leftIn = reduction.AddNode(reduction.NodeCount.ToString());
                Node left1 = reduction.AddNode(reduction.NodeCount.ToString());
                Node left2 = reduction.AddNode(reduction.NodeCount.ToString());
                Node left3 = reduction.AddNode(reduction.NodeCount.ToString());
                Node left4 = reduction.AddNode(reduction.NodeCount.ToString());
                Node leftOut = reduction.AddNode(reduction.NodeCount.ToString());

                Node rightIn = reduction.AddNode(reduction.NodeCount.ToString());
                Node right1 = reduction.AddNode(reduction.NodeCount.ToString());
                Node right2 = reduction.AddNode(reduction.NodeCount.ToString());
                Node right3 = reduction.AddNode(reduction.NodeCount.ToString());
                Node right4 = reduction.AddNode(reduction.NodeCount.ToString());
                Node rightOut = reduction.AddNode(reduction.NodeCount.ToString());

                GraphUtilities.AddEdge(reduction, leftIn.Id, left1.Id);
                GraphUtilities.AddEdge(reduction, left1.Id, left2.Id);
                GraphUtilities.AddEdge(reduction, left2.Id, left3.Id);
                GraphUtilities.AddEdge(reduction, left3.Id, left4.Id);
                GraphUtilities.AddEdge(reduction, left4.Id, leftOut.Id);

                GraphUtilities.AddEdge(reduction, rightIn.Id, right1.Id);
                GraphUtilities.AddEdge(reduction, right1.Id, right2.Id);
                GraphUtilities.AddEdge(reduction, right2.Id, right3.Id);
                GraphUtilities.AddEdge(reduction, right3.Id, right4.Id);
                GraphUtilities.AddEdge(reduction, right4.Id, rightOut.Id);

                GraphUtilities.AddEdge(reduction, leftIn.Id, right2.Id);
                GraphUtilities.AddEdge(reduction, left2.Id, rightIn.Id);
                GraphUtilities.AddEdge(reduction, left3.Id, rightOut.Id);
                GraphUtilities.AddEdge(reduction, leftOut.Id, right3.Id);

                inVertices[int.Parse(edge.Source)].Add(leftIn);
                inVertices[int.Parse(edge.Target)].Add(rightIn);
                outVertices[int.Parse(edge.Source)].Add(leftOut);
                outVertices[int.Parse(edge.Target)].Add(rightOut);
            }

            // Chain square gadgets
            foreach (Node node in G.Nodes)
            {
                int edgeCount = GraphUtilities.CountEdges(node.Edges);

                for (int i = 0; i < edgeCount - 1; i++)
                {
                    int index = int.Parse(node.Id);
                    GraphUtilities.AddEdge(reduction, outVertices[index][i].Id, inVertices[index][i + 1].Id);
                }
            }

            // Create Param * 'Selector Vertex' and connect them with entry and exit node of each chain
            for (int i = 0; i < Param; i++)
            {
                Node selectorNode = reduction.AddNode(reduction.NodeCount.ToString());

                for (int j = 0; j < G.NodeCount; j++)
                {
                    if (inVertices[j].Count == 0) continue;
                    GraphUtilities.AddEdge(reduction, inVertices[j][0].Id, selectorNode.Id);
                    GraphUtilities.AddEdge(reduction, outVertices[j][outVertices[j].Count - 1].Id, selectorNode.Id);
                }
            }

            return new HamilCycle(reduction, param);
        }

        public override IndepSet ToIndepSet()
        {
            return new IndepSet(GraphUtilities.CopyGraph(G), G.NodeCount - Param);
        }

        public override VertexCover ToVertexCover()
        {
            return new VertexCover(GraphUtilities.CopyGraph(G), Param);
        }
    }
}
