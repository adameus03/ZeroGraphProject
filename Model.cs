using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroGraphProject
{
    public class Model
    {
        public enum LimitMethod { ByIters, ByAccuracy }

        private Func<double, double> f;
        private double a;
        private double b;
        private double epsilon;
        private uint iters;
        private LimitMethod lim_method;

        public double BisectionZero()
        {
            double a = this.a;
            double b = this.b;
            if (lim_method == LimitMethod.ByIters)
            {
                for(uint i=0; i<iters; i++)
                {
                    (a,b) = GetShrinkedInterval(a, b);
                }
            }
            return (a + b) / 2;
            
        }
        private (double, double) GetShrinkedInterval(double a, double b)
        {
            double x_m = (a + b) / 2;
            if(OppositeSignsCheck(f.Invoke(a), f.Invoke(x_m))){
                return (a, x_m);
            }
            else
            {
                return (x_m, b);
            }
        }

        private bool OppositeSignsCheck(double a, double b)
        {
            return (a > 0 && b < 0) || (a < 0 && b > 0);
        }

        public Func<double,double> F { get => f; set => f = value; }
        public double A { get => a; set => a = value; }
        public double B { get => b; set => b = value; }
        public double Epsilon { get => epsilon; set => epsilon = value; }
        public uint Iters { get => iters; set => iters = value; }
        public LimitMethod Termination { get => lim_method; set => lim_method = value; }
    }
}
