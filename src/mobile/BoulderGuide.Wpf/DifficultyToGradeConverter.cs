using BoulderGuide.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace BoulderGuide.Wpf {
   public class DifficultyToGradeConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is double diff) {
            return new Grade(diff).ToString();
         } else if (value is int intDiff) {
            return new Grade(intDiff).ToString();
         }

         return value;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
