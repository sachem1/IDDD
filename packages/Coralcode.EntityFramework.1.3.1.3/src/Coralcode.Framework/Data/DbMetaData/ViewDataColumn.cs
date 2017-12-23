namespace Coralcode.Framework.Data.DbMetaData
{
    /// <summary>
    /// 视图列
    /// </summary>
    public class ViewDataColumn
    {
        public string Name { get; set; }

        public string DataType { get; set; }



        public int Sort { get; set; }

        public string DefaultValue { get; set; }

        public bool IsNullable { get; set; }

        public int? CharacterMaximumLength { get; set; }

        public int? NumericPrecision { get; set; }

        public int? NumericScale { get; set; }


    }
}