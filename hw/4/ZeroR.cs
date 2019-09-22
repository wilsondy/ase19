using System.Collections.Generic;
using System.Linq;

namespace hw4
{
    public class ZeroR : IPredict
    {
        private Dictionary<string, int> _classCounts = new Dictionary<string, int>();
        private int _totalRows = 0;

        public int BootstrapMin { get; set; }
        public string AddRow(Row row, List<Col> cols)
        {
            _totalRows++;
            string result = null;
            if(_totalRows >= BootstrapMin)
                result = _classCounts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            
            var goalClass = row.GetGoals().First().RawValue;
            if (!_classCounts.ContainsKey(goalClass))
                _classCounts[goalClass] = 1;
            else
                _classCounts[goalClass] += 1;

            return result;
        }
    }
}