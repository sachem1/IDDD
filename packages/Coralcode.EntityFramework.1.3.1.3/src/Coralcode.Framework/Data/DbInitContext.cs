using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Data;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Modules;
using NPOI.SS.Formula.Functions;

namespace Coralcode.Framework.Data
{
    public class DbInitContext
    {
        public DbInitContext(DbConfig config, DbModule module, DbMode mode)
        {
            Mode = mode;
            Config = config;
            Module = module;
        }

        public DbMode Mode { get; set; }

        public DbConfig Config { get; set; }

        public DbModule Module { get; set; }

        public string ConnectiongString
        {
            get
            {
                if (Mode == DbMode.Write)
                    return Config.WriteConnectionString;
                if (Mode == DbMode.Read)
                    return Config.ReadConnectionString;
                return Config.NameOrConnectionString;
            }
        }

        public List<Type> Types => Module.EntityTypes;

        public string GetIdentity()
        {
            return $"{Config.StaticCoden} {ConnectiongString}";
        }

        public static Func<Type, bool> IsEntity
        {
            get { return item => item.IsSubclassOf(typeof(Entity)); }
        }
    }
}
