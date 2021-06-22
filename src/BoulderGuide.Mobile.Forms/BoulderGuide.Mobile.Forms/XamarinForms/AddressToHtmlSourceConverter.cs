using System;
using System.Globalization;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.XamarinForms {
   public class AddressToHtmlSourceConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is string address &&
            !string.IsNullOrEmpty(address)) {

            string html;
            if (address.IndexOf("instagram", StringComparison.InvariantCultureIgnoreCase) >= 0) {
               html = $@"
<html>
   <body>
      {address}
   </body>
</html>";
            } else {
               html = $@"
<html>
   <body>
      <iframe src=""{address}"" frameborder=""0"" allow=""accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; fullscreen"" style=""width:100%;height:100%;"" allowfullscreen ></iframe>
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
