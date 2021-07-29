using System;
using System.Diagnostics;
using System.Windows.Input;

namespace BoulderGuide.Wpf.ViewModels {
   public class VideoWindowViewModel : BaseViewModel<Views.VideoWindow> {
      public string Id { get; set; }
      public string Url { get; set; }
      public void OnUrlChanged() {
         (OpenInBrowserCommand as Command)?.RaiseCanExecuteChanged();
      }
      public string EmbededCode { get; set; }
      public ICommand OkCommand { get; }
      public ICommand CancelCommand { get; }
      public ICommand OpenInBrowserCommand { get; }

      public VideoWindowViewModel() {
         OkCommand = new Command(Ok);
         CancelCommand = new Command(Cancel);
         OpenInBrowserCommand = new Command(OpenInBrowser, () => Uri.TryCreate(Url, UriKind.Absolute, out _));
      }

      private void Cancel() {
         Close(false);
      }

      private void OpenInBrowser() {
         Process.Start(new ProcessStartInfo {
            FileName = Url,
            UseShellExecute = true
         });
      }

      private void Ok() {
         Close(true);
      }
   }
}
