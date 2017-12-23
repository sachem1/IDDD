using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.HanDuck
{
    public abstract class Duck
    {
        public IFlyBehavior FlyBehavior { get; set; }
        public IQuackbehavior Quackbehavior { get; set; }
        public ISweepingBehavior SweepingBehavior { get; set; }

        public abstract void Display();
        public void PerformFly()
        {
            FlyBehavior.Fly();
        }
        public void PerformQuack()
        {
            Quackbehavior.Quack();
        }
        public void PerformSweeping()
        {
            SweepingBehavior.Swimming();
        }

       
    }
}
