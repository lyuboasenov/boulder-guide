using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoulderGuide.Wpf.Domain {
   public class BaseEntity : INotifyPropertyChanged {
      public event PropertyChangedEventHandler PropertyChanged;
      protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? "something changed"));
      }
   }
}