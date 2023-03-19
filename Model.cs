using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroGraphProject
{
    public class Model
    {
        private RootFindingMethod method;

        public RootFindingMethod Method => method;



        public Func<double, double>[] functions =
        { 
            new Func<double, double>((double x) => x*(x*(x+1)-2)-1),
            new Func<double, double>((double x) => Math.Cos(x)),
            new Func<double, double>((double x) => Math.Pow(2, x)-3),
            new Func<double, double>((double x) => Math.Cos(Math.Exp(x))),
            new Func<double, double>((double x) => Math.Pow(Math.Cos(2*x*x),2)-Math.Pow(2,Math.Sin(x*x)))
        };

        public enum RootFindingMethod { Bisection, RegulaFalsi };
        public class Bisection : ZeroFinder
        {
            protected override double Kernel(double a, double b)
            {
                return (a + b) / 2;
            }
        }

        public class RegulaFalsi : ZeroFinder
        {
            protected override double Kernel(double a, double b)
            {
                return (a * f(b) - b * f(a)) / (b - a);
            }
        }


    }
}

/*public enum LimitMethod { ByIters, ByAccuracy }

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

    try
    {
        if (lim_method == LimitMethod.ByIters)
        {
            for (uint i = 0; i < iters; i++)
            {
                (a, b) = GetBisectionShrinkedInterval(a, b);
            }
        }
        else
        {
            double old_x;
            double new_x;
            do
            {
                old_x = (a + b) / 2;
                (a, b) = GetBisectionShrinkedInterval(a, b);
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
private (double, double) GetBisectionShrinkedInterval(double a, double b)
{
    double x_m = (a + b) / 2;
    double f_xm = f.Invoke(x_m);
    if (f_xm == 0)
    {
        throw new RootFound(x_m);
    }
    if (OppositeSignsCheck(f.Invoke(a), f_xm))
    {
        return (a, x_m);
    }
    else
    {
        return (x_m, b);
    }
}

private (double, double) GetRegulaFalsiShrinkedInterval(double a, double b)
{
    double x_0 = (a * f(b) - b * f(a)) / (b - a);
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
}*/