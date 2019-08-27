using System;
using System.Collections.Generic; 

namespace hw1
{
    class Program
    {
        static void Main(string[] args)
        {
            int numcount =100;
            List<int> randoms = new List<int>();            

            List<double> means = new List<double>();
            List<double> deviations = new List<double>();
            List<double> meansRemove = new List<double>();
            List<double> deviationsRemove = new List<double>();

            Random random = new Random();
            for(int i=0;i<numcount;i++)
                randoms.Add(random.Next(32767));
                            
            var num = new Num();
            //add our randoms, caching every 10 nums - mean and sd.  Skip first group due to numerical issues            
            for(int i=0; i < numcount; i++){
                num.AddNum(randoms[i]);
                
                if(i%10==0 && i>numcount/10){
                    means.Add(num.Mu);
                    deviations.Add(num.SD);
                }
            }

            //remove in reverse, skip last 10
            for(int i=numcount-1; i>=0; i--){
                  if(i%10==0 && i>numcount/10){
                       meansRemove.Add(num.Mu);
                       deviationsRemove.Add(num.SD);
                }
                num.RemoveNum(randoms[i]);
            }
            meansRemove.Reverse();
            deviationsRemove.Reverse();

            for(int i=0;i<meansRemove.Count;i++){
                double meanDelta = means[i] - meansRemove[i];
                double sdDelta = deviations[i] - deviationsRemove[i];
                bool ok = (meanDelta + sdDelta < .0001);
                
                Console.WriteLine($" Group 0-{10*(i+2)} IsEqual() -> {ok} ( M1: {(int)means[i]} M2: {(int)meansRemove[i]} SD1: {deviations[i]} SD2: {deviationsRemove[i]} )");
            }


        }
    }
}
