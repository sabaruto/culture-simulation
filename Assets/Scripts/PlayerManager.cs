using UnityEngine;

public class PlayerManager : BelieverManager
{
    [SerializeField] protected GameObject obj;
    [SerializeField] private int numberOfPlayers;
    [SerializeField] private int personSize;
    private BackgroundManager bgManager;
    private GameObject[] memberObjects;

    public void Awake()
    {
        bgManager = FindObjectOfType<BackgroundManager>();

        memberObjects = new GameObject[numberOfPlayers];
        MemberRenderers = new SpriteRenderer[numberOfPlayers];
        Members = new Member[numberOfPlayers];
    }

    protected override void UpdateMembers()
    {
        // Todo: Attach playerUpdate.compute to this function
    }

    public override void Create()
    {
        var pixelWidth = bgManager.PixelWidth;
        var pixelHeight = bgManager.PixelHeight;
        var personRadius = personSize / 2;

        var minPosition = new Vector2Int(personRadius - 1, personRadius - 1);
        var maxPosition = new Vector2Int(pixelWidth - personRadius, pixelHeight - personRadius);

        for (var playerIndex = 0; playerIndex < numberOfPlayers; playerIndex++)
        {
            var position = new Vector2Int(
                Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y, maxPosition.y)
            );

            var currentBeliefs = new Vector2(Random.value, Random.value);

            var currentObject = Instantiate(
                obj,
                bgManager.PixelToPosition(position[0], position[1]),
                Quaternion.identity
            );
            var currentPlayer = new Member { position = position, beliefScales = currentBeliefs };

            memberObjects[playerIndex] = currentObject;
            MemberRenderers[playerIndex] = currentObject.GetComponent<SpriteRenderer>();
            Members[playerIndex] = currentPlayer;
        }
    }
}