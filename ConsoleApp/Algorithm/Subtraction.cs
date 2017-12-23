using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class Subtraction : IAlgorithmStrategy
    {
        public void Calculation(int a, int b)
        {
            Console.WriteLine("计算结果为："+(a-b));
        }
    }
}
