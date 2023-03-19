using System;

namespace ZeroGraphProject
{
    public partial class Model
    {
        public class RegulaFalsi : ZeroFinder
        {
            protected override double Kernel(double a, double b)
            {
                return (a * f(b) - b * f(a)) / (f(b) - f(a));
            }
    
        }

    }
}
