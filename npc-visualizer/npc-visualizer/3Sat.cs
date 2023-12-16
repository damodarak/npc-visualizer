using Microsoft.Msagl.Drawing;
using System;
using Microsoft.SolverFoundation.Solvers;
using System.Collections.Generic;

namespace npc_visualizer
{
    internal class _3Sat : Problem
    {
        Literal[][] _3sat;

        public _3Sat(Literal[][] sat)
        {
            this.sat = sat;
            this._3sat = To3Sat(sat);
        }

        Literal[][] To3Sat(Literal[][] sat)
        {
            List<List<Literal>> reduction = new List<List<Literal>>();
            int highest = findHighestLit();

            for (int i = 0; i < sat.Length; i++)
            {
                if (sat[i].Length < 4)
                {
                    List<Literal> clause = new List<Literal>(sat[i]);
                    reduction.Add(clause);
                }
                else
                {
                    List<Literal> clause = new List<Literal>(sat[i]);
                    divideClause(reduction, clause, ref highest);
                }
            }

            Literal[][] array = new Literal[reduction.Count][];
            for (int i = 0; i < reduction.Count; i++)
            {
                array[i] = reduction[i].ToArray();
            }

            return array;
        }

        void divideClause(List<List<Literal>> reduction, List<Literal> remaining, ref int highest)
        {
            if (remaining.Count < 4)
            {
                reduction.Add(remaining);
            }
            else
            {
                List<Literal> clause = new List<Literal>();
                for (int i = 0; i < 2; i++)
                {
                    clause.Add(remaining[0]);
                    remaining.RemoveAt(0);
                }
                clause.Add(new Literal(highest + 1, true));
                reduction.Add(clause);
                remaining.Insert(0, new Literal(++highest, false));
                divideClause(reduction, remaining, ref highest);
            }
        }

        int findHighestLit()
        {
            int highest = -1;

            for (int i = 0; i < sat.Length; i++)
            {
                for (int j = 0; j < sat[i].Length; j++)
                {
                    if (sat[i][j].Id > highest)
                    {
                        highest = sat[i][j].Id;
                    }
                }
            }

            return highest;
        }

        public override int[] Solve()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override Tuple<Graph, int> ToIndepSet()
        {
            throw new NotImplementedException();
        }

        public override Literal[][] ToSat()
        {
            throw new NotImplementedException();
        }

        public override Tuple<Graph, int> ToVertexCover()
        {
            throw new NotImplementedException();
        }
    }
}
