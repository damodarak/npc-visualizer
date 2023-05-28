using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SolverFoundation.Solvers;

namespace test_sat_solver
{
    class Program
    {
        static void Main(string[] args)
        {
            SatSolverParams parameters = new SatSolverParams();
            int limVar = 4;//nejvyssi literal v CNF formuli + 1
            Literal[][] clauses = new Literal[][]
            {
            new Literal[] { new Literal(1, true), new Literal(2, true), new Literal(3, true) },   // Clause 1: A or B or C
            new Literal[] { new Literal(1, false), new Literal(2, false), new Literal(3, true) },  // Clause 2: not A or not B or C
            new Literal[] { new Literal(1, false), new Literal(2, true), new Literal(3, false) },  // Clause 3: not A or B or not C
            new Literal[] { new Literal(1, true), new Literal(2, false), new Literal(3, false) }   // Clause 4: A or not B or not C
            };

            IEnumerable<SatSolution> solutions = SatSolver.Solve(parameters, limVar, clauses);

            bool hasSol = false;

            foreach (SatSolution solution in solutions)
            {
                hasSol = true;
                bool[] sol = new bool[3];
                IEnumerable<Literal> lits = solution.Literals;
                foreach (Literal lit in lits)
                {
                    sol[lit.Var - 1] = lit.Sense;
                    //Console.WriteLine(lit.Sense);
                    //Console.WriteLine(lit.Var);
                }               

                Console.WriteLine($"Solution found: 1 = {sol[0]}, 2 = {sol[1]}, 3 = {sol[2]}");
            }

            if (!hasSol)
            {
                Console.WriteLine("No solution found.");
            }
            else
            {
                Console.WriteLine("There are solutions");
            }

            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
    }
}
