using System;
using System.Collections.Generic;
using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;


namespace npc_visualizer
{
    class HamilCycle : GraphProblem
    {
        public HamilCycle(Graph g, int param)
        {
            this.G = g;
            this.Param = param;
        }
        public override Literal[][] ToSat()
        {
            // Special case
            if (this.G.NodeCount == 2)
            {
                sat = new Literal[2][];
                sat[0] = new Literal[1] { new Literal(0, true) };
                sat[1] = new Literal[1] { new Literal(0, false) };
                return sat;
            }

            indexToSatVar = new int[G.NodeCount, G.NodeCount + 1];

            CreateMapping();
            ClauseCount();

            sat = new Literal[clauseCount][];
            DefineClauses();

            return sat;
        }
        public override int[] Solve()
        {
            ToSat();
            int varLim = G.NodeCount * G.NodeCount;
            IEnumerable<SatSolution> solutions = SatSolver.Solve(new SatSolverParams(), varLim, sat);

            foreach (SatSolution solution in solutions)
            {
                // If inside, then there is a solution
                IEnumerable<int> positive = solution.Pos;
                int[] ordering = new int[G.NodeCount];
                foreach (int pos in positive)
                {
                    ordering[pos / G.NodeCount] = pos % G.NodeCount;
                }
                this.solution = ordering;
                return ordering;
            }

            this.solution = null;
            return this.solution;
        }

        void CreateMapping()
        {
            int nodeCount = G.NodeCount;
            int satVar = 0;

            for (int i = 1; i < nodeCount + 1; i++)
            {
                for (int vertexNum = 0; vertexNum < nodeCount; vertexNum++)
                {
                    indexToSatVar[vertexNum, i] = satVar++;
                }
            }
        }
        void ClauseCount()
        {
            int seriesSum = G.NodeCount * (G.NodeCount - 1) / 2;
            int missingEdges = seriesSum - G.EdgeCount;

            clauseCount = 2 * (G.NodeCount * (seriesSum + 1)) + G.NodeCount * (G.NodeCount - 1) + 2 * missingEdges;
        }
        void DefineClauses()
        {
            int clauseIndex = 0;

            // In the first two parts the one-to-one numbering of the vertices is specified
            for (int index = 1; index < G.NodeCount + 1; index++)
            {
                // At most one vertex
                for (int vertexOne = 0; vertexOne < G.NodeCount; vertexOne++)
                {
                    for (int vertexTwo = vertexOne + 1; vertexTwo < G.NodeCount; vertexTwo++)
                    {
                        sat[clauseIndex++] = new Literal[2]
                        {
                            new Literal(indexToSatVar[vertexOne, index], false),
                            new Literal(indexToSatVar[vertexTwo, index], false)
                        };
                    }
                }

                //at least one vertex
                sat[clauseIndex] = new Literal[G.NodeCount];
                for (int vertex = 0; vertex < G.NodeCount; vertex++)
                {
                    sat[clauseIndex][vertex] = new Literal(indexToSatVar[vertex, index], true);
                }
                clauseIndex++;
            }

            for (int vertex = 0; vertex < G.NodeCount; vertex++)
            {
                // At most one index
                for (int i = 1; i < G.NodeCount + 1; i++)
                {
                    for (int j = i + 1; j < G.NodeCount + 1; j++)
                    {
                        sat[clauseIndex++] = new Literal[2]
                        {
                            new Literal(indexToSatVar[vertex, i], false),
                            new Literal(indexToSatVar[vertex, j], false)
                        };
                    }
                }

                // At least one index
                sat[clauseIndex] = new Literal[G.NodeCount];
                for (int i = 0; i < G.NodeCount; i++)
                {
                    sat[clauseIndex][i] = new Literal(indexToSatVar[vertex, i + 1], true);
                }
                clauseIndex++;
            }

            // In the third part it is verified that edges between the i-th and (i + 1)-th vertex exist for all 1 <= i < |V|
            for (int index = 1; index < G.NodeCount; index++)
            {
                for (int vertex = 0; vertex < G.NodeCount; vertex++)
                {
                    int[] adjacentNodes = GraphUtilities.AdjacentNodes(G.FindNode(vertex.ToString()), G);
                    sat[clauseIndex] = new Literal[adjacentNodes.Length + 1];
                    sat[clauseIndex][0] = new Literal(indexToSatVar[vertex, index], false);
                    for (int adjNodeIndex = 0;  adjNodeIndex < adjacentNodes.Length; adjNodeIndex++)
                    {
                        sat[clauseIndex][adjNodeIndex + 1] = new Literal(indexToSatVar[adjacentNodes[adjNodeIndex], index + 1], true);
                    }
                    clauseIndex++;
                }
            }

            // In the fourth part it is verified that the first and the last vertex are connected
            var missingEdges = GraphUtilities.FindMissingEdges(G);
            for (int missEdge = 0; missEdge < missingEdges.Length; missEdge++)
            {
                sat[clauseIndex++] = new Literal[]
                {
                new Literal(indexToSatVar[missingEdges[missEdge].Item1, 1], false),
                new Literal(indexToSatVar[missingEdges[missEdge].Item2, G.NodeCount], false)
                };

                sat[clauseIndex++] = new Literal[]
                {
                new Literal(indexToSatVar[missingEdges[missEdge].Item2, 1], false),
                new Literal(indexToSatVar[missingEdges[missEdge].Item1, G.NodeCount], false)
                };
            }
        }
        public override void DrawSolution()
        {
            if (solution == null || solution.Length != G.NodeCount || solution.Length < 2)
            {
                return;
            }

            for (int i = 0; i < solution.Length - 1; i++)
            {
                int source = solution[i];
                int target = solution[i + 1];
                int first, second;
                if (source > target)
                {
                    first = target;
                    second = source;
                }
                else
                {
                    first = source;
                    second = target;
                }

                string edgeId = $"{first}_{second}";
                Edge edge = GraphUtilities.EdgeById(G, edgeId);
                edge.Attr.ArrowheadAtSource = ArrowStyle.Diamond;
                edge.Attr.ArrowheadAtTarget = ArrowStyle.Diamond;
                edge.Attr.Color = Color.Red;
            }

            Edge lastEdge = GraphUtilities.EdgeById(G, $"{solution[solution.Length - 1]}_{solution[0]}");
            lastEdge.Attr.ArrowheadAtSource = ArrowStyle.Diamond;
            lastEdge.Attr.ArrowheadAtTarget = ArrowStyle.Diamond;
            lastEdge.Attr.Color = Color.Red;
        }
        public override Clique ToClique()
        {
            ToSat();
            _3Sat reduction3Sat = new _3Sat(this.sat);
            return reduction3Sat.ToClique();
        }
        public override Colorability ToColorability()
        {
            ToSat();
            _3Sat reduction3Sat = new _3Sat(this.sat);
            return reduction3Sat.ToColorability();
        }
        public override DominatingSet ToDominatingSet()
        {
            ToSat();
            _3Sat reduction3Sat = new _3Sat(this.sat);
            return reduction3Sat.ToDominatingSet();
        }
        public override HamilCycle ToHamilCycle()
        {
            return new HamilCycle(GraphUtilities.CopyGraph(G), Param);
        }
        public override IndepSet ToIndepSet()
        {
            ToSat();
            _3Sat reduction3Sat = new _3Sat(this.sat);
            return reduction3Sat.ToIndepSet();
        }
        public override VertexCover ToVertexCover()
        {
            ToSat();
            _3Sat reduction3Sat = new _3Sat(this.sat);
            return reduction3Sat.ToVertexCover();
        }
    }
}
