using System;

namespace ZeroGraphProject
{
    public partial class Model
    {
        public double CalculateRoot(int functionIndex, RootFindingMethod method, ZeroFinder.LimitMethod terminationCondition, double terminationConstant, double intervalLeft, double intervalRight)
        {
            ZeroFinder zeroFinder = CreateZeroFinderInstance(method);
            zeroFinder.F = functions[functionIndex];
            zeroFinder.A = intervalLeft;
            zeroFinder.B = intervalRight;
            zeroFinder.Termination = terminationCondition;
            if (terminationCondition == ZeroFinder.LimitMethod.ByAccuracy)
            {
                zeroFinder.Epsilon = terminationConstant;
            }
            else if (terminationCondition == ZeroFinder.LimitMethod.ByIters)
            {
                zeroFinder.Iters = (uint)terminationConstant;
            }
            zeroFinder.IntervalShrinking += (s, e)=>
            {
                OnComputationMonitorSignalReceived(e);
            };

            double root = zeroFinder.Zero();
            this.LastZeroFinder = zeroFinder;
            return root;

        }

        private ZeroFinder CreateZeroFinderInstance(RootFindingMethod method)
        {
            if (method == RootFindingMethod.Bisection)
            {
                return new Bisection();
            }
            else
            {
                return new RegulaFalsi();
            }
        }

        public event EventHandler<ZeroFinder.IntervalShrinkingEventArgs> ComputationMonitorSignalReceived;
        private void OnComputationMonitorSignalReceived(ZeroFinder.IntervalShrinkingEventArgs e)
        {
            this.ComputationMonitorSignalReceived?.Invoke(this, e);
        }

        public Func<double, double>[] functions =
        { 
            new Func<double, double>((double x) => x*(x*(x+1)-2)-1),
            new Func<double, double>((double x) => Math.Cos(x)),
            new Func<double, double>((double x) => Math.Pow(2, x)-3),
            new Func<double, double>((double x) => Math.Cos(Math.Exp(x))),
            new Func<double, double>((double x) => Math.Pow(Math.Cos(2*x*x),2)-Math.Pow(2,Math.Sin(x*x)))
        };

        public enum RootFindingMethod { Bisection, RegulaFalsi };

        public ZeroFinder LastZeroFinder { get; private set; }


    }
}