using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BelieverManager : MonoBehaviour
{
    public struct Member
    {
        public Vector2 position;
        public Vector2 beliefScales;
    }
    protected Member[] members;
    protected GameObject[] memberObjects;
    protected SpriteRenderer[] memberRenderers;
    protected void Initialize(int memberNumber)
    {
        memberObjects = new GameObject[memberNumber];
        memberRenderers = new SpriteRenderer[memberNumber];
        members = new Member[memberNumber];
    }
    
    public abstract void Create();
    public void Update()
    {
        UpdateColors();
    }
    public void UpdateColors()
    {
        for (int memberIndex = 0; memberIndex < members.Length; memberIndex++)
        {
            Member currentMember = members[memberIndex];
            Color averagePlayerColor = GetAverageMemberColor(currentMember);
            memberRenderers[memberIndex].color = averagePlayerColor;
        }
    }
    private Color GetAverageMemberColor(Member member)
    {
        Color newColor = Color.black;
        int beliefNumber = Beliefs.Count;
        
        float totalScale = 0;
        for (int valueIndex = 0; valueIndex < beliefNumber; valueIndex++) 
        { 
            totalScale += member.beliefScales[valueIndex]; 
        }

        if (totalScale == 0)
        {
            return Color.red;
        }

        for (int beliefIndex = 0; beliefIndex < beliefNumber; beliefIndex++) 
        {
            Belief currentBelief = Beliefs.GetBelief(beliefIndex);
            newColor += currentBelief.color * member.beliefScales[beliefIndex] / totalScale;
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
}
