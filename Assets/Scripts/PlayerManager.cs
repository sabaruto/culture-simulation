using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerManager : BelieverManager
{
    [SerializeField] private ComputeShader playerShader;
    [SerializeField] private float playerWeighting;
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
        var squareSize = Marshal.SizeOf(typeof(Member));
        var playerSize = Marshal.SizeOf(typeof(Member));
        var beliefSize = Marshal.SizeOf(typeof(Belief));

        var bufferSquares = bgManager.GetBufferMembers();

        var kernel = playerShader.FindKernel("PlayerUpdate");
        var squareBuffer = new ComputeBuffer(bufferSquares.Length, squareSize);
        var playerBuffer = new ComputeBuffer(Members.Length, playerSize);
        var beliefBuffer = new ComputeBuffer(Beliefs.Count, beliefSize);
        
        squareBuffer.SetData(bufferSquares);
        playerBuffer.SetData(Members);
        beliefBuffer.SetData(Beliefs.AllBeliefs);
        
        playerShader.SetBuffer(kernel, "squares", squareBuffer);
        playerShader.SetBuffer(kernel, "players", playerBuffer);
        playerShader.SetBuffer(kernel, "beliefs", beliefBuffer);
        playerShader.SetInt("width", bgManager.PixelWidth);
        playerShader.SetInt("height", bgManager.PixelHeight);
        playerShader.SetFloat("scale", bgManager.Scale);
        playerShader.SetFloat("playerWeighting", playerWeighting);
        playerShader.Dispatch(kernel, Members.Length, 1, 1);
        
        playerBuffer.GetData(Members);
        
        squareBuffer.Dispose();
        playerBuffer.Dispose();
        beliefBuffer.Dispose();
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