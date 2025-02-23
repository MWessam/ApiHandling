// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
//
// internal static class PropertyFieldReflectionCache<T>
// {
//     // Cache only the fields marked with FieldMapAttribute
//     public static readonly FieldInfo[] FieldInfos =
//         typeof(T)
//             .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
//             .Where(f => f.IsDefined(typeof(FieldMapAttribute), inherit: true))
//             .ToArray();
//
//     // Cache only the properties marked with FieldMapAttribute
//     public static readonly PropertyInfo[] PropertyInfos =
//         typeof(T)
//             .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
//             .Where(p => p.IsDefined(typeof(FieldMapAttribute), inherit: true))
//             .ToArray();
// }
// internal static class EntityMapReflectionCache<TEntity, TDto>
// {
//     
//     // The dictionary caches for each effective member name a pair of MemberInfo (one from each type)
//     public static readonly IReadOnlyDictionary<string, (FieldInfo Entity, FieldInfo Dto)> CommonFields = GetCommonFields();
//     public static readonly IReadOnlyDictionary<string, (PropertyInfo Entity, PropertyInfo Dto)> CommonProperties = GetCommonProperties();
//
//     private static IReadOnlyDictionary<string, (FieldInfo Source, FieldInfo Destination)> GetCommonFields()
//     {
//         var sourceMembers = GetEffectiveFields(typeof(TEntity));
//         var destMembers = GetEffectiveFields(typeof(TDto));
//
//         // Find intersection based on effective names
//         var common = sourceMembers.Keys.Intersect(destMembers.Keys)
//             .ToDictionary(name => name, name => (Source: sourceMembers[name], Destination: destMembers[name]));
//
//         return common;
//     }
//     private static IReadOnlyDictionary<string, (PropertyInfo Source, PropertyInfo Destination)> GetCommonProperties()
//     {
//         var sourceMembers = GetEffectiveProperties(typeof(TEntity));
//         var destMembers = GetEffectiveProperties(typeof(TDto));
//
//         // Find intersection based on effective names
//         var common = sourceMembers.Keys.Intersect(destMembers.Keys)
//             .ToDictionary(name => name, name => (Source: sourceMembers[name], Destination: destMembers[name]));
//
//         return common;
//     }
//
//     // This helper collects all public and non-public fields and properties of a type,
//     // and maps them to an effective name: if a member is decorated with FieldMapAttribute
//     // and a non-empty TargetName is provided, that is used; otherwise the member's name is used.
//     private static Dictionary<string, FieldInfo> GetEffectiveFields(Type type)
//     {
//         var dict = new Dictionary<string, FieldInfo>();
//
//         // We can consider both fields and properties
//         var members = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//
//         foreach (var member in members)
//         {
//             // Look for the FieldMap attribute
//             var attr = member.GetCustomAttribute<FieldMapAttribute>();
//             string effectiveName = !string.IsNullOrWhiteSpace(attr?.DtoFieldName)
//                 ? attr.DtoFieldName!
//                 : member.Name;
//             // Optionally: if there are duplicates, decide on a policy (here we take the first one)
//             dict.TryAdd(effectiveName, member);
//         }
//         return dict;
//     }
//     private static Dictionary<string, PropertyInfo> GetEffectiveProperties(Type type)
//     {
//         var dict = new Dictionary<string, PropertyInfo>();
//
//         // We can consider both fields and properties
//         var members = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//
//         foreach (var member in members)
//         {
//             // Look for the FieldMap attribute
//             var attr = member.GetCustomAttribute<FieldMapAttribute>();
//             string effectiveName = !string.IsNullOrWhiteSpace(attr?.DtoFieldName)
//                 ? attr.DtoFieldName!
//                 : member.Name;
//             // Optionally: if there are duplicates, decide on a policy (here we take the first one)
//             dict.TryAdd(effectiveName, member);
//         }
//
//         return dict;
//     }
// }
// /// <summary>
// /// A static registry that holds global mappers for converting from one type to another.
// /// </summary>
// public static class EntityMapperRegistry
// {
//     // Each mapper is a delegate that takes an object and returns an object.
//     private static readonly Dictionary<(Type, Type), Func<object, object>> _registry = new();
//
//     public static void RegisterMapper<TSource, TDestination>(Func<TSource, TDestination> mapper)
//     {
//         _registry[(typeof(TSource), typeof(TDestination))] = (obj) => mapper((TSource)obj);
//     }
//
//     public static bool TryGetMapper(Type sourceType, Type destinationType, out Func<object, object> mapper)
//     {
//         return _registry.TryGetValue((sourceType, destinationType), out mapper);
//     }
// }
//
// public interface IEntityMapper<TEntity, TDto> 
//     where TDto: struct
// {
//     TDto GetDto(TEntity entity);
//     TEntity GetEntity(TDto dto);
// }
//
//
// public sealed class AutoMapper<TEntity, TDto> : IEntityMapper<TEntity, TDto> 
//     where TDto: struct
// {
//     /// <summary>
//     /// Stores the name of the field/property and uses the field mapper to map it.
//     /// </summary>
//     private readonly Dictionary<string, IFieldMapper> _fieldMappers = new();
//     /// <summary>
//     /// Register a custom field mapper.
//     /// </summary>
//     public void RegisterCustomFieldMapper(string fieldName, IFieldMapper mapper)
//     {
//         _fieldMappers[fieldName] = mapper;
//     }
//     public TDto GetDto(TEntity entity)
//     {
//         TDto dto = default;
//         // Create a new instance of TEntity. It must have a parameterless constructor.
//         try
//         {
//             dto = Activator.CreateInstance<TDto>();
//         }
//         catch (Exception e)
//         {
//             throw new Exception($"No constructor found for entity of type {nameof(TEntity)}");
//         }
//         foreach (var commonField in EntityMapReflectionCache<TEntity, TDto>.CommonFields)
//         {
//             FieldInfo entityField = commonField.Value.Entity;
//             FieldInfo dtoField = commonField.Value.Dto;
//
//             // Get the value from the entity
//             object sourceValue = entityField.GetValue(entity);
//
//             // Map value to the DTO field type
//             object mappedValue = MapEntityToDto(commonField.Key, sourceValue, entityField.FieldType, dtoField.FieldType);
//
//             // Set the value on the DTO.
//             // For structs, we use SetValueDirect if available; otherwise, boxing can be used.
//             dtoField.SetValueDirect(__makeref(dto), mappedValue);
//         }
//         foreach (var commonProperty in EntityMapReflectionCache<TEntity, TDto>.CommonProperties)
//         {
//             PropertyInfo entityProp = commonProperty.Value.Entity;
//             PropertyInfo dtoProp = commonProperty.Value.Dto;
//
//             // Only proceed if the DTO property can be written to.
//             if (!dtoProp.CanWrite) continue;
//
//             object sourceValue = entityProp.GetValue(entity);
//             object mappedValue = MapEntityToDto(commonProperty.Key, sourceValue, entityProp.PropertyType, dtoProp.PropertyType);
//             dtoProp.SetValue(dto, mappedValue);
//         }
//
//         return dto;
//     }
//
//     public TEntity GetEntity(TDto dto)
//     {
//         TEntity entity = default;
//         // Create a new instance of TEntity. It must have a parameterless constructor.
//         try
//         {
//             entity = Activator.CreateInstance<TEntity>();
//         }
//         catch (Exception e)
//         {
//             throw new Exception($"No constructor found for entity of type {nameof(TEntity)}");
//         }
//         
//         foreach (var commonField in EntityMapReflectionCache<TEntity, TDto>.CommonFields)
//         {
//             FieldInfo entityField = commonField.Value.Entity;
//             FieldInfo dtoField = commonField.Value.Dto;
//
//             object sourceValue = dtoField.GetValue(dto);
//             object mappedValue = MapDtoToEntity(commonField.Key, sourceValue, dtoField.FieldType, entityField.FieldType);
//             entityField.SetValue(entity, mappedValue);
//         }
//
//         // Map properties
//         foreach (var commonProp in EntityMapReflectionCache<TEntity, TDto>.CommonProperties)
//         {
//             PropertyInfo entityProp = commonProp.Value.Entity;
//             PropertyInfo dtoProp = commonProp.Value.Dto;
//
//             // Only proceed if the entity property can be written to.
//             if (!entityProp.CanWrite) continue;
//
//             object sourceValue = dtoProp.GetValue(dto);
//             object mappedValue = MapDtoToEntity(commonProp.Key, sourceValue, dtoProp.PropertyType, entityProp.PropertyType);
//             entityProp.SetValue(entity, mappedValue);
//         }
//
//         return entity;
//     }
//
//     private object MapDtoToEntity(string fieldName, object sourceValue, Type sourceType, Type destinationType)
//     {
//         return MapValueHelper(sourceValue, sourceType, destinationType, () =>
//         {
//             // First try custom field mapper.
//             if (_fieldMappers.TryGetValue(fieldName, out var mapper))
//                 return mapper.MapToEntity(sourceValue);
//
//             // Then try global IEntityMapper from the registry.
//             if (EntityMapperRegistry.TryGetMapper(sourceType, destinationType, out var globalMapper))
//                 return globalMapper(sourceValue);
//
//             throw new NoFieldMappersException($"No mapping available for converting {sourceType.Name} to {destinationType.Name} for field '{fieldName}'");
//         });
//     }
//     private object MapEntityToDto(string fieldName, object sourceValue, Type sourceType, Type destinationType)
//     {
//         return MapValueHelper(sourceValue, sourceType, destinationType, () =>
//         {
//             if (_fieldMappers.TryGetValue(fieldName, out var mapper))
//                 return mapper.MapToDto(sourceValue);
//
//             if (EntityMapperRegistry.TryGetMapper(sourceType, destinationType, out var globalMapper))
//                 return globalMapper(sourceValue);
//
//             throw new NoFieldMappersException($"No mapping available for converting {sourceType.Name} to {destinationType.Name} for field '{fieldName}'");
//         });
//     }
//
//     private object MapValueHelper(object sourceValue, Type sourceType, Type destinationType, Func<object> onCustomMap)
//     {
//         // Handle nulls
//         if (sourceValue == null)
//         {
//             return destinationType.IsValueType ? Activator.CreateInstance(destinationType) : null;
//         }
//
//         // If the destination type is assignable from the source type, use it directly.
//         if (destinationType.IsAssignableFrom(sourceType))
//         {
//             return sourceValue;
//         }
//         // Check if we are dealing with collections (but ignore string).
//         if (sourceType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(sourceType) &&
//             destinationType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(destinationType))
//         {
//             return MapCollection(sourceValue, sourceType, destinationType);
//         }
//
//         // Try a primitive conversion if possible.
//         try
//         {
//             // Note: Convert.ChangeType works for many primitives.
//             return Convert.ChangeType(sourceValue, destinationType);
//         }
//         catch
//         {
//             return onCustomMap?.Invoke();
//         }
//     }
//     /// <summary>
//     /// Maps a source collection to a destination collection by converting each element.
//     /// </summary>
//     private object MapCollection(object sourceValue, Type sourceType, Type destinationType)
//     {
//         // Determine the element types for source and destination.
//         Type sourceElementType = GetElementType(sourceType);
//         Type destinationElementType = GetElementType(destinationType);
//
//         if (sourceElementType == null || destinationElementType == null)
//             throw new InvalidOperationException("Unable to determine collection element types.");
//
//         // Get the source collection as IEnumerable.
//         var sourceEnumerable = sourceValue as IEnumerable;
//         if (sourceEnumerable == null)
//             return null;
//
//         // Create a destination list of the destination element type.
//         var listType = typeof(List<>).MakeGenericType(destinationElementType);
//         var destList = (IList)Activator.CreateInstance(listType);
//
//         foreach (var item in sourceEnumerable)
//         {
//             // Recursively map each element.
//             object mappedItem = MapValueHelper(item, sourceElementType, destinationElementType, () =>
//             {
//                 if (EntityMapperRegistry.TryGetMapper(sourceElementType, destinationElementType, out var globalMapper))
//                     return globalMapper(item);
//                 throw new NoFieldMappersException($"No mapping available for converting collection element from {sourceElementType.Name} to {destinationElementType.Name}");
//             });
//             destList.Add(mappedItem);
//         }
//
//         // If the destination type is an array, convert the list to an array.
//         if (destinationType.IsArray)
//         {
//             var toArrayMethod = listType.GetMethod("ToArray");
//             return toArrayMethod?.Invoke(destList, null);
//         }
//         // Otherwise, if the destination type is assignable from List<T>, return the list.
//         if (destinationType.IsAssignableFrom(listType))
//             return destList;
//
//         // As a last resort, try to create an instance of the destination type from the list.
//         try
//         {
//             return Activator.CreateInstance(destinationType, destList);
//         }
//         catch (Exception ex)
//         {
//             throw new InvalidOperationException($"Cannot convert collection to type {destinationType.Name}", ex);
//         }
//     }
//     /// <summary>
//     /// Attempts to determine the element type of a collection.
//     /// </summary>
//     private Type GetElementType(Type type)
//     {
//         if (type.IsArray)
//             return type.GetElementType();
//         if (type.IsGenericType)
//             return type.GetGenericArguments().FirstOrDefault();
//         return typeof(object);
//     }
// }
//
// internal class NoFieldMappersException : Exception
// {
//     public NoFieldMappersException(string message) : base(message)
//     {
//     }
// }
//
// public interface IFieldMapper
// {
//     object MapToEntity(object dto);
//     object MapToDto(object dto);
// }