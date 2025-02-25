using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ApiHandling.Runtime.Utilities
{
public static class SerializationUtilities
{
    public static Texture2D RenderTextureToTexture2D(this RenderTexture renderTexture)
    {
        return renderTexture.RenderTextureToTexture2D(TextureFormat.RGBA64);
    }
    public static Texture2D RenderTextureToTexture2D(this RenderTexture renderTexture, TextureFormat textureFormat)
    {
        // Set the active RenderTexture to the one you want to read from
        RenderTexture.active = renderTexture;

        // Create a new Texture2D with the same dimensions and format as the RenderTexture
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, textureFormat, false);

        // Read the pixels from the RenderTexture into the Texture2D
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Reset the active RenderTexture to null (to avoid any unintended effects)
        RenderTexture.active = null;

        return texture;
    }
    // public static async UniTask<List<T>> DeserializeArrayOfData<T>(this IApiJsonSerializer<T> apiTypeSerializer, string json, string arrayKey)
    // {
    //     var list = new List<T>();
    //     var jsonObject = JObject.Parse(json);
    //     var data = (JArray)jsonObject[arrayKey];
    //     if (data is null)
    //     {
    //         throw new Exception($"Couldn't find data array of key {arrayKey} in json: {json}");
    //     }
    //
    //     if (data.Count == 0)
    //     {
    //         return new List<T>();
    //     }
    //     foreach (var item in data)
    //     {
    //         var deserializedItem = await apiTypeSerializer.DeserializeApiJson(item.ToString(Formatting.Indented));
    //         if (deserializedItem is null) continue;
    //         list.Add(deserializedItem);
    //     }
    //     return list;
    // }
    public static byte[] SerializeToByteArr(object obj)
    {
        return obj switch
        {
            Texture2D texture2D => SerializeTexture(texture2D),
            string s => SerializeString(s),
            _ => SerializeJson(obj)
        };
    }

    private static byte[] SerializeTexture(Texture2D texture)
    {
        if (true || IsCompressedFormat(texture.format))
        {
            texture = ConvertToUncompressed(texture);
        }
        // Encode the uncompressed texture as PNG or JPG
        return texture.EncodeToPNG();
    }

    private static bool IsCompressedFormat(TextureFormat format)
    {
        // List of common compressed texture formats
        return format == TextureFormat.DXT1 || format == TextureFormat.DXT5 || 
               format == TextureFormat.ETC_RGB4 || format == TextureFormat.ETC2_RGB || 
               format == TextureFormat.ASTC_4x4;
    }

    private static Texture2D ConvertToUncompressed(Texture2D texture)
    {
        // Create a new texture with an uncompressed format
        Texture2D uncompressedTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA64, false);
        // Set pixels from the original texture to the uncompressed texture
        // var pixels = texture.GetPixels();
        Color[] pixels = texture.GetPixels();
        for (var i = 0; i < pixels.Length; i++)
        {
            Color px = pixels[i];
            if (px.r == 0.0f && px.g == 0.0f && px.b == 0.0f)
            {
                px.a = 0.0f;
                pixels[i] = px;
            }
        }
        uncompressedTexture.SetPixels(pixels);
        uncompressedTexture.Apply();
        return uncompressedTexture;
    }

    private static byte[] SerializeString(string str)
    {
        return System.Text.Encoding.UTF8.GetBytes(str);
    }

    private static byte[] SerializeJson<T>(T obj)
    {
        string json = JsonConvert.SerializeObject(obj);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }
}
}