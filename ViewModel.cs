using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ZeroGraphProject
{
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Input;

    public class ViewModel : INotifyPropertyChanged
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

        ButtonCommand replotCommand = new ButtonCommand();
        ButtonCommand calculateRootCommand = new ButtonCommand();
        ButtonCommand printRootCommand = new ButtonCommand();
        ButtonCommand clearTerminalCommand = new ButtonCommand();


        public ViewModel()
        {
            
            this.terminalLines.CollectionChanged += TerminalLines_CollectionChanged;

            this.FormulaIndex = 0;
            this.MethodIndex = 0;
            this.TerminationIndex = 0;
            this.TerminationConstant = 10;
            this.LeftBound = -2.5;
            this.RightBound = 2.5;
            this.InitPlot();
            this.terminalLines.Add("! ZeroFinder 2023");
            this.terminalLines.Add("! Loading successful");
            this.terminalLines.Add("! Awaiting user action");
            this.CalculateRootButtonIsEnabled = true;

            replotCommand.ExecuteReceived += ReplotCommand_ExecuteReceived;
            calculateRootCommand.ExecuteReceived += CalculateRootCommand_ExecuteReceived;
            printRootCommand.ExecuteReceived += PrintRootCommand_ExecuteReceived;
            clearTerminalCommand.ExecuteReceived += ClearTerminalCommand_ExecuteReceived;

            model.ComputationMonitorSignalReceived += Model_ComputationMonitorSignalReceived;

        }

        private void InitPlot()
        {
            this.PlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            });
            this.PlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            });
            this.PlotModel.Series.Add(new FunctionSeries(model.functions[FormulaIndex], LeftBound, RightBound, 0.01));
            this.PlotModel.Series.Add(new FunctionSeries((x) => 0, LeftBound, RightBound, 0.01));


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
            terminalLines.Add("Reading ButtonCommand");
            Replot();
        }
        public void CalculateRootCommand_ExecuteReceived(object sender, EventArgs e)
        {
            terminalLines.Add("Reading CalculateRootCommand");
            Task.Run(() =>
            {
                InTaskMonitoredCalculation();
            });
            
        }

        private double InTaskMonitoredCalculation()
        {
            CalculateRootButtonIsEnabled = false;
            terminalLines.Add($"Calculating root...");
            double root = Double.NaN;
            try
            {
                root = model.CalculateRoot(FormulaIndex, (Model.RootFindingMethod)MethodIndex, (ZeroFinder.LimitMethod)TerminationIndex, TerminationConstant, LeftBound, RightBound);
                terminalLines.Add($"Task ended after {model.LastZeroFinder.ItersUsed} iterations");
                terminalLines.Add($"Root estimated at x = {root} [+{model.LastZeroFinder.RightAccuracy}][-{model.LastZeroFinder.LeftAccuracy}]");
            }
            catch (ZeroFinder.OppositeSignsConditionUnsatisfiedException ex)
            {

                terminalLines.Add($"!!! {ex.Message} !!!");
                terminalLines.Add($"Aborted root estimation");
            }
            finally
            {
                CalculateRootButtonIsEnabled = true;
            }
            return root;
        }

        private void Model_ComputationMonitorSignalReceived(object sender, ZeroFinder.IntervalShrinkedEventArgs e)
        {
            terminalLines.Add($"Computation monitor/> {e.X}    [after {e.I} iters]");
            Thread.Sleep(100);
        }

        public void PrintRootCommand_ExecuteReceived(object sender, EventArgs e)
        {
            terminalLines.Add("Reading PrintRootCommand");
            Task.Run(() =>
            {
                double x = InTaskMonitoredCalculation();
                if (!Double.IsNaN(x))
                {
                    double y = model.functions[formulaIndex].Invoke(x);
                    ScatterSeries series = new ScatterSeries
                    {
                        MarkerFill = MethodIndex == 0 ? OxyColors.Blue : OxyColors.Red,
                        MarkerType = MarkerType.Triangle,
                        MarkerSize = 10
                    };
                    series.Points.Add(new ScatterPoint(x, y));
                    Debug.WriteLine($"x: {x}; y: {y}");

                    this.PlotModel.Series.Add(series);
                    terminalLines.Add($"Solution marker at ({x}, {y})");
                    this.PlotModel.InvalidatePlot(true);
                }

            });
        }

        public void ClearTerminalCommand_ExecuteReceived(object sender, EventArgs e)
        {
            terminalLines.Clear();
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

        public ButtonCommand ReplotCommand { get => this.replotCommand; }
        public ButtonCommand CalculateRootCommand { get => this.calculateRootCommand; }
        public ButtonCommand PrintRootCommand { get => this.printRootCommand; }
        public ButtonCommand ClearTerminalCommand { get => this.clearTerminalCommand; }

        public bool CalculateRootButtonIsEnabled { get; private set; }


    }
}
