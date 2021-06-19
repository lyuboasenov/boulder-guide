using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Services.Data.Entities;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.XamarinForms {
   public class AreaRouteDataTemplateSelector : DataTemplateSelector {
      public DataTemplate AreaTemplate { get; set; }
      public DataTemplate RouteTemplate { get; set; }

      protected override DataTemplate OnSelectTemplate(object item, BindableObject container) {
         if (item is AreaInfoDTO) {
            return AreaTemplate;
         } else if (item is RouteInfo) {
            return RouteTemplate;
         }

         return null;
      }
   }
}
