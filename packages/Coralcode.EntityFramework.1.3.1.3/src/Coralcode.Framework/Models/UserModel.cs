using System;
using System.Collections.Generic;

namespace Coralcode.Framework.Models{
    public class UserModel {

        public UserModel()
        {
            ActiveTime = DateTime.Now;
            Additions = new Dictionary<string, string>();
        }
        /// <summary>
        /// 用户id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 活跃时间
        /// </summary>
        public DateTime ActiveTime { get; set; }

        /// <summary>
        /// 登陆的认证信息
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 附加信息
        /// </summary>
        public Dictionary<string, string> Additions { get; set; } 
    }
}