using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class StrategyContext
    {
        public IAlgorithmStrategy AlgorithmStrategy { get; set; }

        public void SetStrategy(IAlgorithmStrategy strategy)
        {
            AlgorithmStrategy = strategy;
        }
        public void Calculation(int a,int b)
        {
            AlgorithmStrategy.Calculation(a, b);
        }
    }
}
