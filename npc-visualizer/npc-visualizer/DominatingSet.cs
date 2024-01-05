using System;
using System.Collections.Generic;

using Microsoft.SolverFoundation.Solvers;
using Microsoft.Msagl.Drawing;

namespace npc_visualizer
{
    class DominatingSet : GraphProblem
    {
        public DominatingSet(Graph g, int param)
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

            solution =  GraphUtilities.SatSolutionToVertices(solutions, Param, satVarToVertex);

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

            //the chosen vertex-set is a dominating set
            foreach (Node node in G.Nodes)
            {
                int[] adjacentNodes = GraphUtilities.AdjacentNodes(node, G);
                sat[clauseIndex] = new Literal[(adjacentNodes.Length + 1) * Param];
                int literal = 0;
                
                for (int i = 1; i < Param + 1; i++)
                {
                    sat[clauseIndex][literal++] = new Literal(indexToSatVar[int.Parse(node.Id), i], true);
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
            int nodeCount = G.NodeCount;
            int seriesSum = (nodeCount - 1) * nodeCount / 2;

            clauseCount =  (Param * seriesSum) + nodeCount;
        }
        public override Clique ToClique()
        {
            throw new NotImplementedException();
        }
        public override Colorability ToColorability()
        {
            throw new NotImplementedException();
        }
        public override DominatingSet ToDominatingSet()
        {
            return new DominatingSet(GraphUtilities.CopyGraph(G), Param);
        }
        public override HamilCycle ToHamilCycle()
        {
            throw new NotImplementedException();
        }
        public override IndepSet ToIndepSet()
        {
            ToSat();
            _3Sat reduction3Sat = new _3Sat(this.sat);
            return reduction3Sat.ToIndepSet();
        }
        public override VertexCover ToVertexCover()
        {
            throw new NotImplementedException();
        }
    }
}
