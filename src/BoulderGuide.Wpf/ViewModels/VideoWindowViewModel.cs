using System.Windows.Input;

namespace BoulderGuide.Wpf.ViewModels {
   public class VideoWindowViewModel : BaseViewModel<Views.VideoWindow> {
      public string Id { get; set; }
      public string Url { get; set; }
      public string Embed { get; set; }
      public ICommand OkCommand { get; }
      public ICommand CancelCommand { get; }

      public VideoWindowViewModel() {
         OkCommand = new Command(Ok);
         CancelCommand = new Command(Cancel);
      }

      private void Cancel() {
         Close(false);
      }

      private void Ok() {
         Close(true);
      }
   }
}
