using System.Text;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace hw3
{
    public class Col
    {
        const char IGNORE_CHAR = '?';
        const char NUMERIC_CHAR = '$';
        const char MINIMIZE_CHAR = '<';
        const char MAXIMIZE_CHAR = '>';
        const char GOAL_CHAR = '!';
        public int N { get; protected set; }
        public virtual bool AddNum(double n)
        {            
            N++;
            return true;
        }
        public virtual bool RemoveNum(double n)
        {            
            N--;
            return true;
        }
        public bool Ignore {get; private set;} = false;
        public string Name {get; private set;}
        public CellType CellType {get; set;}
        public bool Goal { get; private set; }
        public bool Minimize { get; private set; }
        public int Weight {get; private set;}
        public int Index { get; internal set; }

        public Col(string colName){
            colName = colName.Trim();
            Ignore = colName.StartsWith(IGNORE_CHAR);
            Name = colName;    

            if(colName.IndexOf(NUMERIC_CHAR)<=2)//first or second char!
                CellType = CellType.Numeric;

        }
        
         private static bool Test(char x, string s){
             int index = s.IndexOf(x); 
             return index >=0 && index <=1;
        }
        public static Col Create( string colName){
            Col output = null;
            colName = colName.Trim();
            bool hasMin = Test(MINIMIZE_CHAR, colName);
            bool hasMax = Test(MAXIMIZE_CHAR, colName);
            bool hasNumeric = Test(NUMERIC_CHAR, colName);
            bool hasGoal  = Test(GOAL_CHAR, colName);
            
             if(hasMin || hasMax || hasNumeric)
                output = new Num(colName);
            else 
                output = new Sym(colName);

                
                output.Goal = hasGoal || hasMin || hasMax;
                output.Minimize = hasMin;
                output.Weight = hasMin ? -1 : 1;

            return output;
        }
       

        public virtual bool AddCell(Cell cell){return false;}
        public virtual void ToTreeString(StringBuilder b, int c){
            b.Append("|").Append("\t").Append(c).AppendLine("");            
        }

    }
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
    public class Some : Col
    {
        public Some(string colName) : base(colName){}
        public override bool AddCell(Cell cell){return false;}
    }

    public class Num : Col
    {

        private double Low  = Double.PositiveInfinity ;
        private double High = Double.NegativeInfinity;
        public double Mu { get; private set; }
        public double SD { get; private set; }
        private double M2;

        public Num(string colName) : base(colName){}

        public override bool AddCell(Cell cell){
            if(cell.IsNumeric() && !cell.IsEmpty()){
                AddNum(cell.NumericValue);
                return true;
            }
            return false;

        }
        public override void ToTreeString(StringBuilder b, int c){
            base.ToTreeString(b,c);
            b.AppendLine("|\t|\t|\tadd: Num1");
            b.AppendLine($"|\t|\t|\tcol: {Index}");
            b.AppendLine($"|\t|\t|\thi: {High}");
            b.AppendLine($"|\t|\t|\tlo: {Low}");
            b.AppendLine($"|\t|\t|\tm2: {M2}");
            b.AppendLine($"|\t|\t|\tmu: {Mu}");
            b.AppendLine($"|\t|\t|\tn: {N}");
            b.AppendLine($"|\t|\t|\tsd: {SD}");
            b.AppendLine($"|\t|\t|\ttxt: {Name}");
        }


        public override bool AddNum(double n)
        {
            if (!base.AddNum(n))
                return false;
            Low = Math.Min(n, Low);        
            High = Math.Max(n, High);

            double delta = n - Mu;
            Mu += delta / N;
            double delta2 = n - Mu;
            M2 += delta * delta2;

            SD = StandardDeviation();
            return true;
        }
        private double StandardDeviation()
        {
            if (M2 < 0 || N < 2)
                return 0;
            return (double) Math.Sqrt((M2 / (N - 1)));
        }
        public override bool RemoveNum(double n)
        {
            if (!base.RemoveNum(n))
                return false;

            if (N < 2) {
                SD = 0; 
                return true;
            }

            double delta = n - Mu;
            Mu -= delta/N;
            double delta2 = n - Mu;

            M2 -= delta * delta2;
            SD = StandardDeviation();
            return true;
  
        }
    }
}