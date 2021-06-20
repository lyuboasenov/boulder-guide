using Prism.Services.Dialogs;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BoulderGuide.Mobile.Forms.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class TextView : ContentView {
      public static readonly BindableProperty TitleProperty =
         BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(string),
            null,
            propertyChanged: (bindable, _, __) =>
               (bindable as TextView)?.UpdateTitle());

      private void UpdateTitle() {
         lblTitle.Text = Title;
      }

      public string Title {
         get { return (string) GetValue(TitleProperty); }
         set { SetValue(TitleProperty, value); }
      }

      public static readonly BindableProperty TextProperty =
         BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(string),
            null,
            propertyChanged: (bindable, _, __) =>
               (bindable as TextView)?.UpdateText());
      private bool invisibleIfNoText;

      private void UpdateText() {
         if ((Text?.Length ?? 0) > 130) {
            lblText.Text = Text.Substring(0, 120).Trim(' ', '\r', '\n') + " ...";
            lblMore.IsVisible = true;
         } else {
            lblText.Text = Text;
            lblMore.IsVisible = false;
         }

         IsVisible = !(string.IsNullOrEmpty(Text) && InvisibleIfNoText);
      }

      public string Text {
         get { return (string) GetValue(TextProperty); }
         set { SetValue(TextProperty, value); }
      }

      public bool InvisibleIfNoText {
         get {
            return invisibleIfNoText;
         }
         set {
            invisibleIfNoText = value;
            IsVisible = !(value && string.IsNullOrEmpty(Text));
         }
      }

      public TextView() {
         InitializeComponent();
      }

      private void TapGestureRecognizer_Tapped(object sender, EventArgs e) {
         var dialogService = (IDialogService) Prism.PrismApplicationBase.Current.Container.CurrentScope.Resolve(typeof(IDialogService));
         dialogService.ShowDialog(nameof(TextViewDialogPage), new DialogParameters() {
            { nameof(Title), Title },
            { nameof(Text), Text }
         });
      }
   }
}