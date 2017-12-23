using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.Framework.Serializers.Dynamic
{
    public class XmlConfig : IConfig
    {
        public string PathOrSourceString { get; set; }
        public dynamic Data { get; set; }
    }
}
