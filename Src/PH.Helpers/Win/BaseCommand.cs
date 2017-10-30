using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PH.Helpers.Win
{
   
    public class BaseCommand : ICommand
    {

        private readonly Action _execute;

        private readonly Func<bool> _canExecute;
        private readonly Action<bool> _setGeneralExecutingFlag;

        public BaseCommand(Action execute)
            : this(execute, null)
        {
        }

        public BaseCommand(Action execute, Func<bool> canExecute) :  this(execute, canExecute, null)
        {
        
        }

        public BaseCommand(Action execute, Func<bool> canExecute, Action<bool> setGeneralExecutingFlag)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
            _setGeneralExecutingFlag = setGeneralExecutingFlag;
        }

        #region "ICommand"

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged(object parameter)
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        [DebuggerStepThrough()]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            if (_setGeneralExecutingFlag != null)
                _setGeneralExecutingFlag(true);
            try
            {
                _execute();
            }
            finally
            {
                if (_setGeneralExecutingFlag != null)
                    _setGeneralExecutingFlag(false);
            }
        }

        #endregion
    }
}
