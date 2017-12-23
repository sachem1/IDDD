using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.HanDuck
{
    public class RubberDuck :Duck
    {
        public RubberDuck()
        {
            Quackbehavior = new QuackBehavior();
            SweepingBehavior = new SweepingBehavior();
        }

        public override void Display()
        {
            Console.WriteLine("我是一只橡皮鸭！");
            var isd=Power(1, 2);
            Console.WriteLine("我是一只橡皮鸭！isd:"+ isd);
        }

        public IEnumerable Power(int number, int exponent)
        {
            int counter = 0;
            int result = 1;
            while (counter++ < exponent)
            {
                result = result * number;
                yield return result;
            }
        }

    }
}
