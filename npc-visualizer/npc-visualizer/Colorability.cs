using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    class Colorability
    {
        public static int[] Solve(Graph g, int colors)
        {
            int[] satVarToIndex = new int[colors * g.NodeCount];
            Dictionary<int, int> indexToSatVar = new Dictionary<int, int>();

            CreateMapping(satVarToIndex, indexToSatVar, g.NodeCount, colors);
            int clauseCount = ClauseCount(g.NodeCount, colors, g.EdgeCount);

            Literal[][] clauses = new Literal[clauseCount][];
            DefineClauses(clauses, colors, g, indexToSatVar);
            int varLim = g.NodeCount * colors;
            IEnumerable<SatSolution> solutions = SatSolver.Solve(new SatSolverParams(), varLim, clauses);

            foreach (SatSolution solution in solutions)
            {
                //if inside, then there is a solution
                IEnumerable<int> positive = solution.Pos;
                int[] coloring = new int[g.NodeCount];
                foreach (int pos in positive)
                {
                    coloring[satVarToIndex[pos] % 1000] = satVarToIndex[pos] / 1000;
                }
                return coloring;
            }

            return new int[] { };
        }

        static void CreateMapping(int[] satVarToIndex, Dictionary<int, int> indexToSatVar, int nodeCount, int colors)
        {
            int satVar = 0;

            for (int i = 1; i < colors + 1; i++)
            {
                for (int vertexNum = 0; vertexNum < nodeCount; vertexNum++)
                {
                    indexToSatVar[i * 1000 + vertexNum] = satVar;
                    satVarToIndex[satVar++] = i * 1000 + vertexNum;
                }
            }
        }

        static void DefineClauses(Literal[][] clauses, int colors, Graph g, Dictionary<int, int> indexToSatVar)
        {
            int clauseIndex = 0;

            //the first part selects one color for each vertex
            for (int vertex = 0; vertex < g.NodeCount; vertex++)
            {
                //at most one color
                for (int i = 1; i < colors + 1; i++)
                {
                    for (int j = i + 1; j < colors + 1; j++)
                    {
                        clauses[clauseIndex++] = new Literal[2]
                        {
                            new Literal(indexToSatVar[i * 1000 + vertex], false),
                            new Literal(indexToSatVar[j * 1000 + vertex], false)
                        };
                    }
                }

                //at least one color
                clauses[clauseIndex] = new Literal[colors];
                for (int i = 0; i < colors; i++)
                {
                    clauses[clauseIndex][i] = new Literal(indexToSatVar[(i + 1) * 1000 + vertex], true);
                }
                clauseIndex++;
            }

            //the second part verifies that no conict-edge exists
            foreach (Edge edge in g.Edges)
            {
                for (int i = 1; i < colors + 1; i++)
                {
                    clauses[clauseIndex++] = new Literal[2]
                    {
                        new Literal(indexToSatVar[i * 1000 + int.Parse(edge.Source)], false),
                        new Literal(indexToSatVar[i * 1000 + int.Parse(edge.Target)], false)
                    };
                }
            }
        }

        static int ClauseCount(int nodeCount, int colors, int edgeCount)
        {
            int seriesSum = colors * (colors - 1) / 2;


            return (nodeCount * (seriesSum + 1)) + (edgeCount * colors);
        }
    }
}
