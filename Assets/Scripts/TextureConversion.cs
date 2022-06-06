using UnityEngine;

public static class TextureConversion
{
    public static Texture2D ConvertTexture(RenderTexture tex)
    {
        var tex2D = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
        var currentActiveRT = RenderTexture.active;
        RenderTexture.active = tex;

        tex2D.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex2D.Apply();

        RenderTexture.active = currentActiveRT;
        return tex2D;
    }
}