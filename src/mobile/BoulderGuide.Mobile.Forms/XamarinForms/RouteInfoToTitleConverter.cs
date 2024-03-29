﻿using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Domain;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.XamarinForms {
   public class RouteInfoToTitleConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
         if (value is RouteInfo info) {
            return $"{info.Name} ({new Grade(info.Difficulty)})";
         } else {
            return value;
         }
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}
