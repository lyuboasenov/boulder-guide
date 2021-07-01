using System.ComponentModel;

namespace BoulderGuide.Wpf.Domain {
   public class BaseEntity : INotifyPropertyChanged {
      public event PropertyChangedEventHandler PropertyChanged;
      protected void RaisePropertyChanged(string propertyName) {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}