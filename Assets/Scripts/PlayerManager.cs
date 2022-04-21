using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : BelieverManager
{
    [SerializeField] private int numberOfPlayers;
    [SerializeField] private int personSize;
    private BackgroundManager bgManager;

    public void Awake()
    {
        bgManager = FindObjectOfType<BackgroundManager>();
        Initialize(numberOfPlayers);
    }

    public override void Create()
    {
        int pixelWidth = bgManager.PixelWidth;
        int pixelHeight = bgManager.PixelHeight;
        float scale = bgManager.Scale;
        int personRadius = personSize / 2;

        Vector2 minPosition = bgManager.PixelToPosition(personRadius - 1, personRadius - 1);
        Vector2 maxPosition = bgManager.PixelToPosition(pixelWidth - personRadius, pixelHeight - personRadius);

        for (int playerIndex = 0; playerIndex < numberOfPlayers; playerIndex++)
        {
            Vector2 position = new Vector2(
                MathFunctions.Snap(Random.Range(minPosition.x, maxPosition.x), scale),
                MathFunctions.Snap(Random.Range(minPosition.y, maxPosition.y), scale)
            );

            Vector2 currentBeliefs = new Vector2(Random.value, Random.value);

            GameObject currentObject = Instantiate(
                obj,
                position,
                Quaternion.identity
            );
            Member currentPlayer = new Member{position = position, beliefScales = currentBeliefs};

            memberObjects[playerIndex] = currentObject;
            memberRenderers[playerIndex] = currentObject.GetComponent<SpriteRenderer>();
            members[playerIndex] = currentPlayer;
        }
    }
}
