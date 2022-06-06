using UnityEngine;

[System.Serializable]
public struct Belief
{
    public Color color;
}

public static class Beliefs
{
    private static Belief[] beliefs;

    public static void Initialize(Color[] colors)
    {
        beliefs = new Belief[colors.Length];

        for (int index = 0; index < colors.Length; index++)
        {
            beliefs[index] = new Belief { color = colors[index] };
        }
    }

    public static Belief GetBelief(int index)
    {
        return beliefs[index];
    }

    public static Belief[] AllBeliefs => beliefs;

    public static int Count => beliefs.Length;
}