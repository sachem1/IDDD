using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Coralcode.Framework.Validator.DataAnnotations
{
    /// <summary>
    /// 浮点类型指定整数位、小数位
    /// </summary>
    public class DecimalAttribute : ValidationAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="precision">整数位个数</param>
        /// <param name="scale">小数位</param>
        public DecimalAttribute(byte precision, byte scale)
        {
            this.Precision = precision;
            this.Scale = scale;
        }

        public byte Precision { get; set; }
        public byte Scale { get; set; }

        public override bool IsValid(object value)
        {
            if(!(value is decimal))
                return false;

            var d = (Decimal) value;
            var strDecimal = d.ToString(CultureInfo.InvariantCulture);

            var intPrecision = Convert.ToInt32(Precision);
            var intScale = Convert.ToInt32(Scale);

            if (strDecimal.IndexOf(".", StringComparison.Ordinal) < 0)
            {
                if(strDecimal.Length > intPrecision)
                    return false;
                return true;
            }

            var decimalSplit = strDecimal.Split('.');
            if (decimalSplit[0].Length > intPrecision)
                return false;

            if (decimalSplit[1].Length > intScale)
                return false;

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("{0}输入的数值大于该字段接受的最大值", name);
        }
    }
}