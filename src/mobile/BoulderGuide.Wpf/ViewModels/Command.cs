using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace BoulderGuide.Wpf.ViewModels {
   internal class Command : ICommand {

      private Action _action;
      private Func<bool> _canExecute;


      private Action<object> _actionParam;
      private Func<object, bool> _canExecuteParam;

      public Command(Action action, Func<bool> canExecute = null) {
         _action = action;
         _canExecute = canExecute;
      }

      public Command(Action<object> actionParam, Func<object, bool> canExecuteParam = null) {
         _actionParam = actionParam;
         _canExecuteParam = canExecuteParam;
      }

      public bool CanExecute(object parameter) {
         if (_canExecuteParam != null) {
            return _canExecuteParam.Invoke(parameter);
         } else if (_canExecute != null) {
            return _canExecute.Invoke();
         } else {
            return true;
         }
      }

      public void Execute(object parameter) {
         if (_actionParam != null) {
            _actionParam.Invoke(parameter);
         } else if (_action != null) {
            _action.Invoke();
         }
      }

      public event EventHandler CanExecuteChanged;

      public void RaiseCanExecuteChanged() {
         CanExecuteChanged?.Invoke(this, EventArgs.Empty);
      }
   }
}
