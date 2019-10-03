using System;
using System.Collections.Generic;
using System.Text;

namespace hw4
{
    public abstract class Col
    {
        const char IGNORE_CHAR = '?';
        const char NUMERIC_CHAR = '$';
        const char MINIMIZE_CHAR = '<';
        const char MAXIMIZE_CHAR = '>';
        const char GOAL_CHAR = '!';

        public List<Cell> cells = new List<Cell>();
        public int N { get; protected set; }
        public virtual void AddCell(Cell cell)
        {            
            N++;
            cells.Add(cell);
        }
        public virtual Cell RemoveCell(Cell cell)
        {            
            N--;
            cells.Remove(cell);
            return cell;
        }
        public bool Ignore {get; private set;} = false;
        public string Name {get; private set;}
        public CellType CellType {get; set;}
        public bool Goal { get; private set; }
        public bool Minimize { get; private set; }
        public int Weight {get; private set;}
        public int Index { get; internal set; }
        public int Rank { get; set; }

        public abstract double Variety();
        public Col(){
           

        }

        public void SetName(string colName)
        {
            colName = colName.Trim();
            Ignore = colName.StartsWith(IGNORE_CHAR);
            Name = colName;    

            if(colName.IndexOf(NUMERIC_CHAR)<=2)//first or second char!
                CellType = CellType.Numeric;
        }
         public double Expect(Col other)
         {
             var n = N + other.N;
             return N / n * Variety() + other.N / n * other.Variety();
         }
        private static bool Test(char x, string s){
            int index = s.IndexOf(x); 
            return index >=0 && index <=1;
        }
        public static Col Create( string colName){
            Col output = null;
            colName = colName.Trim();
            bool hasMin = Test(MINIMIZE_CHAR, colName);
            bool hasMax = Test(MAXIMIZE_CHAR, colName);
            bool hasNumeric = Test(NUMERIC_CHAR, colName);
            bool hasGoal  = Test(GOAL_CHAR, colName);
            
            if(hasMin || hasMax || hasNumeric)
                output = new Num(colName);
            else 
                output = new Sym(colName);

                
            output.Goal = hasGoal || hasMin || hasMax;
            output.Minimize = hasMin;
            output.Weight = hasMin ? -1 : 1;

            return output;
        }
       

       
        public virtual void ToTreeString(StringBuilder b, int c, string prefix){    
        }

        public virtual double Likelihood(string rawValue, double prior, double magicM)
        {
            throw new NotImplementedException();
        }
        
        public virtual Col Clone()
        {
            Col result = (Col) this.MemberwiseClone();
            result.N = 0;
            result.cells = new List<Cell>();
            foreach (var cell in cells)
            {
                result.AddCell(cell);
            }
            return result;
        }

        public abstract bool IsCentralTendencyDifferent(Col other, double epsilon);
        
        public void SortNaturally()
        {
            cells.Sort((cell, cell1) =>
                cell.IsNumeric()
                    ?  cell.NumericValue.CompareTo(cell1.NumericValue)
                    : String.Compare(cell.RawValue, cell1.RawValue));
        }

        public (T, T) Split<T>(int index) where T: Col, new()
        {
            T left = new T() {Name = this.Name};
            T right = new T() {Name = this.Name};
            
            int i = 0;
            foreach (var c in cells)
            {
                if(i<index)
                    left.AddCell(c);
                else 
                    right.AddCell(c);
                i++;
            }

            return (left, right);
        }

    }
}