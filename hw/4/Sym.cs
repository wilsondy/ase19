using System.Text;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace hw4
{
    public class Sym : Col
    {
        private Dictionary<string,int> counts = new Dictionary<string, int>();
        public string Mode {get; private set;} = null;
        private int modeCount =0;
        public double Entropy {get {
            double sum = 0;
            foreach(var symbol in counts){
                var p = (double)symbol.Value/(double)N;
                sum -= p*Math.Log(p,2);

            }
            return sum;
        }
        }
        public Sym(string colName) : base(colName){
            CellType = CellType.Symbolic;
        }
        public override bool AddCell(Cell cell){

            if(!counts.ContainsKey(cell.RawValue))
                counts[cell.RawValue]=1;
            else    
                counts[cell.RawValue]++;

            if(modeCount < counts[cell.RawValue]){
                Mode = cell.RawValue;
                modeCount =counts[cell.RawValue];
            }
            base.N++;
            return true;
        }
        public override double Likelihood(string rawValue, double prior, double magicM)
        {
            double count = 0;
            if (counts.ContainsKey(rawValue))
                count = counts[rawValue];
            return (count  + magicM*prior) / (double) (N + magicM);
        }
        
        public override Col Clone()
        {
            Sym result = (Sym) this.MemberwiseClone();
            result.N = 0;
            result.counts = new Dictionary<string, int>();
            result.modeCount = 0;
            result.Mode = null;
            return result;
        }
        
         public override void ToTreeString(StringBuilder b, int c){
            base.ToTreeString(b,c);
            b.AppendLine("|\t|\t|\tadd: Sym1");
            b.AppendLine($"|\t|\t|\tcnt");
            foreach(var item in counts){
                b.AppendLine($"|\t|\t|\t|\t {item.Key}: {item.Value}");    
            }
            b.AppendLine($"|\t|\t|\tcol: {Index}");
            b.AppendLine($"|\t|\t|\tmode: {Mode}");
            b.AppendLine($"|\t|\t|\tmost: {modeCount}");
            b.AppendLine($"|\t|\t|\tn: {N}");

            b.AppendLine($"|\t|\t|\ttxt: {Name}");
        }

    }
}