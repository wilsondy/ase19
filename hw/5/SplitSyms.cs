using System;
using System.Collections.Generic;
using System.Linq;

namespace hw4
{
    public class SplitSyms
    {
        private static double trivial = 1.025;
        private static double cohen = 0.3;
        private static double min = 0.5;
        
        public Col before;
        public double gain = 0;
        private int step = 0;
        private Cell start;
        private Cell stop;
        private double epsilon;
        public List<Col> ranges;

        public void Split(Tbl table, int colToSplitOn, int otherColumn)
        {
            
            table.Cols[colToSplitOn].SortNaturally();
            before = table.Cols[colToSplitOn].Clone();
            var other = table.Cols[otherColumn];
            step = (int) Math.Pow(before.cells.Count, min);

            start = before.cells.First();
            stop = before.cells.Last();
            ranges = new List<Col>();
           // epsilon = ((Num) other).SD * cohen;
            SplitInternal(before,  1);
            //gain /= len(i._lst);
        }

        private int SplitInternal( Col before , int rank)
        {
            (Sym left, Sym right) = before.Split<Sym>(1);
            var best = before.Variety();
            int? cut = null;
            
            for (int j = 1; j < before.N; j++)
            {
                var now = left.cells.Last();
                left.AddCell(right.RemoveCell(right.cells.First()));
                var after = left.cells.Last();
                                
                if (left.N >= step && right.N >= step)
                {
                    if (now.RawValue.Equals(after.RawValue))
                        continue;
                    if (right.Mode != left.Mode)
                    {
                        if (after.RawValue != start.RawValue)
                        {
                            if (stop.RawValue != now.RawValue)
                            {
                                var expect = left.Expect(right);
                                if (expect * trivial < best)
                                {
                                    best = expect;
                                    cut = j;

                                }
                            }
                        }
                    }
                }
            }

            if (cut.HasValue)
            {
                (left, right) = before.Split<Sym>(cut.Value);
                rank = SplitInternal(left, rank) + 1;
                rank = SplitInternal(right, rank);
            }
            else
            {
                gain += before.N * before.Variety();
                before.Rank = rank;
                ranges.Add(before);
            }

            return rank;
        }
    }
}