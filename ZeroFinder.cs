using System;

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
            ItersUsed = 0;
            try
            {
                if (lim_method == LimitMethod.ByIters)
                {
                    for (int i = 0; i < iters; i++)
                    {
                        (a, b) = GetShrinkedInterval(a, b, ItersUsed++); 
                    }
                }
                else
                {
                    double old_x;
                    double new_x;
                    do
                    {
                        old_x = (a + b) / 2;
                        (a, b) = GetShrinkedInterval(a, b, ItersUsed++); 
                        new_x = (a + b) / 2;
                    }
                    while (Math.Abs(new_x - old_x) >= epsilon);
                }
            }
            catch (RootFound solution)
            {
                MemZero = solution.Root;
                return solution.Root;
            }

            double root = Kernel(a, b);
            MemZero = root;
            return root;

        }

        protected abstract double Kernel(double a, double b);

        public event EventHandler<IntervalShrinkedEventArgs> IntervalShrinked;
        private (double, double) GetShrinkedInterval(double a, double b, int i)
        {
            double x_0 = Kernel(a, b);
            this.OnIntervalShrinked(x_0, i);
            double f_x0 = f.Invoke(x_0);
            if (f_x0 == 0)
            {
                MemA = x_0 - double.Epsilon;
                MemB = x_0 + double.Epsilon;
                throw new RootFound(x_0);
            }
            if (OppositeSignsCheck(f.Invoke(a), f_x0))
            {
                MemA = a;
                MemB = x_0;
                return (a, x_0);
            }
            else
            {
                MemA = x_0;
                MemB = b;
                return (x_0, b);
            }
        }

        public void OnIntervalShrinked(double x, int i)
        {
            this.IntervalShrinked?.Invoke(this, new IntervalShrinkedEventArgs(x, i));
        }


        public class IntervalShrinkedEventArgs : EventArgs
        {
            private double x;
            private int i;
            public IntervalShrinkedEventArgs(double x, int i)
            {
                this.x = x;
                this.i = i;
            }
            public double X => x;
            public double I => i;
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


        public int ItersUsed { get; private set; }
        public double MemZero { get; private set; }
        public double MemA { get; private set; }
        public double MemB { get; private set; }

        public double LeftAccuracy => MemZero - MemA;
        public double RightAccuracy => MemB - MemZero;

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
            public override string Message => $"The opposite signs condition was not satisfied given the range [{a}; {b}]";
        }
    }
}
