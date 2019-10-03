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

        public Sym() : base()
        {
            CellType = CellType.Symbolic;
        }

        public Sym(string colName) : base(){
            SetName(colName);
            CellType = CellType.Symbolic;
        }
        public override void AddCell(Cell cell){

            base.AddCell(cell);
            if(!counts.ContainsKey(cell.RawValue))
                counts[cell.RawValue]=1;
            else    
                counts[cell.RawValue]++;

            if(modeCount < counts[cell.RawValue]){
                Mode = cell.RawValue;
                modeCount =counts[cell.RawValue];
            }
            
        }
        
        public override bool IsCentralTendencyDifferent(Col other, double epsilon)
        {
            Sym otherSym = other as Sym;
            if (otherSym == null)
                return true;
            return otherSym.Mode != this.Mode;
        }

        
        public override double Likelihood(string rawValue, double prior, double magicM)
        {
            double count = 0;
            if (counts.ContainsKey(rawValue))
                count = counts[rawValue];
            return (count  + magicM*prior) / (double) (N + magicM);
        }
        public override double Variety()
        {
            return Entropy;
        }
        public override Col Clone()
        {
            Sym result = (Sym) this.MemberwiseClone();
            result.N = 0;
            result.counts = new Dictionary<string, int>();
            result.modeCount = 0;
            result.Mode = null;
            result.cells = new List<Cell>();
            foreach (var cell in cells)
            {
                result.AddCell(cell);
            }
            return result;
        }
        
         
         public override void ToTreeString(StringBuilder b, int c, string prefix){
             
             b.Append($"{prefix}.n {N} | {prefix}.mode:{Mode,4} {prefix}.ent {Variety()}");
            
         }
        

    }
}