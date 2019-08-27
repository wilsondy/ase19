using System.Numerics;
using System;
namespace hw1
{
    public class Col
    {
        public int N { get; private set; }
        public virtual bool AddNum(int n)
        {            
            N++;
            return true;
        }
        public virtual bool RemoveNum(int n)
        {            
            N--;
            return true;
        }


    }
    public class Sym : Col
    {

    }
    public class Some : Col
    {

    }

    public class Num : Col
    {

        private int Low  = 10^32 ;
   
        private int High = -1*10^32;
        public double Mu { get; private set; }
        public double SD { get; private set; }
        private double M2;

        public override bool AddNum(int n)
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
        private int StandardDeviation()
        {
            if (M2 < 0 || N < 2)
                return 0;
            return (int) Math.Sqrt((M2 / (N - 1)));
        }
        public override bool RemoveNum(int n)
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