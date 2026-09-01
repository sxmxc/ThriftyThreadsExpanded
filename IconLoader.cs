using System.Reflection;
using ThriftyThreadsExpanded.Utils;
using UnityEngine;

#if IL2CPP
using Il2CppInterop.Runtime.InteropTypes.Arrays;
#endif

namespace ThriftyThreadsExpanded;

internal static class IconLoader
{
    public static Sprite? Load(string itemId)
    {
        var resourceName = $"{Constants.IconResourcePrefix}{itemId}.png";
        using var stream = typeof(Core).Assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            return null;
        }

        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        var texture = new Texture2D(2, 2);
#if IL2CPP
        ImageConversion.LoadImage(texture, (Il2CppStructArray<byte>)buffer.ToArray());
#else
        ImageConversion.LoadImage(texture, buffer.ToArray());
#endif
        texture.name = $"{itemId}_Icon";
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
