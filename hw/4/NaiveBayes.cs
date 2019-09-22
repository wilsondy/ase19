using System;
using System.Collections.Generic;
using System.Linq;

namespace hw4
{
    public class NaiveBayes : IPredict
    {
        private int magicK = 1;
        public int BootstrapMin { get; set; }
        private Dictionary<string, Tbl> _classCounts = new Dictionary<string, Tbl>();
        private int _numRows = 0;
        public string AddRow(Row row, List<Col> cols)
        {
            _numRows++;
            string prediction = null;

            if (_numRows >= BootstrapMin)
            {
                prediction = Predict(row, cols[0].N);
            }
            Train(row, cols);

            return prediction;

        }

        private string Predict(Row row, int totalRows)
        {
            double best = Double.NegativeInfinity;
            string result = null;
            foreach (var classCount in _classCounts)
            {
                double nthings = _classCounts.Count; //THIS IS A GUESS
                var classRows = classCount.Value.Rows.Count;
                double like = (classRows + magicK) / (totalRows + magicK * nthings);
                double prior = like;
                like = Math.Log(like,2);
                
                
                for(int i=0;i< row.Cells.Count;i++)
                {
                    var col = classCount.Value.Cols[i];
                    
                    //Don't train on the class variable!
                    if (col.Goal || col.Ignore)
                        continue;
                    
                    var prob = col.Likelihood(row.Cells[i].RawValue, prior, 2);
                    
                    if (classCount.Value.Cols[i].CellType == CellType.Numeric)
                    {
                        like += Math.Log(prob,2);
                    }
                    else
                    {
                        //like += log( SymLike(thing.cols[c], x, prior, i.m) )
                        like += Math.Log(prob,2);
                    }
                }

                if (like > best || result == null)
                {
                    best = like;
                    result = classCount.Key;
                }
            }

            return result;
        }

        private void Train(Row row, List<Col> cols)
        {
            string goal = row.GetGoals().First().RawValue;

            if (!_classCounts.ContainsKey(goal))
            {
                List<Col> colsCopy = new List<Col>();
                foreach (var col in cols)
                {
                    colsCopy.Add(col.Clone());
                }
                _classCounts[goal] = new Tbl(colsCopy);
            }

            Tbl table = _classCounts[goal];
            table.AddRow(row);
        }
    }
}