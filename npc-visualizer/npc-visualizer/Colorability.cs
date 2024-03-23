using System;
using System.Collections.Generic;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    class Colorability : GraphProblem
    {
        public Colorability(Graph g, int param)
        {
            this.G = g;
            this.Param = param;
        }
        public override Literal[][] ToSat()
        {
            indexToSatVar = new int[G.NodeCount, Param + 1];

            CreateMapping();
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

            IEnumerator<SatSolution> solutionEnumerator = solutions.GetEnumerator();

            if (solutionEnumerator.MoveNext())
            {
                // If inside, then there is a solution
                IEnumerable<int> positive = solutionEnumerator.Current.Pos;
                int[] coloring = new int[G.NodeCount];
                foreach (int pos in positive)
                {
                    coloring[pos % G.NodeCount] = pos / G.NodeCount;
                }
                this.solution = coloring;
                return coloring;
            }

            this.solution = null;
            return this.solution;
        }
        void CreateMapping()
        {
            int nodeCount = G.NodeCount;
            int colors = Param;
            int satVar = 0;

            for (int i = 1; i < colors + 1; i++)
            {
                for (int vertexNum = 0; vertexNum < nodeCount; vertexNum++)
                {
                    indexToSatVar[vertexNum, i] = satVar++;
                }
            }
        }
        void DefineClauses()
        {
            int clauseIndex = 0;

            // The first part selects one color for each vertex
            for (int vertex = 0; vertex < G.NodeCount; vertex++)
            {
                // At most one color
                for (int i = 1; i < Param + 1; i++)
                {
                    for (int j = i + 1; j < Param + 1; j++)
                    {
                        sat[clauseIndex++] = new Literal[2]
                        {
                            new Literal(indexToSatVar[vertex, i], false),
                            new Literal(indexToSatVar[vertex, j], false)
                        };
                    }
                }

                // At least one color
                sat[clauseIndex] = new Literal[Param];
                for (int i = 0; i < Param; i++)
                {
                    sat[clauseIndex][i] = new Literal(indexToSatVar[vertex, i + 1], true);
                }
                clauseIndex++;
            }

            // The second part verifies that no conflict-edge exists
            foreach (Edge edge in G.Edges)
            {
                for (int i = 1; i < Param + 1; i++)
                {
                    sat[clauseIndex++] = new Literal[2]
                    {
                        new Literal(indexToSatVar[int.Parse(edge.Source), i], false),
                        new Literal(indexToSatVar[int.Parse(edge.Target), i], false)
                    };
                }
            }
        }
        void ClauseCount()
        {
            int seriesSum = Param * (Param - 1) / 2;

            clauseCount = (G.NodeCount * (seriesSum + 1)) + (G.EdgeCount * Param);
        }
        public override void DrawSolution()
        {
            if (solution == null)
            {
                return;
            }

            Color[] colors = new Color[20]
            {
                Color.Blue, Color.Brown, Color.BlueViolet, Color.DarkGreen, Color.Gold, Color.Indigo, Color.Lime,
                Color.Magenta, Color.MistyRose, Color.Olive, Color.Orange, Color.Red, Color.Purple, Color.Silver,
                Color.Snow, Color.Tan, Color.White, Color.Yellow, Color.LightCoral, Color.LemonChiffon
            };

            for (int i = 0; i < solution.Length; i++)
            {
                G.FindNode(i.ToString()).Attr.FillColor = colors[solution[i]];
            }
        }
        public override Clique ToClique()
        {
            ToSat();
            _3Sat reduction3Sat = new _3Sat(this.sat);
            return reduction3Sat.ToClique();
        }
        public override Colorability ToColorability()
        {
            return new Colorability(GraphUtilities.CopyGraph(G), Param);
        }
        public override DominatingSet ToDominatingSet()
        {
            ToSat();
            _3Sat reduction3Sat = new _3Sat(this.sat);
            return reduction3Sat.ToDominatingSet();
        }
        public override HamilCycle ToHamilCycle()
        {
            ToSat();
            _3Sat reduction3Sat = new _3Sat(this.sat);
            return reduction3Sat.ToHamilCycle();
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
