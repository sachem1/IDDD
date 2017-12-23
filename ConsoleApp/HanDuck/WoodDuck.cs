using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.HanDuck
{
    public class WoodDuck :Duck
    {
        public WoodDuck()
        {
            SweepingBehavior = new SweepingBehavior();
        }

        public override void Display()
        {
            Console.WriteLine("我是木头鸭！");
        }


       
    }
}
