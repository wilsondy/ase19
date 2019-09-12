using System.Linq;
using System;
using System.Collections.Generic;

namespace hw3
{
    class Program
    {
        static void Main(string[] args)
        {
            var sample = "aaaabbc";
            var entropyTest = sample.ToCharArray();
            var s = new Sym("test");
            foreach(var blah in entropyTest){
                s.AddCell(new Cell(""+blah, CellType.Symbolic));
            }
            Console.WriteLine($"Mode of {sample} is {s.Mode}");
            Console.WriteLine($"Entropy of {sample} is {s.Entropy}");

            var abcd = new Abcd();
            for (int j = 1; j <= 6; j++)
                abcd.RecordPrediction("yes", "yes");
            for (int j = 1; j <= 2; j++)
                abcd.RecordPrediction("no", "no");

            for (int j = 1; j <= 5; j++)
                abcd.RecordPrediction("maybe", "maybe");

            abcd.RecordPrediction("maybe", "no");
            var outpt = abcd.ToString();
            Console.WriteLine("\nOutput for sample Abcd problem: ");
            Console.WriteLine(outpt);


            var t = new Tbl();
            t.Read("weatherinput.csv");
            Console.Out.WriteLine(t.ToString());
            Console.Out.WriteLine(t.ToTreeString());
        }
    }
}