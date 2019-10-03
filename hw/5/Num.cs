using System;
using System.Collections.Generic;
using System.Text;

namespace hw4
{
    public class Num : Col
    {

        public double Low  = Double.PositiveInfinity ;
        public double High = Double.NegativeInfinity;
        public double Mu { get; private set; }
        public double SD { get; private set; }
        private double M2;

        public Num() : base()
        {
            
        }

        public Num(string colName) : base()
        {
            SetName(colName);
        }

        public override void AddCell(Cell cell){

            if(cell.IsNumeric() && !cell.IsEmpty()){
                base.AddCell(cell);
                var n = cell.NumericValue;
                Low = Math.Min(n, Low);        
                High = Math.Max(n, High);

                double delta = n - Mu;
                Mu += delta / N;
                double delta2 = n - Mu;
                M2 += delta * delta2;

                SD = StandardDeviation();
            }
        }
        
        public override Col Clone()
        {
            Num result = (Num) this.MemberwiseClone();
            result.N = 0;
            result.Low  = Double.PositiveInfinity ;
            result.High = Double.NegativeInfinity;
            result.M2 = 0;
            result.SD = 0;
            
            result.cells = new List<Cell>();
            foreach (var cell in cells)
            {
                result.AddCell(cell);
            }
            return result;
        }

        public override bool IsCentralTendencyDifferent(Col other, double epsilon)
        {
            Num otherNum = other as Num;
            if (otherNum == null)
                return true;
            return Math.Abs(otherNum.Mu - this.Mu) >= epsilon;
        }

        public override bool Equals(object obj)
        {
            var input = obj as Num;
            if (input == null) 
                return false;
            return (Math.Abs(input.Mu - this.Mu) < Double.Epsilon*2);
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

        public override double Variety()
        {
            return SD;
        }

        public override void ToTreeString(StringBuilder b, int c, string prefix){
            
            b.Append($"{prefix}.n {N,3:D1} | {prefix}.lo:{Low,10:F4} {prefix}.hi {High,10:F4}");
            
        }

        private double StandardDeviation()
        {
            if (M2 < 0 || N < 2)
                return 0;
            return (double) Math.Sqrt((M2 / (N - 1)));
        }
        public override Cell RemoveCell(Cell cell)
        {
            base.RemoveCell(cell);
            var n = cell.NumericValue;
            
            if (N < 2) {
                SD = 0; 
            }

            double delta = n - Mu;
            Mu -= delta/N;
            double delta2 = n - Mu;

            M2 -= delta * delta2;
            SD = StandardDeviation();

            return cell;
        }

       
    }
}