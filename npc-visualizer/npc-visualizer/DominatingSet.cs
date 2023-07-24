using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SolverFoundation.Solvers;
using Microsoft.Msagl.Drawing;

namespace npc_visualizer
{
    class DominatingSet
    {
        public static int[] Solve(Graph g, int param)
        {
            int[] satVarToVertex = new int[param * g.NodeCount];
            Dictionary<int, int> indexToSatVar = new Dictionary<int, int>();

            Utilities.CreateMapping(satVarToVertex, indexToSatVar, g.NodeCount, param);
            int clauseCount = ClauseCount(g.NodeCount, param);

            Literal[][] clauses = new Literal[clauseCount][];
            DefineClauses(clauses, param, g, indexToSatVar);
            int varLim = g.NodeCount * param;
            IEnumerable<SatSolution> solutions = SatSolver.Solve(new SatSolverParams(), varLim, clauses);

            foreach (SatSolution solution in solutions)
            {
                //if inside, then there is a solution
                IEnumerable<int> positive = solution.Pos;
                int[] vertices = new int[param];
                int index = 0;
                foreach (int pos in positive)
                {
                    if (index < param)
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

        static void DefineClauses(Literal[][] clauses, int param, Graph g, Dictionary<int, int> indexToSatVar)
        {
            int clauseIndex = 0;

            //at most K vertices are chosen
            for (int i = 1; i < param + 1; i++)
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

            //the chosen vertex-set is a dominating set
            foreach (Node node in g.Nodes)
            {
                int[] adjacentNodes = AdjacentNodes(node, g);
                clauses[clauseIndex] = new Literal[adjacentNodes.Length * param];
                int literal = 0;
                
                for (int i = 1; i < param + 1; i++)
                {
                    for (int neighbour = 0; neighbour < adjacentNodes.Length; neighbour++)
                    {
                        clauses[clauseIndex][literal++] = new Literal(indexToSatVar[i * 1000 + adjacentNodes[neighbour]], true);
                    }
                }
                clauseIndex++;
            }
        }

        static int[] AdjacentNodes(Node node, Graph g)
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

        static int ClauseCount(int nodeCount, int param)
        {
            int seriesSum = (nodeCount - 1) * nodeCount / 2;

            return (param * seriesSum) + nodeCount;
        }
    }
}
