namespace Coralcode.Framework.Cache {
    public interface IVolatileToken {
        bool IsCurrent { get; }
    }
}