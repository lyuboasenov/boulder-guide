using BoulderGuide.Mobile.Forms.Services.Data;
using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.XamarinForms {
   public class AreaInfoToImageConverter : IValueConverter {
      private static readonly Random random = new Random((int)DateTime.Now.Ticks);
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is AreaInfo info && (info.Images?.Any() ?? false)) {
            var path = info.GetImageLocalPath(info.Images[random.Next(0, info.Images.Length)]);
            return ImageSource.FromFile(path);
         }

         return null;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
