using System.Collections.Generic;

namespace Coralcode.Framework.Validator
{

    /// <summary>
    /// Data Annotations 验证提供者
    /// </summary>
    public class EntityValidatorProvider
    {
        /// <summary>
        /// Create a entity validator
        /// </summary>
        /// <returns></returns>
        public static IEntityValidator Validator
        {
            get { return new EntityValidator(); }
        }

        public IEnumerable<string> Validate<TEntity>(TEntity item)
            where TEntity : class
        {
            var validator = Validator;
            var result = new List<string>();
            if (validator.IsValid(result))
                return validator.GetInvalidMessages(result);
            return result;
        }
    }
}
