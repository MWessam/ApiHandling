using System;
using System.Collections.Generic;
using System.Linq;
using Mapster;
using Newtonsoft.Json;

namespace BalootApi
{
    public static class MapperLayer
    {
        public static void InitializeMappers()
        {
            TypeAdapterConfig<InventoryItemDto, Item>.NewConfig()
                .Map(dest => dest.Id, src => src.ItemId)
                .Map(dest => dest.Price, src => src.Price)
                .Map(dest => dest.Quantity, src => src.Quantity);

            TypeAdapterConfig<SelectedItemsDto, ItemDto>.NewConfig()
                .Map(x => x, x => x.Item);

            TypeAdapterConfig<RelationDto, UserDto>.NewConfig()
                .Map(dest => dest, src => src.User)
                .Ignore(dest => dest.IsFriend)
                .AfterMapping((src, dest) =>
                {
                    if (src.Accepted && src.RelationType == "friend")
                    {
                        dest.IsFriend = true;
                    }
                });


            TypeAdapterConfig<NotificationDto, BaseNotification>.NewConfig()
                .Ignore((dest) => dest.Type)
                .Ignore((dest) => dest.Post)
                .AfterMapping((src, dest) =>
                {
                    dest.Type = ParseNotificationType(src.Type);
                    dest.Post = new Post()
                    {
                        Id = src.PostId
                    };
                });

            TypeAdapterConfig<ColorCustomizationItem, UpdateCustomizationItemDto>.NewConfig()
                .Map(dest => dest.Type, src => src.CustomizationType.ToString())
                .Map(dest => dest.ItemId, src => int.Parse(src.Index));

            TypeAdapterConfig<CustomizationItemDto, ColorCustomizationItem>.NewConfig()
                .Map(dest => dest.CustomizationType, src => Enum.Parse<ECustomization>(src.Type))
                .AfterMapping((src, dest) => dest.Index = src.Item.Id.ToString());

            TypeAdapterConfig<CharacterAvatarData, List<ColorCustomizationItem>>.NewConfig()
                .Map(dest => dest, src => src.CustomizationItemsDictionary.Select(x => x.Value).ToList());
            TypeAdapterConfig<ColorCustomizationItem, CustomizationItemDto>.NewConfig()
                .Map(dest => dest.Type, src => src.CustomizationType.ToString())
                .Map(dest => dest.Item, src => new ItemDto()
                {
                    Id = int.Parse(src.Index)
                });

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
                "post_like" => ENotificationType.PostLike,
                "post_comment" => ENotificationType.Comment,
                "comment_like" => ENotificationType.CommentLike,

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
