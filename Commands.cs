using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZeroGraphProject
{
    public class ButtonCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public event EventHandler ExecuteReceived;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ExecuteReceived?.Invoke(this, new EventArgs());
        }
    }

}
