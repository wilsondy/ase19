using System.Linq;
using System;
using System.Collections.Generic;

namespace hw4
{
    class Program
    {
        static void Main(string[] args)
        {
           RunData("weathernon.csv");
           RunData("diabetes.csv");
        }

        private static void RunData(string filename)
        {
            var t = new Tbl(new List<Col>());
            t.AddPredictor(new ZeroR() {BootstrapMin = 3}, "ZeroR");
            t.AddPredictor(new NaiveBayes() {BootstrapMin = 4}, "NaiveBayes");
            t.Read(filename);
            var results = t.GetResults();
            Console.Out.WriteLine($"----- Data File: {filename} ------ ");
            foreach (var r in results)
            {
                Console.Out.WriteLine($"----- Learner: {r.Label} ------ ");
                Console.Out.WriteLine(r.ToString());
            }
        }
    }
}