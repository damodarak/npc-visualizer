using System;
using Microsoft.Msagl.Drawing;
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
            // Isn't used in this particular set of NP-Complete problems
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
            Graph g = new Graph();
            int param = _3sat.Length;

            Dictionary<int, List<Node>> mapping = new Dictionary<int, List<Node>>();

            foreach (Literal[] clause in _3sat)
            {
                Node[] nodesFromClause = new Node[clause.Length];
                for (int i = 0; i < clause.Length; i++) // Create maximum of 3 vertices
                {
                    nodesFromClause[i] = g.AddNode(g.NodeCount.ToString());
                }

                // Creating triangles for each clause
                // Nested for loop but the maximum amount of edges is 3
                for (int i = 0; i < nodesFromClause.Length; i++)
                {
                    for (int j = i + 1; j < nodesFromClause.Length; j++)
                    {
                        Edge ed = g.AddEdge(nodesFromClause[i].Id.ToString(), nodesFromClause[j].Id.ToString());
                        ed.Attr.ArrowheadAtTarget = ArrowStyle.None;
                        ed.Attr.Id = nodesFromClause[i].Id + "_" + nodesFromClause[i].Id;
                    }
                }

                // Create edges for the opposite literals in SAT formula
                for (int i = 0; i < clause.Length; i++)
                {
                    int litSense = clause[i].Sense ? 1 : -1;
                    int litVar = clause[i].Var + 1;
                    int negatednLitaralDictKey = litVar * litSense * -1; // litVar is incremented by 1, that solves positive and negative 0

                    if (mapping.ContainsKey(negatednLitaralDictKey))
                    {
                        List<Node> nodes = mapping[negatednLitaralDictKey];
                        foreach (Node node in nodes)
                        {
                            Edge ed = g.AddEdge(node.Id, nodesFromClause[i].Id);
                            ed.Attr.ArrowheadAtTarget = ArrowStyle.None;
                            ed.Attr.Id = node.Id + "_" + nodesFromClause[i].Id;
                        }
                    }
                }

                // Add literals to mapping Dictionary
                for (int i = 0; i < clause.Length; i++)
                {
                    int litSense = clause[i].Sense ? 1 : -1;
                    int litVar = clause[i].Var + 1;
                    int literalDictKey = litVar * litSense;

                    if (mapping.ContainsKey(literalDictKey))
                    {
                        mapping[literalDictKey].Add(nodesFromClause[i]);
                    }
                    else
                    {
                        mapping[literalDictKey] = new List<Node> { nodesFromClause[i] };
                    }
                }
            }

            return new Tuple<Graph, int>(g, param);
        }

        public override Literal[][] ToSat()
        {
            return _3sat;
        }

        public override Tuple<Graph, int> ToVertexCover()
        {
            throw new NotImplementedException();
        }
    }
}
