using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms {
   public static class Breadcrumbs {
      public static ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>(new[] {
         new Item() {
            Name = Strings.Home,
            Path = "/MainPage"
         }
         });

      public class Item {
         public string Name { get; set; }
         public string Path { get; set; }
         public INavigationParameters Parameters { get; set; }

         public override string ToString() {
            return new string(' ', Items.IndexOf(this) * 3) + Name;
         }
      }


   }
}
