namespace Coralcode.Framework.Mapper
{
    public class DataMapperProvider
    {
        public static IDataMapper  Mapper
        {
            get { return new EmitmapperDataMapper(); }
        }
    }
}
