using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class Addition : IAlgorithmStrategy
    {
        public int Num1;
        public int Num2;
        public void Calculation(int a, int b)
        {
            Console.WriteLine("计算结果："+(a+b));
        }
    }
}
