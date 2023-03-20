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

            if(!OppositeSignsCheck(f(a), f(b))) //sprawdź założenie o przeciwnych znakach funkcji na krańcach przedziału
            {
                throw new OppositeSignsConditionUnsatisfiedException(a, b);
            }
            ItersUsed = 0;
            try
            {
                if (lim_method == LimitMethod.ByIters) //sprawdź typ warunku stopu
                {
                    for (int i = 0; i < iters; i++)
                    {
                        (a, b) = GetShrinkedInterval(a, b, ItersUsed++); 
                    }
                }
                else
                {
                    double old_x; // aka x_(i-1)
                    double new_x; // aka x_i
                    do
                    {
                        old_x = (a + b) / 2;
                        (a, b) = GetShrinkedInterval(a, b, ItersUsed++); 
                        new_x = (a + b) / 2;
                    }
                    while (Math.Abs(new_x - old_x) >= epsilon); //sprawdzenie dokładności aka |x_i-x_(i-1)|>e (war. stopu niespełniony)
                }
            }
            catch (RootFound solution) //zwrócenie wyniku, gdy pierwiastek został znaleziony przed założoną liczbą iteracji / osiągnięciem warunku |x_(i+1)-x_i|<e
            {
                MemZero = solution.Root; //oprócz zwrócenia wyniku, zapamiętaj go w właściwości MemZero (aka Memory Zero)
                return solution.Root;
            }

            double root = Kernel(a, b); // patrz na metodę niżej
            MemZero = root;
            return root;

        }

        //Kernel zwraca dla bisekcji środek przedziału, a dla regula falsi punkt przecięcia prostej z osią OX
        protected abstract double Kernel(double a, double b);

        private (double, double) GetShrinkedInterval(double a, double b, int i)
        {
            double x_0 = Kernel(a, b);        //  Obliczenie wartości zwróconej przez kernel
            this.OnIntervalShrinking(x_0, i); //informowanie o zdarzeniu pomniejszania przedziału
            double f_x0 = f.Invoke(x_0);    // obliczenie f(x_0)
            if (f_x0 == 0) // jeśli f(x_0)=0 to bingo! Znaleźliśmy pierwiastek. W tym bloku kodu wyślemy go do metody wywołującej w formie wyjątku
            {
                MemA = x_0 - double.Epsilon; // ustawiamy promień przedziału na tak mały jak się da (ale niezerowy)
                MemB = x_0 + double.Epsilon; //
                throw new RootFound(x_0); // tutaj wysyłamy pierwiastek do metody wywołującej
            }
            if (OppositeSignsCheck(f.Invoke(a), f_x0)) //sprawdza czy pierwiastek jest z lewej strony x_0
            {
                MemA = a;   //
                MemB = x_0; // to są pola do zapamiętywania końcowego przedziału
                return (a, x_0); // zwraca lewą stronę x_0
            }
            else
            {
                MemA = x_0;
                MemB = b;
                return (x_0, b); //zwraca prawą stronę x_0
            }
        }

        public event EventHandler<IntervalShrinkingEventArgs> IntervalShrinking;

        public void OnIntervalShrinking(double x, int i)
        {
            this.IntervalShrinking?.Invoke(this, new IntervalShrinkingEventArgs(x, i));
        }


        public class IntervalShrinkingEventArgs : EventArgs
        {
            private double x;
            private int i;
            public IntervalShrinkingEventArgs(double x, int i)
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
