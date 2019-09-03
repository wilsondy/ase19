using System;
using System.Collections.Generic; 

namespace hw2
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new Tbl();
            t.Read(args[0]);
            Console.Out.WriteLine(t.ToString());
           Console.Out.WriteLine(t.ToTreeString());
        }
    }
}
