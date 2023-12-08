using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SolverFoundation.Solvers;
using Microsoft.Msagl.Drawing;

namespace npc_visualizer
{
    class DominatingSet : Problem
    {
        public DominatingSet(Graph g, int param)
        {
            this.g = g;
            this.param = param;
        }
        public override Literal[][] ToSat()
        {
            satVarToVertex = new int[param * g.NodeCount];
            indexToSatVar = new int[g.NodeCount, param + 1];

            Utilities.CreateMapping(satVarToVertex, indexToSatVar, g.NodeCount, param);
            ClauseCount();

            sat = new Literal[clauseCount][];
            DefineClauses();

            return sat;
        }
        public override int[] Solve()
        {
            ToSat();
            int varLim = g.NodeCount * param;
            IEnumerable<SatSolution> solutions = SatSolver.Solve(new SatSolverParams(), varLim, sat);

            solution =  Utilities.SatSolutionToVertices(solutions, param, satVarToVertex);

            return solution;
        }

        void DefineClauses()
        {
            int clauseIndex = 0;

            //at most K vertices are chosen
            for (int i = 1; i < param + 1; i++)
            {
                for (int vertexNum1 = 0; vertexNum1 < g.NodeCount; vertexNum1++)
                {
                    for (int vertexNum2 = vertexNum1 + 1; vertexNum2 < g.NodeCount; vertexNum2++)
                    {
                        sat[clauseIndex++] = new Literal[2]
                        {
                            new Literal(indexToSatVar[vertexNum1, i], false),
                            new Literal(indexToSatVar[vertexNum2, i], false)
                        };
                    }
                }
            }

            //the chosen vertex-set is a dominating set
            foreach (Node node in g.Nodes)
            {
                int[] adjacentNodes = Utilities.AdjacentNodes(node, g);
                sat[clauseIndex] = new Literal[adjacentNodes.Length * param];
                int literal = 0;
                
                for (int i = 1; i < param + 1; i++)
                {
                    for (int neighbour = 0; neighbour < adjacentNodes.Length; neighbour++)
                    {
                        sat[clauseIndex][literal++] = new Literal(indexToSatVar[adjacentNodes[neighbour], i], true);
                    }
                }
                clauseIndex++;
            }
        }
        void ClauseCount()
        {
            int nodeCount = g.NodeCount;
            int seriesSum = (nodeCount - 1) * nodeCount / 2;

            clauseCount =  (param * seriesSum) + nodeCount;
        }
        public override Graph ToClique()
        {
            throw new NotImplementedException();
        }
        public override Graph ToColorability()
        {
            throw new NotImplementedException();
        }
        public override Graph ToDominatingSet()
        {
            return g;
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
