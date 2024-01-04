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

            //at most K vertices are chosen
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

            //the chosen vertex-set is a vertex cover indeed
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
        public override GraphProblem ToClique()
        {
            Graph flippedGraph = GraphUtilities.FlipEdges(G);
            return new Clique(flippedGraph, G.NodeCount - Param);
        }

        public override GraphProblem ToColorability()
        {
            throw new NotImplementedException();
        }

        public override GraphProblem ToDominatingSet()
        {
            Graph reduction = GraphUtilities.CopyGraph(G);

            // Remove singletons
            List<string> removeNodes = new List<string>();
            foreach (Node node in reduction.Nodes)
            {
                bool hasEdges = false;
                foreach (Edge edge in node.Edges)
                {
                    hasEdges = true;
                }

                if (!hasEdges)
                {
                    removeNodes.Add(node.Id);
                }
            }

            foreach (string node in removeNodes)
            {
                reduction.RemoveNode(reduction.FindNode(node));
            }

            foreach (Edge edge in G.Edges)
            {
                int newNode = reduction.NodeCount;
                GraphUtilities.AddEdge(reduction, edge.SourceNode.Id, newNode.ToString());
                GraphUtilities.AddEdge(reduction, edge.TargetNode.Id, newNode.ToString());
            }

            return new DominatingSet(reduction, Param);
        }

        public override GraphProblem ToHamilCycle()
        {
            throw new NotImplementedException();
        }

        public override GraphProblem ToIndepSet()
        {
            return new IndepSet(GraphUtilities.CopyGraph(G), G.NodeCount - Param);
        }

        public override GraphProblem ToVertexCover()
        {
            return new VertexCover(GraphUtilities.CopyGraph(G), Param);
        }
    }
}
