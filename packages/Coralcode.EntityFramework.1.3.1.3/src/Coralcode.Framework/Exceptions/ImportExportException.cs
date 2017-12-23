using System;
using System.Collections.Generic;

namespace Coralcode.Framework.Exceptions
{
    public class ImportExportException : CoralException
    {
        public ImportExportException()
        : this("导入导出错误"){ }

        public ImportExportException(string message)
            : base(message)
        {
        }

        public List<string> ErrorColumn { get; set; }

        public Dictionary<string, List<string>> DataErrorColumn { get; set; }
    }
}
