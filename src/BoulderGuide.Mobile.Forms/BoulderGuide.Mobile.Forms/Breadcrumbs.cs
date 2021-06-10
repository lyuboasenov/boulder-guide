using Prism.Navigation;
using System.Collections.ObjectModel;

namespace BoulderGuide.Mobile.Forms {
   public static class Breadcrumbs {
      public static ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>(new[] {
         new Item() {
            Glyph = Icons.MaterialIconFont.Home,
            Title = Strings.Home,
            Path = "/MainPage/NavigationPage/HomePage"
         }});

      public class Item {
         public string Offset {
            get {
               return new string(' ', Items.IndexOf(this) * 3);
            }
         }
         public string Glyph { get; set; }
         public string Title { get; set; }
         public string Path { get; set; }
         public INavigationParameters Parameters { get; set; }

         public override string ToString() {
            return $"{Title}: {Path}";
         }
      }
   }
}
