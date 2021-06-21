using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BoulderGuide.Mobile.Forms.Domain.DTOs {
   [JsonConverter(typeof(StringEnumConverter))]
   public enum RegionAccess {
      @public,
      @private
   }
}