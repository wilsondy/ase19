namespace hw4
{
    public class Some : Col
    {
        public Some(string colName) : base(colName){}
        public override bool AddCell(Cell cell){return false;}
    }
}