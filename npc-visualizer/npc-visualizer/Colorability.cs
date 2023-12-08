﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Msagl.Drawing;
using Microsoft.SolverFoundation.Solvers;

namespace npc_visualizer
{
    class Colorability : Problem
    {
        public Colorability(Graph g, int param)
        {
            this.g = g;
            this.param = param;
        }
        public override Literal[][] ToSat()
        {
            indexToSatVar = new int[g.NodeCount, param + 1];

            CreateMapping(indexToSatVar, g.NodeCount, param);
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

            foreach (SatSolution solution in solutions)
            {
                //if inside, then there is a solution
                IEnumerable<int> positive = solution.Pos;
                int[] coloring = new int[g.NodeCount];
                foreach (int pos in positive)
                {
                    coloring[pos % g.NodeCount] = pos / g.NodeCount;
                }
                this.solution = coloring;
                return coloring;
            }

            this.solution = new int[] { };
            return new int[] { };
        }
        static void CreateMapping(int[,] indexToSatVar, int nodeCount, int colors)
        {
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

            //the first part selects one color for each vertex
            for (int vertex = 0; vertex < g.NodeCount; vertex++)
            {
                //at most one color
                for (int i = 1; i < param + 1; i++)
                {
                    for (int j = i + 1; j < param + 1; j++)
                    {
                        sat[clauseIndex++] = new Literal[2]
                        {
                            new Literal(indexToSatVar[vertex, i], false),
                            new Literal(indexToSatVar[vertex, j], false)
                        };
                    }
                }

                //at least one color
                sat[clauseIndex] = new Literal[param];
                for (int i = 1; i < param + 1; i++)
                {
                    sat[clauseIndex][i] = new Literal(indexToSatVar[vertex, i], true);
                }
                clauseIndex++;
            }

            //the second part verifies that no conict-edge exists
            foreach (Edge edge in g.Edges)
            {
                for (int i = 1; i < param + 1; i++)
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
            int seriesSum = param * (param - 1) / 2;

            clauseCount = (g.NodeCount * (seriesSum + 1)) + (g.EdgeCount * param);
        }
        public override void DrawSolution()
        {
            Color[] colors = new Color[20]
            {
                Color.Blue, Color.Brown, Color.BlueViolet, Color.DarkGreen, Color.Gold, Color.Indigo, Color.Lime,
                Color.Magenta, Color.MistyRose, Color.Olive, Color.Orange, Color.Red, Color.Purple, Color.Silver,
                Color.Snow, Color.Tan, Color.White, Color.Yellow, Color.LightCoral, Color.LemonChiffon
            };

            for (int i = 0; i < solution.Length; i++)
            {
                g.FindNode(i.ToString()).Attr.FillColor = colors[solution[i]];
            }
        }
        public override Graph ToClique()
        {
            throw new NotImplementedException();
        }
        public override Graph ToColorability()
        {
            return g;
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
