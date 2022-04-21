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
    [SerializeField] protected GameObject obj;
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

        for (int beliefIndex = 0; beliefIndex < beliefNumber; beliefIndex++) 
        {
            Belief currentBelief = Beliefs.GetBelief(beliefIndex);
            newColor += currentBelief.color * member.beliefScales[beliefIndex] / totalScale;
        }
        
        return newColor;
    }
    public Member[] GetBufferMembers()
    {
        return members;
    }
}
