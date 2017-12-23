using ConsoleApp.HanDuck;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // 这种策略模式，必须知道有什么功能，否则出现错误
            // 行为都是一样的，虽然行为是组合的，但感觉就不是那么会事
            Duck woodduck = new WoodDuck();
            woodduck.Display();
            //woodduck.FlyBehavior.Fly();
            //woodduck.Quackbehavior.Quack();
            woodduck.SweepingBehavior.Swimming();

            RubberDuck rubberDuck = new RubberDuck();
            rubberDuck.Display();
            rubberDuck.SweepingBehavior.Swimming();
            Console.ReadLine();
        }


       
    }
}
