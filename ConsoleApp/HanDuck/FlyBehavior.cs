using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.HanDuck
{
    public class FlyBehavior : IFlyBehavior
    {
        public void Fly()
        {
            Console.WriteLine("WO 快要飞起来了");
        }
    }
}
