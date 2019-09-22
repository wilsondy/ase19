using System;
using System.Text;

namespace hw4
{
    public class Col
    {
        const char IGNORE_CHAR = '?';
        const char NUMERIC_CHAR = '$';
        const char MINIMIZE_CHAR = '<';
        const char MAXIMIZE_CHAR = '>';
        const char GOAL_CHAR = '!';
        public int N { get; protected set; }
        public virtual bool AddNum(double n)
        {            
            N++;
            return true;
        }
        public virtual bool RemoveNum(double n)
        {            
            N--;
            return true;
        }
        public bool Ignore {get; private set;} = false;
        public string Name {get; private set;}
        public CellType CellType {get; set;}
        public bool Goal { get; private set; }
        public bool Minimize { get; private set; }
        public int Weight {get; private set;}
        public int Index { get; internal set; }

        public Col(string colName){
            colName = colName.Trim();
            Ignore = colName.StartsWith(IGNORE_CHAR);
            Name = colName;    

            if(colName.IndexOf(NUMERIC_CHAR)<=2)//first or second char!
                CellType = CellType.Numeric;

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
       

        public virtual bool AddCell(Cell cell){return false;}
        public virtual void ToTreeString(StringBuilder b, int c){
            b.Append("|").Append("\t").Append(c).AppendLine("");            
        }

        public virtual double Likelihood(string rawValue, double prior, double magicM)
        {
            throw new NotImplementedException();
        }
        
        public virtual Col Clone()
        {
            Col result = (Col) this.MemberwiseClone();
            result.N = 0;
            
            return result;
        }
    }
}