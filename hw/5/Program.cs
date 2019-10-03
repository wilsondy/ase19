using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace hw4
{
    class Program
    {

        static void Main(string[] args)
        {
           RunData<Num>("data1.csv");
           RunData<Sym>("data2.csv");
        }

        private static void RunData<T>(string filename) where T: Col, new()
        {
            var t = new Tbl(new List<Col>());
           
            t.Read(filename);
            var splitter = new Div2();
            splitter.Split<T>(t,1,0);
            
            Console.Out.WriteLine($"----- Data File: {filename} ------ ");            
            PrintSplit(splitter,0,t);
        }

        private static void PrintSplit(Div2 splitter, int xColumn, Tbl t)
        {
            StringBuilder b = new StringBuilder();
            foreach (var y in splitter.ranges)
            {
                var indices = y.cells.Select(cell => cell.Index());
                Num xCol = new Num(t.Cols[xColumn].Name);
               
                    var myXFriends = t.Cols[xColumn].cells.Where(cell1 => indices.Contains(cell1.Index()));

                    foreach (var xf in myXFriends)
                        xCol.AddCell(xf);
               
                xCol.ToTreeString(b, 3,"x");
                b.Append("  |  ");
                y.ToTreeString(b,3, "y");
                b.AppendLine("");
            }
//            b.AppendLine($"{splitter.before.Variety()} {splitter.gain}");
            Console.Out.WriteLine(b.ToString());
        }
    }
}