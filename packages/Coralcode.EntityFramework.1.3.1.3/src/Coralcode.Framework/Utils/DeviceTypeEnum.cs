using System;
using System.ComponentModel;

namespace Coralcode.Framework.Utils
{
    [Flags]
    public enum DeviceType
    {
        [Description("android")]
        Android=1,
        [Description("ios")]
        IOS=2,
        [Description("winphone")]
        WinPhone=4

    }
}