using BoulderGuide.Mobile.Forms.Domain;
using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.XamarinForms {
   public class AreaInfoToImageConverter : IValueConverter {
      private static readonly Random random = new Random((int)DateTime.Now.Ticks);
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is AreaInfo info && (info.Images?.Any() ?? false)) {
            var index = random.Next(0, info.Images.Count());

            return ImageSource.FromStream(() => info.Images.ElementAt(index).GetStream());
         }

         return null;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
