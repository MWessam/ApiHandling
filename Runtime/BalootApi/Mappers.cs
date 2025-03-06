using System;
using Mapster;
using Newtonsoft.Json;

namespace BalootApi
{
    public static class MapperLayer
    {
        public static void InitializeMappers()
        {
            TypeAdapterConfig<InventoryItemDto, ItemDto>.NewConfig()
                .Map(x => x, x => x.Item);
            
            TypeAdapterConfig<SelectedItemsDto, ItemDto>.NewConfig()
                .Map(x => x, x => x.Item);

            TypeAdapterConfig<NotificationDto, BaseNotification>.NewConfig()
                .Map(x => x.Type, x => ParseNotificationType(x.Type));
        }

        private static ENotificationType ParseNotificationType(string type)
        {
            return type switch
            {
                "system" => ENotificationType.System,
                "message" => ENotificationType.Message,
                "friend_request" => ENotificationType.FriendRequest,
                "friend_request_accepted" => ENotificationType.FriendAccepted,
                "follow" => ENotificationType.Follow,
                _ => ENotificationType.None
            };
        }
    }
    public class StringToIntConverter : JsonConverter<int>
    {
        public override int ReadJson(JsonReader reader, Type objectType, int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Handle null values if necessary
            if (reader.TokenType == JsonToken.Null)
            {
                return 0;
            }
        
            // When the token is a string, try to parse it as a decimal first
            if (reader.TokenType == JsonToken.String)
            {
                var str = reader.Value.ToString();
                if (decimal.TryParse(str, out var dec))
                {
                    // You might decide how to round or convert the decimal to int
                    return (int)dec;
                }
                throw new JsonSerializationException($"Unable to parse '{str}' as a number.");
            }
        
            // If it's already an integer, just return it
            if (reader.TokenType == JsonToken.Integer)
            {
                return Convert.ToInt32(reader.Value);
            }
        
            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing integer.");
        }

        public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer)
        {
            // Write the integer value (or format as string if needed)
            writer.WriteValue(value);
        }
    }

}