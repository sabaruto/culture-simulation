using UnityEngine;

public abstract class BelieverManager : MonoBehaviour
{
    protected SpriteRenderer[] MemberRenderers;

    protected Member[] Members;

    public void Update()
    {
        UpdateColors();
    }

    public abstract void Create();

    public virtual void UpdateColors()
    {
        for (var memberIndex = 0; memberIndex < Members.Length; memberIndex++)
        {
            var currentMember = Members[memberIndex];
            var averagePlayerColor = GetAverageMemberColor(currentMember);
            MemberRenderers[memberIndex].color = averagePlayerColor;
        }
    }

    private Color GetAverageMemberColor(Member member)
    {
        var newColor = Color.black;
        var beliefNumber = Beliefs.Count;

        float totalScale = 0;
        for (var valueIndex = 0; valueIndex < beliefNumber; valueIndex++) totalScale += member.beliefScales[valueIndex];

        if (totalScale == 0) return Color.red;

        for (var beliefIndex = 0; beliefIndex < beliefNumber; beliefIndex++)
        {
            var currentBelief = Beliefs.GetBelief(beliefIndex);
            newColor += currentBelief.color * member.beliefScales[beliefIndex] / totalScale;
        }

        if (newColor[0] > 1 || newColor[1] > 1 || newColor[2] > 1) newColor = Color.white;

        if (newColor[0] < 0 || newColor[1] < 0 || newColor[2] < 0) newColor = Color.black;

        return newColor;
    }

    public Member[] GetBufferMembers()
    {
        return Members;
    }

    public struct Member
    {
        public Vector2Int position;
        public Vector2 beliefScales;
    }
}