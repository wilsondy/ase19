using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace hw4
{

    public interface IPredict
    {
        /// <summary>
        /// Min number of rows before prediction will begin
        /// </summary>
        int BootstrapMin { get; set; }
        /// <summary>
        /// Train and/or predict.
        /// If null is returned no prediction was attempted.
        /// </summary>
        /// <param name="row"></param>
        /// <returns>the prediction for the row or Null if non was attempted</returns>
        string AddRow(Row row, List<Col> cols);
        
    }
    
    public enum CellType { Numeric, Symbolic };
    public class Tbl
    {

      //  bool HasHeader = false;
      public List<Row> Rows = new List<Row>();
      public List<Col> Cols = new List<Col>();
        private List<IPredict> _predicts = new List<IPredict>();
        private List<Abcd> _predictReporter = new List<Abcd>();

        public Tbl(List<Col> cols)
        {
            Cols = cols;
        }

        public void AddPredictor(IPredict p, string label)
        {
            _predicts.Add(p);
            _predictReporter.Add(new Abcd() {Label = label});
        }

        public Abcd[] GetResults()
        {
            return _predictReporter.ToArray();
        }
        public void Read(string fileName)
        {
            int numIgnoreCols = 0;
            using (var reader = new StreamReader(fileName))
            {
                int row = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();
                   
                    var values = line.Split(',');

                    if(Cols.Count > 0 && Cols.Count() != values.Count()){ //toss mismatched
                        Console.Error.WriteLine("Possible Error on line " + row);                        
                    }

                  
                    if (row == 0 )
                    {
                        int colNum = 1;
                        //we have a well-defined header
                        foreach (var c in values)
                        {
                            var newCol = Col.Create(c.Trim());
                            newCol.Index = colNum;
                            colNum++;
                            Cols.Add(newCol);
                            
                            if(newCol.Ignore)
                                numIgnoreCols++;
                        }
                        row++;
                        continue;
                    }
                   
                    int col = 0;
                    var currRow = new Row();
                    int numColValues =0;
                    foreach (var c in values)
                    {
                        if (Cols[col].Ignore)
                        {
                            col++;
                            continue;
                        }
                        var cell = currRow.AddCell(c.Trim(), Cols[col].CellType, Cols[col]);
                        if(!cell.IsEmpty()){
                            numColValues++;
                            Cols[col].AddCell(cell);
                        }
                        col++;
                    }

                   
                    row++;
                    Rows.Add(currRow);
                    int predictor = 0;
                    foreach (var predict in _predicts)
                    {
                        var prediction = predict.AddRow(currRow, Cols);
                        if (prediction != null)
                        {
                            _predictReporter[predictor].RecordPrediction(currRow.GetGoals().First().RawValue, prediction);
                        }

                        predictor++;

                    }
                    
                }


            }
        }

        public string ToTreeString(){
            StringBuilder b = new StringBuilder();
            b.AppendLine("t.cols");
            int c = 1;
            List<int> goals = new List<int>();
            List<int> nums = new List<int>();
            List<int> syms = new List<int>();
            List<int> w = new List<int>();
            List<int> xnums = new List<int>();
            List<int> xs = new List<int>();
            List<int> xsyms = new List<int>();
            List<int> cls = new List<int>();

            foreach(var col in Cols){
                if(!col.Ignore)
                    col.ToTreeString(b, c);
                if(col.Ignore)
                    continue;
                
                if(col.Goal)
                    goals.Add(col.Index);
                if(col.CellType == CellType.Numeric){
                    nums.Add(col.Index);
                    if(!col.Goal){
                        xnums.Add(col.Index);
                        xs.Add(col.Index);
                    }
                }
                else{
                    syms.Add(col.Index);
                    if(!col.Goal){
                        xsyms.Add(col.Index);
                        xs.Add(col.Index);
                    }
                    else
                        cls.Add(col.Index);
                }
                

            }
            b.AppendLine("t.my");
            AddBits(cls, "class", b);
            AddBits(goals, "goals", b);
            AddBits(nums, "nums", b);
            AddBits(syms, "syms", b);
            b.AppendLine("|\tw");
            foreach(var col in Cols)
                if(col.Weight <0 )
                  b.AppendLine($"|\t|\t{col.Index}:{col.Weight}");
            AddBits(xnums, "xnums", b);
            AddBits(xs, "xs", b);
            AddBits(xsyms, "xsyms", b);
            return b.ToString();
        }

        public void AddBits(List<int> indexes, string label, StringBuilder b){
            b.AppendLine($"|\t{label}");
            foreach (var item in indexes)
            {
                b.AppendLine($"|\t|\t{item}");    
            }
            
        }

        public override string ToString(){
            StringBuilder header = new StringBuilder();
            foreach(var col in Cols){    
                if(!col.Ignore)              
                    header.Append(col.Name.PadRight(12));//.Append(","); //TODO this breaks down if col names get long.  
                
            }
            
            StringBuilder body = new StringBuilder(header.ToString().TrimEnd(','));
            body.AppendLine("");
            foreach(var row in Rows)
                body.AppendLine(row.ToString());

            return body.ToString();    
        }

        /// <summary>
        /// Ideally this would be used by the file reader - but for now we aren't doing that 
        /// </summary>
        /// <param name="row"></param>
        public void AddRow(Row row)
        {
            int col = 0;
            int numColValues =0;
            foreach (var c in row.Cells)
            {
                if (Cols[col].Ignore)
                {
                    col++;
                    continue;
                }
                if(!c.IsEmpty()){
                    numColValues++;
                    Cols[col].AddCell(c);
                }
                col++;
            }
            Rows.Add(row);
        }
    }
        public class Row
        {
            public List<Cell> Cells = new List<Cell>();
            public Cell AddCell(string value, CellType t, Col colDef)
            {
                var cell = new Cell(value, t, colDef);
                Cells.Add(cell);
                return cell;
            }
             public void ToTreeString(StringBuilder b, int c){
                 b.AppendLine($"|\t{c}");            
                b.AppendLine($"|\t|\tcells");            
                int i =1;
                foreach(var cell in Cells){
                    cell.ToTreeString(b, i);
                    i++;
                }
            }

             public IEnumerable<Cell> GetGoals()
             {
                 return Cells.Where(c => c.IsGoal());
             }

            public override string ToString(){
                StringBuilder b = new StringBuilder();
                foreach(var c in Cells){
                    b.Append(c);//.Append(",").;                    
                }
                return b.ToString().TrimEnd(',');
            }
        }

        public class Cell
        {

            //Null = no value
            //Empty = not sure
            public String RawValue = null;
            public CellType CellType;
            public Double NumericValue;
            private bool _parseFailed;
            private Col _colDef;

            public Cell(string raw, CellType t, Col colDef)
            {
                RawValue = raw.Split("#")[0].Trim();
                CellType = t;
                _colDef = colDef;
                if (IsNumeric() && !IsEmpty())
                    if (!Double.TryParse(RawValue, out NumericValue)){
                        NumericValue = Double.NaN;
                        _parseFailed = true;
                    }
            }

            public bool IsGoal()
            {
                return _colDef.Goal;
            }

            public bool IsNumeric()
            {
                return CellType == CellType.Numeric;
            }

            public bool IsEmpty()
            {
                return _parseFailed || String.IsNullOrEmpty(RawValue);
            }

            public void ToTreeString(StringBuilder b, int c){
                b.AppendLine($"|\t|\t|\t{c}: {ToString()}");            
            }

            public override string ToString()
            {
                string result = "";
                if (IsEmpty())
                    result = "?";
                else
                    result = RawValue;

                return result.PadRight(12);
            }
        }
    }
