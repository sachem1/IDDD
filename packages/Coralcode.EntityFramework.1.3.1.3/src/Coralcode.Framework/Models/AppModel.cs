using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralcode.Framework.Models
{
    public class AppModel
    {
        public long Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        /// <summary>
        /// 安全码
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }
    }
}
