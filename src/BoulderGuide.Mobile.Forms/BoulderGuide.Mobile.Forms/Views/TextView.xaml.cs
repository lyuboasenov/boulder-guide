using Prism.Services.Dialogs;
using System;
using System.Text;
using System.Text.RegularExpressions;
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
         var cleanText = CleanMarkdown(Text);
         if ((Text?.Length ?? 0) > 130 || cleanText?.Split(new[] { Environment.NewLine }, StringSplitOptions.None)?.Length > 3) {
            lblText.Text = cleanText.Substring(0, Math.Min(cleanText.Length, 120)).Trim(' ', '\r', '\n') + " ...";
            lblMore.IsVisible = true;
         } else {
            lblText.Text = cleanText;
            lblMore.IsVisible = false;
         }

         IsVisible = !(string.IsNullOrEmpty(Text) && InvisibleIfNoText);
      }
      private static Regex regex = new Regex(
         @"\[(?<label>.+)\]\W*\((?<url>https?:.+)\)",
         RegexOptions.Compiled);
      private string CleanMarkdown(string text) {
         if (string.IsNullOrEmpty(text)) {
            // Happy path
            return string.Empty;
         }

         var sb = new StringBuilder();
         var matches = regex.Matches(text);
         var index = 0;
         if (matches.Count > 0) {
            foreach(Match match in matches) {
               if (match.Index > index) {
                  sb.Append(text.Substring(index, match.Index - index));
                  index = match.Index;
               }

               sb.Append(match.Groups["label"].Value);
               index = match.Index + match.Length;
            }

            if (index < text.Length) {
               sb.Append(text.Substring(index));
            }
         } else {
            sb.Append(text);
         }

         return sb.ToString();
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