using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ZeroGraphProject
{
    using OxyPlot;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            this.Title = null;
            this.Points = new List<DataPoint> { new DataPoint(0, 4), new DataPoint(10, 13), new DataPoint(20, 15), new DataPoint(30, 16), new DataPoint(40, 12), new DataPoint(50, 12) };
        }
        public Model.LimitMethod limitMethod { get; private set; }
        public uint N { get; private set; }
        public double Epsilon { get; private set; }
        public string Title { get; private set; }
        public IList<DataPoint> Points { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
