using System;
using System.Collections.Generic;

using Microsoft.SolverFoundation.Solvers;
using Microsoft.Msagl.Drawing;

namespace npc_visualizer
{
    class Clique : GraphProblem
    {
        public Clique(Graph g, int param) 
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
            if (Param == 1)
            {
                solution = new int[] { 0 };
                return solution;
            }

            ToSat();
            int varLim = G.NodeCount * Param;
            IEnumerable<SatSolution> satSolutions = SatSolver.Solve(new SatSolverParams(), varLim, sat);

            solution = GraphUtilities.SatSolutionToVertices(satSolutions, Param, satVarToVertex);

            return solution;
        }

        void ClauseCount()
        {
            int nodeCount = G.NodeCount;
            int edgeCount = G.EdgeCount;

            int seriesSumClique = ((Param - 1) * Param) / 2;
            int seriesSumNodes = ((nodeCount - 1) * nodeCount) / 2;
            int maxEdges = (nodeCount * (nodeCount - 1)) / 2;
            int missingEdges = maxEdges - edgeCount;

            clauseCount =  (seriesSumClique * nodeCount) + (seriesSumClique * missingEdges * 2) + Param + (Param * seriesSumNodes);
        }

        void DefineClauses()
        {
            int clauseIndex = 0;
            int nodeCount = G.NodeCount;

            //ith and jth vertices in one clique are different
            for (int vertex = 0; vertex < nodeCount; vertex++)
            {
                for (int i = 1; i < Param + 1; i++)
                {
                    for (int j = i + 1; j < Param + 1; j++)
                    {
                        sat[clauseIndex++] = new Literal[]
                        {
                            new Literal(indexToSatVar[vertex, i], false),
                            new Literal(indexToSatVar[vertex, j], false)
                        };
                    }
                }
            }

            //Any two vertices in the clique are connected
            Tuple<int, int>[] missingEdges = GraphUtilities.FindMissingEdges(G);
            for (int missEdge = 0; missEdge < missingEdges.Length; missEdge++)
            {
                for (int i = 1; i < Param + 1; i++)
                {
                    for (int j = i + 1; j < Param + 1; j++)
                    {
                        sat[clauseIndex++] = new Literal[]
                        {
                        new Literal(indexToSatVar[missingEdges[missEdge].Item1, i], false),
                        new Literal(indexToSatVar[missingEdges[missEdge].Item2, j], false)
                        };

                        sat[clauseIndex++] = new Literal[]
                        {
                        new Literal(indexToSatVar[missingEdges[missEdge].Item1, j], false),
                        new Literal(indexToSatVar[missingEdges[missEdge].Item2, i], false)
                        };
                    }
                }
            }

            //There is an ith vertex
            for (int i = 1; i < Param + 1; i++)
            {
                sat[clauseIndex] = new Literal[G.NodeCount];
                for (int vertex = 0; vertex < G.NodeCount; vertex++)
                {
                    sat[clauseIndex][vertex] = new Literal(indexToSatVar[vertex, i], true);
                }

                clauseIndex++;
            }

            //there is only one ith vertex in the clique
            for (int i = 1; i < Param + 1; i++)
            {
                for (int nodeNum1 = 0; nodeNum1 < G.NodeCount; nodeNum1++)
                {
                    for (int nodeNum2 = nodeNum1 + 1; nodeNum2 < G.NodeCount; nodeNum2++)
                    {
                        sat[clauseIndex++] = new Literal[]
                        {
                        new Literal(indexToSatVar[nodeNum1, i], false),
                        new Literal(indexToSatVar[nodeNum2, i], false)
                        };
                    }
                }
            }
        }
        public override GraphProblem ToClique()
        {
            return new Clique(GraphUtilities.CopyGraph(G), Param);
        }
        public override GraphProblem ToColorability()
        {
            ToSat();
            _3Sat reduction3Sat = new _3Sat(this.sat);
            return reduction3Sat.ToColorability();
        }
        public override GraphProblem ToDominatingSet()
        {
            throw new NotImplementedException();
        }
        public override GraphProblem ToHamilCycle()
        {
            throw new NotImplementedException();
        }
        public override GraphProblem ToIndepSet()
        {
            Graph flippedGraph = GraphUtilities.FlipEdges(G);
            return new IndepSet(flippedGraph, Param);
        }
        public override GraphProblem ToVertexCover()
        {
            Graph flippedGraph = GraphUtilities.FlipEdges(G);
            return new VertexCover(flippedGraph, G.NodeCount - Param);
        }
    }   
}
