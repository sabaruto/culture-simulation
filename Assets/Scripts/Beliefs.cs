using System;
using UnityEngine;

[Serializable]
public struct Belief
{
    public Color color;
}

public static class Beliefs
{
    public static Belief[] AllBeliefs { get; private set; }

    public static int Count => AllBeliefs.Length;

    public static void Initialize(Color[] colors)
    {
        AllBeliefs = new Belief[colors.Length];

        for (var index = 0; index < colors.Length; index++) AllBeliefs[index] = new Belief { color = colors[index] };
    }

    public static Belief GetBelief(int index)
    {
        return AllBeliefs[index];
    }
}