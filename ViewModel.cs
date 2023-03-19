using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ZeroGraphProject
{
    using OxyPlot;
    using OxyPlot.Series;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Input;

    public class ViewModel : INotifyPropertyChanged/*, ICommand*/
    {
        private Model model = new Model();
        private readonly ObservableCollection<string> terminalLines = new ObservableCollection<string>();

        private int terminationIndex;
        private int formulaIndex;
        private int methodIndex;
        private double terminationConstant;
        private double leftBound;
        private double rightBound;

        private PlotModel plotModel = new PlotModel();

        ReplotCommand replotCommand = new ReplotCommand();

        public ViewModel()
        {
            /*this.Title = null;
            this.Points = new List<DataPoint> { new DataPoint(0, 4), new DataPoint(10, 13), new DataPoint(20, 15), new DataPoint(30, 16), new DataPoint(40, 12), new DataPoint(50, 12) };*/
            this.terminalLines.CollectionChanged += TerminalLines_CollectionChanged;

            this.FormulaIndex = 0;
            this.MethodIndex = 0;
            this.TerminationIndex = 0;
            this.TerminationConstant = 10;
            this.LeftBound = -2.5;
            this.RightBound = 2.5;
            this.PlotModel.Series.Add(new FunctionSeries(model.functions[FormulaIndex], LeftBound, RightBound, 0.01));
            this.PlotModel.Series.Add(new FunctionSeries((x) => 0, LeftBound, RightBound, 0.01));
            this.terminalLines.Add("! ZeroFinder 2023");
            this.terminalLines.Add("! Loading successful");
            this.terminalLines.Add("! Awaiting user action");

            replotCommand.ExecuteReceived += ReplotCommand_ExecuteReceived;

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(300);
                    this.terminalLines.Add("!");
                }
            });
        }

        public void Replot()
        {
            this.PlotModel.Series.Clear();
            this.PlotModel.Series.Add(new FunctionSeries(model.functions[FormulaIndex], LeftBound, RightBound, 0.01));
            this.PlotModel.Series.Add(new FunctionSeries((x) => 0, LeftBound, RightBound, 0.01));
            this.PlotModel.InvalidatePlot(true);
            OnPropertyChanged(nameof(this.PlotModel));
        }

        private void TerminalLines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (terminalLines.Count > 20)
            {
                terminalLines.RemoveAt(0);
            }
            this.TerminalText = String.Join('\n', this.terminalLines);
            OnPropertyChanged(nameof(TerminalText));
            
        }

        /*public ZeroFinder.LimitMethod limitMethod { get; private set; }
public uint N { get; private set; }
public double Epsilon { get; private set; }
public string Title { get; private set; }
public IList<DataPoint> Points { get; private set; }*/

        public event PropertyChangedEventHandler PropertyChanged;


        public void OnPropertyChanged([CallerMemberName] string  propertyName=null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName != nameof(TerminalText))
            {
                terminalLines.Add($"Requested {propertyName} set {this.GetType().GetProperty(propertyName)?.GetValue(this)}");
            }
        }

        public void ReplotCommand_ExecuteReceived(object sender, EventArgs e)
        {
            Replot();
        }

        public int FormulaIndex
        {
            get => formulaIndex;
            set
            {
                formulaIndex = value;
                OnPropertyChanged(nameof(formulaIndex));
                Replot();
            }
        }
        public int MethodIndex
        {
            get => methodIndex;
            set
            {
                methodIndex = value;
                OnPropertyChanged(nameof(methodIndex));
            }
        }
        public int TerminationIndex
        {
            get => terminationIndex;
            set {
                terminationIndex = value;
                OnPropertyChanged(nameof(terminationIndex));
                OnPropertyChanged(nameof(TerminationSymbolic));
            }
        }
        public double TerminationConstant
        {
            get => terminationConstant;
            set
            {
                terminationConstant = value;
                OnPropertyChanged(nameof(terminationConstant));
            }
        }
        public string TerminationSymbolic { get => TerminationIndex == 0 ? "N =" : @"\varepsilon ="; }
        public double LeftBound
        {
            get => leftBound;
            set
            {
                leftBound = value;
                OnPropertyChanged(nameof(leftBound));
            }
        }
        public double RightBound
        {
            get => rightBound;
            set
            {
                rightBound = value;
                OnPropertyChanged(nameof(rightBound));
            }
        }
        public string TerminalText { get; private set; }

        public PlotModel PlotModel { get => this.plotModel; }

        public ReplotCommand ReplotCommand { get => this.replotCommand; }

    }
}
