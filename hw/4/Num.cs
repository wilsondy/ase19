using System;
using System.Text;

namespace hw4
{
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
        
        public override Col Clone()
        {
            Num result = (Num) this.MemberwiseClone();
            result.N = 0;
            result.Low  = Double.PositiveInfinity ;
            result.High = Double.NegativeInfinity;
            result.M2 = 0;
            result.SD = 0;
            return result;
        }
        
        public override double Likelihood(string rawValue, double prior, double magicM)
        {
            double x = 0;
            x = Double.Parse(rawValue);
            double variance = Math.Pow(SD, 2);
            double denom = Math.Sqrt(3.14159 * 2 * variance);
            double num = Math.Pow( 2.71828 , (-Math.Pow((x - Mu), 2) / (2 * variance + 0.0001)));
            return num / (denom + Double.Epsilon);
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