using System.Text;
using System;
using System.Collections.Generic;
namespace hw4
{
    class AbcdResult
    {
        public int A = 0, B = 0, C = 0, D = 0;
    }
    public class Abcd
    {

        int yes = 0;
        int no = 0;
        Dictionary<string, AbcdResult> known = new Dictionary<string, AbcdResult>();
        public string Label { get; set; }
        public void RecordPrediction(string actual, string prediction)
        {

            //Console.WriteLine($"{actual} VS {prediction}");
            
            if (!known.ContainsKey(prediction))
                InitializeKnown(prediction);
            if (!known.ContainsKey(actual))
                InitializeKnown(actual);

            if (actual == prediction)
                yes++;
            else
                no++;

            foreach (var key in known.Keys)
            {
                if (actual == key)
                {
                    if (actual == prediction)
                        known[key].D++;
                    else
                        known[key].B++;
                }
                else
                {
                    if (prediction == key)
                        known[key].C++;
                    else
                        known[key].A++;

                }
            }

        }

        public override string ToString(){
            const int w = 5;
            StringBuilder output = new StringBuilder();
             output.AppendLine($"{"num", w}|{"a", w}|{"b", w}|{"c", w}|{"d", w}|{"acc", w}|{"prec", w}|{"pd", w}|{"pf", w}|{"f", w}|{"g", w}|{"class", w+w}");
            foreach (var entry in known) {
                var x = entry.Value;
                double pd = 0, pf = 0, pn = 0, prec = 0,  g =0,  f =0, acc = 0;
                double a = x.A;
                double b = x.B;
                double c = x.C;
                double d = x.D;
                if (b+d > 0) pd = d/(b+d);
                if (a+c > 0) pf = c/(a+c); 
                if (a+c > 0) pn = (b+d) / (a+c);
                if (c+d > 0) prec = d / (c+d);
                if (1-pf+pd > 0 ) g=2*(1-pf) * pd / (1-pf+pd);
                if (prec+pd > 0 ) f=2*prec*pd / (prec + pd);
                if (yes + no > 0 ) 
                    acc  = (double)yes / (double)(yes + no);
                output.AppendLine($"{yes+no, w}|{a, w}|{b, w}|{c, w}|{d, w}|{acc, w:N2}|{prec,w:N2}|{pd, w:N2}|{pf, w:N2}|{f, w:N2}|{g, w:N2}|{entry.Key, w+w}");
            }
            return output.ToString();
       }

        private void InitializeKnown(string theClass)
        {
            known[theClass] = new AbcdResult() { A = yes + no };

        }
    }
}