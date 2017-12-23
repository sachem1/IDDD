namespace Coralcode.Framework.Aspect.Unity
{
    public class ConstructorParameter
    {
        internal object Value { get;private set; }

        public ConstructorParameter(object value)
        {
            Value = value;
        }
    }
}
