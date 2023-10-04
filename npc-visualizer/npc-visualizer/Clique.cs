using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SolverFoundation.Solvers;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace npc_visualizer
{
    class Clique : Problem
    {
        public Clique(Graph g, int param) 
        {
            this.g = g;
            this.param = param;
        }

        protected override void ToSat()
        {
            satVarToVertex = new int[param * g.NodeCount];
            indexToSatVar = new Dictionary<int, int>();

            Utilities.CreateMapping(satVarToVertex, indexToSatVar, g.NodeCount, param);
            ClauseCount(g.NodeCount, param, g.EdgeCount);

            sat = new Literal[clauseCount][];
            DefineClauses();
        }
        public override void Solve()
        {
            if (param == 1)
            {
                solution = new int[] { 0 };
                return;
            }

            ToSat();

            int varLim = g.NodeCount * param;
            IEnumerable<SatSolution> satSolutions = SatSolver.Solve(new SatSolverParams(), varLim, sat);

            solution = Utilities.SatSolutionToVertices(satSolutions, param, satVarToVertex);
        }

        void ClauseCount(int nodeCount, int cliqueSize, int edgeCount)
        {
            int seriesSumClique = ((cliqueSize - 1) * cliqueSize) / 2;
            int seriesSumNodes = ((nodeCount - 1) * nodeCount) / 2;
            int maxEdges = (nodeCount * (nodeCount - 1)) / 2;
            int missingEdges = maxEdges - edgeCount;

            clauseCount =  (seriesSumClique * nodeCount) + (seriesSumClique * missingEdges * 2) + cliqueSize + (cliqueSize * seriesSumNodes);
        }

        protected override void DefineClauses()
        {
            int clauseIndex = 0;
            int nodeCount = g.NodeCount;

            //ith and jth vertices in one clique are different
            for (int k = 0; k < nodeCount; k++)
            {
                for (int i = 1; i < param + 1; i++)
                {
                    for (int j = i + 1; j < param + 1; j++)
                    {
                        sat[clauseIndex++] = new Literal[]
                        {
                            new Literal(indexToSatVar[i * 1000 + k], false),
                            new Literal(indexToSatVar[j * 1000 + k], false)
                        };
                    }
                }
            }

            //Any two vertices in the clique are connected
            Tuple<int, int>[] missingEdges = Utilities.FindMissingEdges(g);
            for (int k = 0; k < missingEdges.Length; k++)
            {
                for (int i = 1; i < param + 1; i++)
                {
                    for (int j = i + 1; j < param + 1; j++)
                    {
                        sat[clauseIndex++] = new Literal[]
                        {
                        new Literal(indexToSatVar[i * 1000 + missingEdges[k].Item1], false),
                        new Literal(indexToSatVar[j * 1000 + missingEdges[k].Item2], false)
                        };

                        sat[clauseIndex++] = new Literal[]
                        {
                        new Literal(indexToSatVar[j * 1000 + missingEdges[k].Item1], false),
                        new Literal(indexToSatVar[i * 1000 + missingEdges[k].Item2], false)
                        };
                    }
                }
            }

            //There is an ith vertex
            for (int i = 1; i < param + 1; i++)
            {
                sat[clauseIndex] = new Literal[g.NodeCount];
                for (int j = 0; j < g.NodeCount; j++)
                {
                    sat[clauseIndex][j] = new Literal(indexToSatVar[i * 1000 + j], true);
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
                        new Literal(indexToSatVar[i * 1000 + nodeNum1], false),
                        new Literal(indexToSatVar[i * 1000 + nodeNum2], false)
                        };
                    }
                }
            }
        }

        public override Graph ToClique()
        {
            return g;
        }

        public override Graph ToColorability()
        {
            throw new NotImplementedException();
        }

        public override Graph ToDominatingSet()
        {
            throw new NotImplementedException();
        }

        public override Graph ToHamilPath()
        {
            throw new NotImplementedException();
        }

        public override Graph ToIndepSet()
        {
            throw new NotImplementedException();
        }

        public override Graph ToVertexCover()
        {
            throw new NotImplementedException();
        }
    }   
}
