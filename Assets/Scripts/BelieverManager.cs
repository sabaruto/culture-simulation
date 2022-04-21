using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BelieverManager : MonoBehaviour
{
    public struct Member
    {
        public Vector2 position;
        public Dictionary<Belief, float> beliefScales; 
    }

    public struct BufferMember
    {
        public Vector2 position;
        public Vector2 beliefScales;
    }
    [SerializeField] protected GameObject obj;
    protected Member[] members;
    protected BufferMember[] bufferMembers;
    protected GameObject[] memberObjects;
    protected SpriteRenderer[] memberRenderers;
    protected void Initialize(int memberNumber)
    {
        members = new Member[memberNumber];
        memberObjects = new GameObject[memberNumber];
        memberRenderers = new SpriteRenderer[memberNumber];
        bufferMembers = new BufferMember[memberNumber];
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
        int beliefNumber = member.beliefScales.Count;
        
        float totalScale = 0;
        foreach (float value in member.beliefScales.Values) { totalScale += value; }

        foreach (Belief belief in Beliefs.AllBeliefs)
        {
            newColor += belief.color * member.beliefScales[belief] / totalScale;
        }
        
        return newColor;
    }
    public BufferMember[] GetBufferMembers()
    {
        return bufferMembers;
    }
    public BufferMember ConvertMember(Member member)
    {
        Vector2 beliefScales = new Vector2();
        for (int beliefIndex = 0; beliefIndex < Beliefs.Count; beliefIndex++)
        {
            Belief currentBelief = Beliefs.GetBelief(beliefIndex);
            beliefScales[beliefIndex] = member.beliefScales[currentBelief];
        }

        return new BufferMember
        {
            position = member.position, 
            beliefScales = beliefScales
        };
    }

    // Updates the members from the bufferMember values
    protected void UpdateMembers()
    {
        for (int memberIndex = 0; memberIndex < members.Length; memberIndex++)
        {
            Member currentMember = members[memberIndex];
            BufferMember currentBufferMember = bufferMembers[memberIndex];
            currentMember.position = currentBufferMember.position;

            for (int beliefIndex = 0; beliefIndex < Beliefs.Count; beliefIndex++)
            {
                Belief belief = Beliefs.GetBelief(beliefIndex);
                currentMember.beliefScales[belief] = currentBufferMember.beliefScales[beliefIndex];
            }

            members[memberIndex] = currentMember;
        }
    }
}
