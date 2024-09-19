using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Users.Model
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Role
    {
        PATIENT,
        CARE_GIVER,
        ADMIN
    }
}
