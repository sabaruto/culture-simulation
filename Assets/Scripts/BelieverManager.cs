using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BelieverManager : MonoBehaviour
{
    /// <summary>
    /// A member of the beliefs
    /// </summary>
    public struct Member
    {
        public Vector2Int position;
        public int beliefPosition;
        public int beliefLength;
    }
    protected Member[] members;
    protected List<float> beliefScales;
    public abstract void Create();
    public float[] BeliefScales => beliefScales.ToArray();
    protected virtual void Awake()
    {
        beliefScales = new List<float>();
    }
    public void Update()
    {
        UpdateColors();
    }
    public abstract void UpdateColors();
    protected Color GetAverageMemberColor(Member member)
    {
        Color newColor = Color.black;
        int beliefNumber = Beliefs.Count;

        float totalScale = 0;
        for (int valueIndex = 0; valueIndex < beliefNumber; valueIndex++)
        {
            totalScale += GetBeliefScale(member, valueIndex);
        }

        if (totalScale == 0)
        {
            return Color.red;
        }

        for (int beliefIndex = 0; beliefIndex < beliefNumber; beliefIndex++)
        {
            Belief currentBelief = Beliefs.GetBelief(beliefIndex);
            newColor += currentBelief.color * GetBeliefScale(member, beliefIndex) / totalScale;
        }

        if (newColor[0] > 1 || newColor[1] > 1 || newColor[2] > 1)
        {
            newColor = Color.white;
        }

        if (newColor[0] < 0 || newColor[1] < 0 || newColor[2] < 0)
        {
            newColor = Color.black;
        }

        return newColor;
    }
    public Member[] GetBufferMembers()
    {
        return members;
    }

    public Member CreateMember(Vector2Int position, float[] beliefs)
    {
        // Find the end of the belief array
        int beliefPosition = beliefScales.Count;
        int beliefLength = beliefs.Length;

        // Add member belief to array
        beliefScales.AddRange(beliefs);
        Member newMember = new Member
        {
            position = position,
            beliefPosition = beliefPosition,
            beliefLength = beliefLength
        };
        return newMember;
    }

    public float GetBeliefScale(Member member, int index)
    {
        int memberBeliefScaleIndex = index + member.beliefPosition;
        return beliefScales[memberBeliefScaleIndex];
    }

    public void UpdateBeliefScales(float[] newBeliefScales)
    {
        beliefScales = new List<float>(newBeliefScales);
    }
}
