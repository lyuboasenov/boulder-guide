using BoulderGuide.Wpf.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BoulderGuide.Wpf {
   public class AreaRouteDataTemplateSelector : DataTemplateSelector {
      public HierarchicalDataTemplate AreaTemplate { get; set; }
      public DataTemplate RouteTemplate { get; set; }

      public override DataTemplate SelectTemplate(object item, DependencyObject container) {
         if (item is Area) {
            return AreaTemplate;
         } else if (item is Route) {
            return RouteTemplate;
         } else {
            return new DataTemplate();
         }
      }
   }
}
