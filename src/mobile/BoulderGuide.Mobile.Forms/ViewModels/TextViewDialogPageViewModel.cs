using Prism.Services.Dialogs;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class TextViewDialogPageViewModel : DialogViewModelBase {

      public string Title { get; set; }
      public string Text { get; set; }

      public override void OnDialogOpened(IDialogParameters parameters) {
         base.OnDialogOpened(parameters);

         if (parameters.TryGetValue(nameof(Title), out string title) &&
            parameters.TryGetValue(nameof(Text), out string text)) {
            Title = title;
            Text = text;
         } else {
            Close();
         }
      }

   }
}
