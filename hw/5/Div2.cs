using System;
using System.Collections.Generic;
using System.Linq;

namespace hw4
{
    public class Div2
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

        public void Split<T>(Tbl table, int colToSplitOn, int otherColumn) where T: Col, new()
        {
            
            table.Cols[colToSplitOn].SortNaturally();
            before = table.Cols[colToSplitOn].Clone();
            var other = table.Cols[otherColumn];
            step = (int) Math.Pow(before.cells.Count, min);

            start = before.cells.First();
            stop = before.cells.Last();
            ranges = new List<Col>();
            epsilon = other.Variety() * cohen;
            SplitInternal<T>(before,  1);
            //gain /= len(i._lst);
        }

        private int SplitInternal<T>( Col before , int rank) where T: Col, new()
        {
            (var left, var right) = before.Split<T>(1);
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
                    if (left.IsCentralTendencyDifferent(right, epsilon))
                    {
                        if (after.IsDifferent( start, epsilon))
                        {
                            if (stop.IsDifferent ( now, epsilon))
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
                (left, right) = before.Split<T>(cut.Value);
                rank = SplitInternal<T>(left, rank) + 1;
                rank = SplitInternal<T>(right, rank);
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