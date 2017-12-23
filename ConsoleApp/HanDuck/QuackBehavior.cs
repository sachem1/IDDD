using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.HanDuck
{
    public class QuackBehavior : IQuackbehavior
    {
        public void Quack()
        {
            Console.WriteLine("呱呱，呱呱 ，呱呱呱。。。"); ;
        }
    }
}
