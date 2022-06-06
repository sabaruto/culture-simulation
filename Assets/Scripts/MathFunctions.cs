using UnityEngine;

public static class MathFunctions
{
    /// <summary>
    ///     Rounds the value into a specific resolution.
    /// </summary>
    /// <param name="x">The analogue value</param>
    /// <param name="scale">The maximum resolution</param>
    /// <returns></returns>
    public static float Snap(float x, float scale)
    {
        var bloatedX = x / scale;
        return Mathf.Round(bloatedX) * scale;
    }
}