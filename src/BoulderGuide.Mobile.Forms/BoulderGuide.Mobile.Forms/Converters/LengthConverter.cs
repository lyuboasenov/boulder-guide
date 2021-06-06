using System;
using System.Globalization;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.Converters {
   public class LengthConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is null) {
            return 0;
         } else {
            var pi = value.GetType().GetProperty("Length");
            return pi?.GetValue(value) ?? 0;
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
