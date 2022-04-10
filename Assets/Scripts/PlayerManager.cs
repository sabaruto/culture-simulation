using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Player
{
    public Vector2 position;
    public Dictionary<Belief, float> beliefScales;
}

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject personObject;
    [SerializeField] private int numberOfPlayers;
    [SerializeField] private int personSize;
    private Player[] players;
    private GameObject[] playerObjects;
    private SpriteRenderer[] playerRenderers;
    private BackgroundManager bgManager;

    public void Awake()
    {
        bgManager = FindObjectOfType<BackgroundManager>();
        players = new Player[numberOfPlayers];
        playerObjects = new GameObject[numberOfPlayers];
        playerRenderers = new SpriteRenderer[numberOfPlayers];
    }

    public void Update()
    {
        UpdateObjectPlayerColor();
    }

    public void CreatePlayers()
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

            Dictionary<Belief, float> currentBeliefs = new Dictionary<Belief, float>();
            foreach (Belief belief in Beliefs.AllBeliefs) { currentBeliefs.Add(belief, Random.value); }

            GameObject currentObject = Instantiate(
                personObject,
                position,
                Quaternion.identity
            );
            Player currentPlayer = new Player{position = position, beliefScales = currentBeliefs};

            players[playerIndex] = currentPlayer;
            playerObjects[playerIndex] = currentObject;
            playerRenderers[playerIndex] = currentObject.GetComponent<SpriteRenderer>();
        }
    }

    public void UpdateObjectPlayerColor()
    {
        for (int playerIndex = 0; playerIndex < numberOfPlayers; playerIndex++)
        {
            Player currentPlayer = players[playerIndex];
            Color averagePlayerColor = GetAveragePlayerColor(currentPlayer);
            playerRenderers[playerIndex].color = averagePlayerColor;
        }
    }

    private Color GetAveragePlayerColor(Player player)
    {
        Color newColor = Color.black;
        int beliefNumber = player.beliefScales.Count;
        
        float totalScale = 0;
        foreach (float value in player.beliefScales.Values) { totalScale += value; }

        foreach (Belief belief in Beliefs.AllBeliefs)
        {
            newColor += belief.color * player.beliefScales[belief] / totalScale;
        }

        return newColor;
    }
}
