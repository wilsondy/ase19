using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace hw2
{
    public enum CellType { Numeric, Symbolic };
    public class Tbl
    {

      //  bool HasHeader = false;
        List<Row> Rows = new List<Row>();
        List<Col> Cols = new List<Col>();

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

                  
                    if (row == 0 && values.Any(a => a.Trim().StartsWith("?") || a.Trim().StartsWith("$") || a.Trim().StartsWith("<")))
                    {
                        //we have a well-defined header
                        foreach (var c in values)
                        {
                            var newCol = Col.Create(c);
                            
                            Cols.Add(newCol);
                            
                            if(newCol.Ignore)
                                numIgnoreCols++;
                        }
                        row++;
                        continue;
                    }
                    else if(row == 0)
                    {
                        //we have no header defined
                        int j = 0;
                        foreach (var c in values)
                        {
                            Cols.Add(new Col("Col" + j));
                            j++;
                        }
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
                        var cell = currRow.AddCell(c.Trim(), Cols[col].CellType);
                        if(!cell.IsEmpty()){
                            numColValues++;
                            Cols[col].AddCell(cell);
                        }
                        col++;
                    }

                   
                    row++;
                    Rows.Add(currRow);

                    
                }


            }
        }

        public string ToTreeString(){
            StringBuilder b = new StringBuilder();
            b.AppendLine("t.cols");
            int c = 1;
            foreach(var col in Cols){
                col.ToTreeString(b, c);
            }
            b.AppendLine("t.rows");
            int r = 1;
            foreach(var row in Rows){
                row.ToTreeString(b, r);
                r++;
            }
            return b.ToString();
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

    }
        public class Row
        {
            public List<Cell> Cells = new List<Cell>();
            public Cell AddCell(string value, CellType t)
            {
                var cell = new Cell(value, t);
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
            public Cell(string raw, CellType t)
            {
                RawValue = raw.Split("#")[0].Trim();
                CellType = t;

                if (IsNumeric() && !IsEmpty())
                    if (!Double.TryParse(RawValue, out NumericValue)){
                        NumericValue = Double.NaN;
                        _parseFailed = true;
                    }
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
