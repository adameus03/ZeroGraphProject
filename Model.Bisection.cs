using System;

namespace ZeroGraphProject
{
    public partial class Model
    {
        public class Bisection : ZeroFinder
        {
            protected override double Kernel(double a, double b)
            {
                return (a + b) / 2;
            }

        }


    }
}