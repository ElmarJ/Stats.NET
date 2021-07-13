using System;
using System.Collections.Generic;
using System.Text;
using MathLib;
using NGenerics.DataStructures;

namespace MathLibTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Matrix test = Matrix.IdentityMatrix(5);
            test[1, 3] = 3;
            test[1, 4] = 6;
            test[0, 1] = -2;
            test[0, 4] = -4;
            Console.WriteLine("Breedte: {0}", test.ColumnCount);
            Console.WriteLine("[2, 3]: {0}", test[2, 3]);
            Console.WriteLine("[3, 3]: {0}", test[3, 3]);
            Console.WriteLine("[1, 3]: {0}", test[1, 3]);
            Console.ReadLine();

            Matrix test2 = Matrix.NullMatrix(5, 4);
            test2[1, 2] = 6;
            test2[2, 3] = 4;

            Matrix test3 = test * test2;

            Console.WriteLine(test3);
            Console.WriteLine("Test: {0}", test);
            Console.WriteLine("Determinant test: {0}", test.Determinent);
            Console.WriteLine("Echelon test: {0}", test.EchelonForm);
            Console.WriteLine("Inverse test: {0}", test.Inverse);
            Console.ReadLine();
        }
    }
}
