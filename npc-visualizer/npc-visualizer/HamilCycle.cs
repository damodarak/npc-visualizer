using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphmapsWithMesh;
using Microsoft.SolverFoundation.Services;
using Microsoft.SolverFoundation.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace npc_visualizer
{
    class HamilCycle : Problem
    {
        public HamilCycle(Graph g, int param)
        {
            this.g = g;
            this.param = param;
        }
        public override Literal[][] ToSat()
        {
            indexToSatVar = new int[g.NodeCount, g.NodeCount + 1];

            CreateMapping();
            ClauseCount();

            sat = new Literal[clauseCount][];
            DefineClauses();

            return sat;
        }
        public override int[] Solve()
        {
            ToSat();
            int varLim = g.NodeCount * g.NodeCount;
            IEnumerable<SatSolution> solutions = SatSolver.Solve(new SatSolverParams(), varLim, sat);

            foreach (SatSolution solution in solutions)
            {
                //if inside, then there is a solution
                IEnumerable<int> positive = solution.Pos;
                int[] ordering = new int[g.NodeCount];
                foreach (int pos in positive)
                {
                    ordering[pos / g.NodeCount] = pos % g.NodeCount;
                }
                this.solution = ordering;
                return ordering;
            }

            this.solution = new int[] { };
            return new int[] { };
        }

        void CreateMapping()
        {
            int nodeCount = g.NodeCount;
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
            int seriesSum = g.NodeCount * (g.NodeCount - 1) / 2;
            int missingEdges = seriesSum - g.EdgeCount;

            clauseCount = 2 * (g.NodeCount * (seriesSum + 1)) + g.NodeCount * (g.NodeCount - 1) + 2 * missingEdges;
        }
        void DefineClauses()
        {
            int clauseIndex = 0;

            //In the first two parts the one-to-one numbering of the vertices is specified
            for (int index = 1; index < g.NodeCount + 1; index++)
            {
                //at most one vertex
                for (int vertexOne = 0; vertexOne < g.NodeCount; vertexOne++)
                {
                    for (int vertexTwo = vertexOne + 1; vertexTwo < g.NodeCount; vertexTwo++)
                    {
                        sat[clauseIndex++] = new Literal[2]
                        {
                            new Literal(indexToSatVar[vertexOne, index], false),
                            new Literal(indexToSatVar[vertexTwo, index], false)
                        };
                    }
                }

                //at least one vertex
                sat[clauseIndex] = new Literal[g.NodeCount];
                for (int vertex = 0; vertex < g.NodeCount; vertex++)
                {
                    sat[clauseIndex][vertex] = new Literal(indexToSatVar[vertex, index], true);
                }
                clauseIndex++;
            }

            for (int vertex = 0; vertex < g.NodeCount; vertex++)
            {
                //at most one index
                for (int i = 1; i < g.NodeCount + 1; i++)
                {
                    for (int j = i + 1; j < g.NodeCount + 1; j++)
                    {
                        sat[clauseIndex++] = new Literal[2]
                        {
                            new Literal(indexToSatVar[vertex, i], false),
                            new Literal(indexToSatVar[vertex, j], false)
                        };
                    }
                }

                //at least one index
                sat[clauseIndex] = new Literal[g.NodeCount];
                for (int i = 0; i < g.NodeCount; i++)
                {
                    sat[clauseIndex][i] = new Literal(indexToSatVar[vertex, i + 1], true);
                }
                clauseIndex++;
            }

            //In the third part it is verified that edges between the i-th and (i + 1)-th vertex exist for all 1 <= i < |V|
            for (int index = 1; index < g.NodeCount; index++)
            {
                for (int vertex = 0; vertex < g.NodeCount; vertex++)
                {
                    int[] adjacentNodes = Utilities.AdjacentNodes(g.FindNode(vertex.ToString()), g);
                    sat[clauseIndex] = new Literal[adjacentNodes.Length + 1];
                    sat[clauseIndex][0] = new Literal(indexToSatVar[vertex, index], false);
                    for (int adjNodeIndex = 0;  adjNodeIndex < adjacentNodes.Length; adjNodeIndex++)
                    {
                        sat[clauseIndex][adjNodeIndex + 1] = new Literal(indexToSatVar[adjacentNodes[adjNodeIndex], index + 1], true);
                    }
                    clauseIndex++;
                }
            }

            //In the fourth part it is verified that the first and the last vertex are connected
            var missingEdges = Utilities.FindMissingEdges(g);
            for(int missEdge = 0; missEdge < missingEdges.Length; missEdge++)
            {
                sat[clauseIndex++] = new Literal[]
                {
                new Literal(indexToSatVar[missingEdges[missEdge].Item1, 1], false),
                new Literal(indexToSatVar[missingEdges[missEdge].Item2, g.NodeCount], false)
                };

                sat[clauseIndex++] = new Literal[]
                {
                new Literal(indexToSatVar[missingEdges[missEdge].Item2, 1], false),
                new Literal(indexToSatVar[missingEdges[missEdge].Item1, g.NodeCount], false)
                };
            }
        }
        public override void DrawSolution()
        {
            if(solution.Length != g.NodeCount || solution.Length < 2)
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
                Microsoft.Msagl.Drawing.Edge edge = Utilities.EdgeById(g, edgeId);
                edge.Attr.ArrowheadAtSource = ArrowStyle.Diamond;
                edge.Attr.ArrowheadAtTarget = ArrowStyle.Diamond;
                edge.Attr.Color = Color.Red;
            }

            Microsoft.Msagl.Drawing.Edge lastEdge = Utilities.EdgeById(g, $"{solution[solution.Length - 1]}_{solution[0]}");
            lastEdge.Attr.ArrowheadAtSource = ArrowStyle.Diamond;
            lastEdge.Attr.ArrowheadAtTarget = ArrowStyle.Diamond;
            lastEdge.Attr.Color = Color.Red;
        }
        public override Tuple<Graph, int> ToClique()
        {
            throw new NotImplementedException();
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
            return new Tuple<Graph, int>(Utilities.CopyGraph(g), param);
        }
        public override Tuple<Graph, int> ToIndepSet()
        {
            throw new NotImplementedException();
        }
        public override Tuple<Graph, int> ToVertexCover()
        {
            throw new NotImplementedException();
        }
    }
}
