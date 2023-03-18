using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroGraphProject
{
    public abstract class ZeroFinder
    {
        public enum LimitMethod { ByIters, ByAccuracy }

        protected Func<double, double> f;
        private double a;
        private double b;
        private double epsilon;
        private uint iters;
        private LimitMethod lim_method;

        public double Zero()
        {
            double a = this.a;
            double b = this.b;

            if(!OppositeSignsCheck(f(a), f(b)))
            {
                throw new OppositeSignsConditionUnsatisfiedException(a, b);
            }

            try
            {
                if (lim_method == LimitMethod.ByIters)
                {
                    for (uint i = 0; i < iters; i++)
                    {
                        (a, b) = GetShrinkedInterval(a, b);
                    }
                }
                else
                {
                    double old_x;
                    double new_x;
                    do
                    {
                        old_x = (a + b) / 2;
                        (a, b) = GetShrinkedInterval(a, b);
                        new_x = (a + b) / 2;
                    }
                    while (Math.Abs(new_x - old_x) >= epsilon);
                }
            }
            catch (RootFound solution)
            {
                return solution.Root;
            }
            return (a + b) / 2;

        }

        protected abstract double Kernel(double a, double b);

        private (double, double) GetShrinkedInterval(double a, double b)
        {
            double x_0 = Kernel(a, b);
            double f_x0 = f.Invoke(x_0);
            if (f_x0 == 0)
            {
                throw new RootFound(x_0);
            }
            if (OppositeSignsCheck(f.Invoke(a), f_x0))
            {
                return (a, x_0);
            }
            else
            {
                return (x_0, b);
            }
        }

        private bool OppositeSignsCheck(double a, double b)
        {
            return (a > 0 && b < 0) || (a < 0 && b > 0);
        }

        public Func<double, double> F { get => f; set => f = value; }
        public double A { get => a; set => a = value; }
        public double B { get => b; set => b = value; }
        public double Epsilon { get => epsilon; set => epsilon = value; }
        public uint Iters { get => iters; set => iters = value; }
        public LimitMethod Termination { get => lim_method; set => lim_method = value; }


        private class RootFound : Exception
        {
            private double root;

            public RootFound(double root)
            {
                this.root = root;
            }

            public double Root => root;
        }

        public class OppositeSignsConditionUnsatisfiedException : Exception
        {
            private double a;
            private double b;

            public OppositeSignsConditionUnsatisfiedException(double a, double b)
            {
                this.a = a;
                this.b = b;
            }
            public double A => a;
            public double B => b;
            public override string Message => $"The opposite signs condition was not satisfied given the range [{a}, {b}]";
        }
    }
}
