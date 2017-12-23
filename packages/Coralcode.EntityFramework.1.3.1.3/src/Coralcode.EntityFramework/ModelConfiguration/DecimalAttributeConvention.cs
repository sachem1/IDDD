using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using Coralcode.Framework.Validator.DataAnnotations;

namespace Coralcode.EntityFramework.ModelConfiguration
{
    public class DecimalAttributeConvention
        : PrimitivePropertyAttributeConfigurationConvention<DecimalAttribute>
    {
        public override void Apply(ConventionPrimitivePropertyConfiguration configuration, DecimalAttribute attribute)
        {
            configuration.HasPrecision(attribute.Precision, attribute.Scale);
        }
    }
}