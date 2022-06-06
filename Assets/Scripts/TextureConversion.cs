using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureConversion
{
    public static Texture2D ConvertTexture(RenderTexture tex)
    {
        Texture2D tex2D = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = tex;

        tex2D.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex2D.Apply();

        RenderTexture.active = currentActiveRT;
        return tex2D;
    }
}