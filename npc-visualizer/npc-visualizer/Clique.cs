using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SolverFoundation.Solvers;
using Microsoft.Msagl.Drawing;

namespace npc_visualizer
{
    class Clique
    {
        public static int[] Solve(Graph g, int cliqueSize)
        {
            if (g.NodeCount == 0)
            {
                return new int[] { };
            }
            else if (cliqueSize == 1)
            {
                return new int[] { 0 };
            }

            int[] satVarToVertex = new int[cliqueSize * g.NodeCount];
            Dictionary<int, int> indexToSatVar = new Dictionary<int, int>();

            CreateMapping(satVarToVertex, indexToSatVar, g.NodeCount, cliqueSize);
            int clauseCount = ClauseCount(g.NodeCount, cliqueSize, g.EdgeCount);

            Literal[][] clauses = new Literal[clauseCount][];
            DefineClauses(clauses, cliqueSize, g, indexToSatVar);
            int varLim = g.NodeCount * cliqueSize;
            IEnumerable<SatSolution> solutions = SatSolver.Solve(new SatSolverParams(), varLim, clauses);

            foreach (SatSolution solution in solutions)
            {
                //if inside, then there is a solution
                IEnumerable<int> positive = solution.Pos;
                int[] vertices = new int[cliqueSize];
                int index = 0;
                foreach (int pos in positive)
                {
                    if(index < cliqueSize)
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
        static void CreateMapping(int[] satVarToVertex, Dictionary<int, int> indexToSatVar, int nodeCount, int cliqueSize)
        {
            int satVar = 0;

            for (int i = 1; i < cliqueSize + 1; i++)
            {
                for (int vertexNum = 0; vertexNum < nodeCount; vertexNum++)
                {
                    indexToSatVar[i * 1000 + vertexNum] = satVar;
                    satVarToVertex[satVar++] = vertexNum;
                }
            }
        }

        static int ClauseCount(int nodeCount, int cliqueSize, int edgeCount)
        {
            int seriesSum = (cliqueSize - 1) * cliqueSize / 2;
            int maxEdges = nodeCount * (nodeCount - 1) / 2;
            int missingEdges = maxEdges - edgeCount;

            return (seriesSum * nodeCount) + (seriesSum * missingEdges * 2) + cliqueSize;
        }

        static void DefineClauses(Literal[][] clauses, int cliqueSize, Graph g, Dictionary<int, int> indexToSatVar)
        {
            int clauseIndex = 0;
            int nodeCount = g.NodeCount;

            //ith and jth vertices in one clique are different
            for (int k = 0; k < nodeCount; k++)
            {
                for (int i = 1; i < cliqueSize + 1; i++)
                {
                    for (int j = i + 1; j < cliqueSize + 1; j++)
                    {
                        clauses[clauseIndex++] = new Literal[]
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
                for (int i = 1; i < cliqueSize + 1; i++)
                {
                    for (int j = i + 1; j < cliqueSize + 1; j++)
                    {
                        clauses[clauseIndex++] = new Literal[]
                        {
                        new Literal(indexToSatVar[i * 1000 + missingEdges[k].Item1], false),
                        new Literal(indexToSatVar[j * 1000 + missingEdges[k].Item2], false)
                        };

                        clauses[clauseIndex++] = new Literal[]
                        {
                        new Literal(indexToSatVar[j * 1000 + missingEdges[k].Item1], false),
                        new Literal(indexToSatVar[i * 1000 + missingEdges[k].Item2], false)
                        };
                    }
                }
            }

            //There is an ith vertex
            for (int i = 1; i < cliqueSize + 1; i++)
            {
                clauses[clauseIndex] = new Literal[g.NodeCount];
                for (int j = 0; j < g.NodeCount; j++)
                {
                    clauses[clauseIndex][j] = new Literal(indexToSatVar[i * 1000 + j], true);
                }

                clauseIndex++;
            }
        }
    }   
}
