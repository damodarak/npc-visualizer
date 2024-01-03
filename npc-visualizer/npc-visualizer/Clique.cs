using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SolverFoundation.Solvers;
using Microsoft.Msagl.Drawing;

namespace npc_visualizer
{
    class Clique : Problem
    {
        public Clique(Graph g, int param) 
        {
            this.g = g;
            this.param = param;
        }

        public override Literal[][] ToSat()
        {
            satVarToVertex = new int[param * g.NodeCount];
            indexToSatVar = new int[g.NodeCount, param + 1];

            GraphUtilities.CreateMapping(satVarToVertex, indexToSatVar, g.NodeCount, param);
            ClauseCount();

            sat = new Literal[clauseCount][];
            DefineClauses();

            return sat;
        }
        public override int[] Solve()
        {
            if (param == 1)
            {
                solution = new int[] { 0 };
                return solution;
            }

            ToSat();
            int varLim = g.NodeCount * param;
            IEnumerable<SatSolution> satSolutions = SatSolver.Solve(new SatSolverParams(), varLim, sat);

            solution = GraphUtilities.SatSolutionToVertices(satSolutions, param, satVarToVertex);

            return solution;
        }

        void ClauseCount()
        {
            int nodeCount = g.NodeCount;
            int edgeCount = g.EdgeCount;

            int seriesSumClique = ((param - 1) * param) / 2;
            int seriesSumNodes = ((nodeCount - 1) * nodeCount) / 2;
            int maxEdges = (nodeCount * (nodeCount - 1)) / 2;
            int missingEdges = maxEdges - edgeCount;

            clauseCount =  (seriesSumClique * nodeCount) + (seriesSumClique * missingEdges * 2) + param + (param * seriesSumNodes);
        }

        void DefineClauses()
        {
            int clauseIndex = 0;
            int nodeCount = g.NodeCount;

            //ith and jth vertices in one clique are different
            for (int vertex = 0; vertex < nodeCount; vertex++)
            {
                for (int i = 1; i < param + 1; i++)
                {
                    for (int j = i + 1; j < param + 1; j++)
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
            Tuple<int, int>[] missingEdges = GraphUtilities.FindMissingEdges(g);
            for (int missEdge = 0; missEdge < missingEdges.Length; missEdge++)
            {
                for (int i = 1; i < param + 1; i++)
                {
                    for (int j = i + 1; j < param + 1; j++)
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
            for (int i = 1; i < param + 1; i++)
            {
                sat[clauseIndex] = new Literal[g.NodeCount];
                for (int vertex = 0; vertex < g.NodeCount; vertex++)
                {
                    sat[clauseIndex][vertex] = new Literal(indexToSatVar[vertex, i], true);
                }

                clauseIndex++;
            }

            //there is only one ith vertex in the clique
            for (int i = 1; i < param + 1; i++)
            {
                for (int nodeNum1 = 0; nodeNum1 < g.NodeCount; nodeNum1++)
                {
                    for (int nodeNum2 = nodeNum1 + 1; nodeNum2 < g.NodeCount; nodeNum2++)
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
        public override Tuple<Graph, int> ToClique()
        {
            return new Tuple<Graph, int>(GraphUtilities.CopyGraph(g), param);
        }
        public override Tuple<Graph, int> ToColorability()
        {
            throw new NotImplementedException();
        }
        public override Tuple<Graph, int> ToDominatingSet()
        {
            throw new NotImplementedException();
        }
        public override Tuple<Graph, int> ToHamilCycle()
        {
            throw new NotImplementedException();
        }
        public override Tuple<Graph, int> ToIndepSet()
        {
            Graph flippedGraph = GraphUtilities.FlipEdges(g);
            return new Tuple<Graph, int>(flippedGraph, param);
        }
        public override Tuple<Graph, int> ToVertexCover()
        {
            Graph flippedGraph = GraphUtilities.FlipEdges(g);
            return new Tuple<Graph, int>(flippedGraph, g.NodeCount - param);
        }
    }   
}
