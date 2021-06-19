using BoulderGuide.Mobile.Forms.Services.Data.Entities;
using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.XamarinForms {
   public class AreasRoutesCountConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is AreaInfo info) {
            return $"{info.Areas?.Count() ?? 0} / {info.Routes?.Count() ?? 0}";
         } else {
            return value;
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
