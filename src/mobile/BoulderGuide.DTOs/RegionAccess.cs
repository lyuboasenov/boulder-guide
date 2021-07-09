using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BoulderGuide.DTOs {
   [JsonConverter(typeof(StringEnumConverter))]
   public enum RegionAccess {
      @public,
      @private
   }
}