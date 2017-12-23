using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Coralcode.Framework.Validator
{
    /// <summary>
    /// 基于 Data Annotations的验证 
    /// 使用IValidatableObject 
    /// 和使用ValidationAttribute来进行验证
    /// </summary>
    public class EntityValidator
        : IEntityValidator
    {
        #region Private Methods

        /// <summary>
        /// 接口验证
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errors">当错误列表,ref </param>
        void SetValidatableObjectErrors<TEntity>(TEntity item, List<string> errors) where TEntity : class
        {
            if (typeof(IValidatableObject).IsAssignableFrom(typeof(TEntity)))
            {
                var validationContext = new ValidationContext(item, null, null);

                var validationResults = ((IValidatableObject)item).Validate(validationContext);

                errors.AddRange(validationResults.Select(vr => vr.ErrorMessage));
            }
        }

        /// <summary>
        /// 属性验证
        /// </summary>
        /// <param name="item"></param>
        /// <param name="errors">当错误列表,ref</param>
        void SetValidationAttributeErrors<TEntity>(TEntity item, List<string> errors) where TEntity : class
        {
            var result = (TypeDescriptor.GetProperties(item)
                .Cast<PropertyDescriptor>()
                .SelectMany(property => property.Attributes.OfType<ValidationAttribute>(), (property, attribute) => new { property, attribute })
                .Where(data => !data.attribute.IsValid(data.property.GetValue(item)))
                .Select(data => data.attribute.FormatErrorMessage(string.Empty))).ToList();

            if (result.Any())
            {
                errors.AddRange(result);
            }
        }

        #endregion

        #region IEntityValidator Members
        /// <summary>
        /// 验证
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool IsValid<TEntity>(TEntity item) where TEntity : class
        {
            if (item == null)
                return false;

            var validationErrors = new List<string>();

            SetValidatableObjectErrors(item, validationErrors);
            SetValidationAttributeErrors(item, validationErrors);

            return !validationErrors.Any();
        }

        /// <summary>
        /// 获取错误消息
        /// </summary>
        public IEnumerable<string> GetInvalidMessages<TEntity>(TEntity item) where TEntity : class
        {
            if (item == null)
                return null;
            var validationErrors = new List<string>();
            SetValidatableObjectErrors(item, validationErrors);
            SetValidationAttributeErrors(item, validationErrors);
            return validationErrors;
        }
        #endregion
    }
}
