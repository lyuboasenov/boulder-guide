using BoulderGuide.Wpf.Domain;
using BoulderGuide.Wpf.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BoulderGuide.Wpf {
   public class AreaRouteEditDataTemplateSelector : DataTemplateSelector {

      public override DataTemplate SelectTemplate(object item, DependencyObject container) {
         DataTemplate template ;
         FrameworkElementFactory gridFactory = new FrameworkElementFactory(typeof(Grid));

         if (item is Area area) {
            template = CreateDataTemplate(() => {
               var vm = new AreaViewModel(area);
               return vm.GetView();
            });
         } else if (item is Route route) {
            template = CreateDataTemplate(() => {
               var vm = new RouteViewModel(route);
               return vm.GetView();
            });
         } else {
            template = new DataTemplate();
         }

         return template;
      }

      public static DataTemplate CreateDataTemplate(Func<object> factory) {
         if (factory == null)
            throw new ArgumentNullException("factory");

         var frameworkElementFactory = new FrameworkElementFactory(typeof(_TemplateGeneratorControl));
         frameworkElementFactory.SetValue(_TemplateGeneratorControl.FactoryProperty, factory);

         var dataTemplate = new DataTemplate(typeof(DependencyObject));
         dataTemplate.VisualTree = frameworkElementFactory;
         return dataTemplate;
      }

      private sealed class _TemplateGeneratorControl : ContentControl {
         internal static readonly DependencyProperty FactoryProperty = DependencyProperty.Register("Factory", typeof(Func<object>), typeof(_TemplateGeneratorControl), new PropertyMetadata(null, _FactoryChanged));

         private static void _FactoryChanged(DependencyObject instance, DependencyPropertyChangedEventArgs args) {
            var control = (_TemplateGeneratorControl) instance;
            var factory = (Func<object>) args.NewValue;
            control.Content = factory?.Invoke();
         }
      }
   }
}
