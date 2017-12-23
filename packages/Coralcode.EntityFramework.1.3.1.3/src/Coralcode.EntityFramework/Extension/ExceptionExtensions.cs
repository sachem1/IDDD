using System;
using System.ComponentModel;
using System.Data.Entity.Validation;
using System.Linq;
using Coralcode.Framework.Reflection;

namespace Coralcode.EntityFramework.Extension {
    public static class ExceptionExtensions {
        public static Exception Format(this DbEntityValidationException exception) {
            return new Exception(string.Join(";", exception.EntityValidationErrors.Select(item => {
                var type = item.Entry.Entity.GetType();
                return string.Join(",", item.ValidationErrors.Select(error => {
                    var description = MetaDataManager.Attribute.GetPropertyAttribute<DescriptionAttribute>(type, error.PropertyName);
                    return description != null
                               ? error.ErrorMessage.Replace(error.PropertyName, description.Description)
                               : error.ErrorMessage;
                }));
            })));
        }
    }
}