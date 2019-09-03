using System.Text;
using System.Numerics;
using System;
namespace hw2
{
    public class Col
    {
        const char IGNORE_CHAR = '?';
        const char NUMERIC_CHAR = '$';
        public int N { get; private set; }
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
        public CellType CellType {get;private set;}
        
        public Col(string colName){
            colName = colName.Trim();
            Ignore = colName.StartsWith(IGNORE_CHAR);
            Name = colName;    

            if(colName.IndexOf(NUMERIC_CHAR)<=2)//first or second char!
                CellType = CellType.Numeric;

        }
        public static Col Create( string colName){
             if(colName.IndexOf(NUMERIC_CHAR)<=2)//first or second char!
                return new Num(colName);
            else
                return new Col(colName);

        }

        public virtual bool AddCell(Cell cell){return false;}
        public virtual void ToTreeString(StringBuilder b, int c){
            b.Append("|").Append("\t").Append(c).AppendLine("");            
        }

    }
    public class Sym : Col
    {
        public Sym(string colName) : base(colName){}
        public override bool AddCell(Cell cell){return false;}

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
            b.AppendLine($"|\t|\t|\tcol: {c}");
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