using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : BelieverManager
{
    [SerializeField] protected GameObject obj;
    [SerializeField] private int numberOfPlayers;
    [SerializeField] private int personSize;
    private BackgroundManager bgManager;
    private GameManager gameManager;
    private GameObject[] memberObjects;
    private SpriteRenderer[] memberRenderers;

    protected override void Awake()
    {
        base.Awake();

        bgManager = FindObjectOfType<BackgroundManager>();
        gameManager = FindObjectOfType<GameManager>();

        memberObjects = new GameObject[numberOfPlayers];
        memberRenderers = new SpriteRenderer[numberOfPlayers];
        members = new Member[numberOfPlayers];
    }

    public override void Create()
    {
        int pixelWidth = bgManager.PixelWidth;
        int pixelHeight = bgManager.PixelHeight;
        int personRadius = personSize / 2;

        Vector2Int minPosition = new Vector2Int(personRadius - 1, personRadius - 1);
        Vector2Int maxPosition = new Vector2Int(pixelWidth - personRadius, pixelHeight - personRadius);

        for (int playerIndex = 0; playerIndex < numberOfPlayers; playerIndex++)
        {
            Vector2Int position = new Vector2Int(
                Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y, maxPosition.y)
            );

            float[] currentBeliefs = new float[gameManager.NumberOfBeliefs];
            for (int beliefIndex = 0; beliefIndex < currentBeliefs.Length; beliefIndex++)
            {
                currentBeliefs[beliefIndex] = Random.value;
            }

            GameObject currentObject = Instantiate(
                obj,
                bgManager.PixelToPosition(position[0], position[1]),
                Quaternion.identity
            );
            Member currentPlayer = CreateMember(position, currentBeliefs);

            memberObjects[playerIndex] = currentObject;
            memberRenderers[playerIndex] = currentObject.GetComponent<SpriteRenderer>();
            members[playerIndex] = currentPlayer;
        }
    }

    public override void UpdateColors()
    {
        for (int memberIndex = 0; memberIndex < members.Length; memberIndex++)
        {
            Member currentMember = members[memberIndex];
            Color averagePlayerColor = GetAverageMemberColor(currentMember);
            memberRenderers[memberIndex].color = averagePlayerColor;
        }
    }
}
