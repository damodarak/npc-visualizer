using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    class VertexCover
    {
        public static int[] Solve(Graph g, int coverSize)
        {
            int[] satVarToVertex = new int[coverSize * g.NodeCount];
            Dictionary<int, int> indexToSatVar = new Dictionary<int, int>();

            Utilities.CreateMapping(satVarToVertex, indexToSatVar, g.NodeCount, coverSize);
            int clauseCount = ClauseCount(g.NodeCount, coverSize, g.EdgeCount);

            Literal[][] clauses = new Literal[clauseCount][];
            DefineClauses(clauses, coverSize, g, indexToSatVar);
            int varLim = g.NodeCount * coverSize;
            IEnumerable<SatSolution> solutions = SatSolver.Solve(new SatSolverParams(), varLim, clauses);

            return Utilities.SatSolutionToVertices(solutions, coverSize, satVarToVertex);
        }

        static void DefineClauses(Literal[][] clauses, int vertexCover, Graph g, Dictionary<int, int> indexToSatVar)
        {
            int clauseIndex = 0;

            //at most K vertices are chosen
            for (int i = 1; i < vertexCover + 1; i++)
            {
                for (int vertexNum1 = 0; vertexNum1 < g.NodeCount; vertexNum1++)
                {
                    for (int vertexNum2 = vertexNum1 + 1; vertexNum2 < g.NodeCount; vertexNum2++)
                    {
                        clauses[clauseIndex++] = new Literal[2]
                        {
                            new Literal(indexToSatVar[i * 1000 + vertexNum1], false),
                            new Literal(indexToSatVar[i * 1000 + vertexNum2], false)
                        };
                    }                    
                }
            }

            //the chosen vertex-set is a vertex cover indeed
            foreach (Edge e in g.Edges)
            {
                clauses[clauseIndex] = new Literal[2 * vertexCover];
                int literal = 0;
                for (int i = 1; i < vertexCover + 1; i++)
                {
                    clauses[clauseIndex][literal++] = new Literal(indexToSatVar[i * 1000 + int.Parse(e.Source)], true);
                    clauses[clauseIndex][literal++] = new Literal(indexToSatVar[i * 1000 + int.Parse(e.Target)], true);                   
                }
                clauseIndex++;
            }
        }

        static int ClauseCount(int nodeCount, int coverSize, int edgeCount)
        {
            int seriesSum = (nodeCount - 1) * nodeCount / 2;

            return (coverSize * seriesSum) + edgeCount;
        }
    }
}
