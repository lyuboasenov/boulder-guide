using BoulderGuide.DTOs;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.XamarinForms {
   public class VideoToHtmlSourceConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is Video video &&
            !string.IsNullOrEmpty(video.EmbedCode)) {

            string html;
            if (Uri.TryCreate(video.EmbedCode, UriKind.Absolute, out Uri _)) {
               html = $@"
<html>
   <body>
      <iframe src=""{video.EmbedCode}"" frameborder=""0"" allow=""accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; fullscreen"" style=""width:100%;height:100%;"" allowfullscreen ></iframe>
   </body>
</html>";
            } else {
               html = $@"
<html>
   <body>
      {video.EmbedCode}
   </body>
</html>";
            }
            return new HtmlWebViewSource() {
               Html = html
            };
         }

         return value;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
