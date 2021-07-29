using JsonSubTypes;
using Newtonsoft.Json;

namespace BoulderGuide.DTOs {
   public abstract class Shape {
      public string _type => GetType().Name;

      public static JsonConverter StandardJsonConverter {
         get {
            return JsonSubtypesConverterBuilder.Of(typeof(Shape), "_type").Build();
         }
      }

      public override string ToString() {
         return _type;
      }
   }
}
