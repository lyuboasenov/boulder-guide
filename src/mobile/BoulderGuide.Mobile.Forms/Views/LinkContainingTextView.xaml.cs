using BoulderGuide.Mobile.Forms.Services.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BoulderGuide.Mobile.Forms.Views {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class LinkContainingTextView : ContentView {
      private Dictionary<object, string> urls = new Dictionary<object, string>();

      private static Regex regex = new Regex(
         @"\[(?<label>.+)\]\W*\((?<url>https?:.+)\)",
         RegexOptions.Compiled);

      public static readonly BindableProperty TextProperty =
         BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(string),
            null,
            propertyChanged: (bindable, _, __) =>
               (bindable as LinkContainingTextView)?.UpdateText());

      private void UpdateText() {
         container.Children.Clear();
         urls.Clear();
         var matches = regex.Matches(Text);
         var index = 0;

         if (matches.Count > 0) {
            foreach (Match match in matches) {
               if (match.Index > index) {
                  container.Children.Add(GetLabel(Text.Substring(index, match.Index - index)));
                  index = match.Index;
               }

               container.Children.Add(GetLink(match.Groups["label"].Value, match.Groups["url"].Value));
               index = match.Index + match.Length;
            }

            if (index < Text.Length) {
               container.Children.Add(GetLabel(Text.Substring(index)));
            }

         } else {
            container.Children.Add(GetLabel(Text));
         }
      }


      private View GetLink(string label, string url) {
         var view = new Label() {
            Text = label,
            TextColor = (Color) Application.Current.Resources["Accent"],
            Margin = 0,
            Padding = 0
         };
         var gesture = new TapGestureRecognizer();
         gesture.Tapped += Gesture_Tapped;
         view.GestureRecognizers.Add(gesture);
         urls.Add(view, url);

         return view;
      }

      private void Gesture_Tapped(object sender, EventArgs e) {
         if (urls.TryGetValue(sender, out string url)) {
            try {
               Browser.OpenAsync(url);
            } catch (Exception ex) {
               ex.Handle();
            }
         }
      }

      private View GetLabel(string text) {
         return new Label() {
            Text = text,
            LineBreakMode = LineBreakMode.WordWrap
         };
      }

      public string Text {
         get { return (string) GetValue(TextProperty); }
         set { SetValue(TextProperty, value); }
      }

      public LinkContainingTextView() {
         InitializeComponent();
      }
   }
}