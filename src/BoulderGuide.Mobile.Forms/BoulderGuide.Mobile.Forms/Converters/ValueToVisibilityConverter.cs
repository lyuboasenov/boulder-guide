using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.Converters {
   public class ValueToVisibilityConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is null ||
            value is string str && string.IsNullOrEmpty(str)) {
            return false;
         }

         return true;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
