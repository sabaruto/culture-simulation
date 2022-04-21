using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BackgroundManager : BelieverManager
{
    [SerializeField] private ComputeShader backgroundShader;
    [SerializeField] private Sprite squareSprite;
    [SerializeField] private float squareUpdateRate;
    [SerializeField] private float neighourWeighting, squareWeighting;
    [SerializeField] private float scale;
    [SerializeField] private int pixelWidth, pixelHeight;

    public int PixelWidth => pixelWidth;
    public int PixelHeight => pixelHeight;
    public float Scale => scale;

    private float updateTimer;
    private GameManager gameManager;
    private PlayerManager playerManager;
    public void Awake()
    {
        gameManager = GetComponent<GameManager>();
        playerManager = GetComponent<PlayerManager>();
        updateTimer = 1 / squareUpdateRate;
        Initialize(pixelWidth * pixelHeight);
    }
    public void Start()
    {
        Create();
    }

    public void FixedUpdate()
    {
        updateTimer += Time.fixedDeltaTime;

        if (updateTimer > 1 / squareUpdateRate)
        {
            updateTimer = 0;
            UpdateSquares();
        }
    }

    public override void Create()
    {
        for (int x = 0; x < pixelWidth; x++)
        {
            for (int y = 0; y < pixelHeight; y++)
            {
                int currentIndex = x * pixelHeight + y;

                GameObject currentObject = new GameObject("Cube - x:" + x + " y:" + y, typeof(SpriteRenderer));
                SpriteRenderer currentSpriteSpr = currentObject.GetComponent<SpriteRenderer>();
                Vector2 currentBeliefs = new Vector2(Random.value, Random.value);

                currentSpriteSpr.sprite = squareSprite;
                currentSpriteSpr.sortingLayerName = "Background";

                currentObject.transform.position = PixelToPosition(x, y);
                currentObject.transform.localScale = new Vector2(scale, scale);

                memberObjects[currentIndex] = currentObject;

                Member currentSquare = new Member
                {
                    position = PixelToPosition(x, y), 
                    beliefScales = currentBeliefs
                };

                memberObjects[currentIndex] = currentObject;
                memberRenderers[currentIndex] = currentObject.GetComponent<SpriteRenderer>();
                members[currentIndex] = currentSquare;
            }
        }
    }

    private void UpdateSquares()
    {
        int squareSize = Marshal.SizeOf(typeof(Member));
        int playerSize = Marshal.SizeOf(typeof(PlayerManager.Member));
        int beliefSize = Marshal.SizeOf(typeof(Belief));

        PlayerManager.Member[] bufferPlayers = playerManager.GetBufferMembers();

        int kernel = backgroundShader.FindKernel("BackgroundUpdate");
        ComputeBuffer squareBuffer = new ComputeBuffer(members.Length, squareSize);
        ComputeBuffer playerBuffer = new ComputeBuffer(bufferPlayers.Length, playerSize);
        ComputeBuffer beliefBuffer = new ComputeBuffer(Beliefs.Count, beliefSize);

        squareBuffer.SetData(members);
        playerBuffer.SetData(bufferPlayers);
        beliefBuffer.SetData(Beliefs.AllBeliefs);

        backgroundShader.SetBuffer(kernel, "squares", squareBuffer);
        backgroundShader.SetBuffer(kernel, "players", playerBuffer);
        backgroundShader.SetBuffer(kernel, "beliefs", beliefBuffer);
        backgroundShader.SetInt("width", pixelWidth);
        backgroundShader.SetInt("height", pixelHeight);
        backgroundShader.SetFloat("neighbourWeighting", neighourWeighting);
        backgroundShader.SetFloat("squareWeighting", squareWeighting);
        backgroundShader.Dispatch(kernel, members.Length / 32, 1, 1);

        squareBuffer.GetData(members);
        
        squareBuffer.Dispose();
        playerBuffer.Dispose();
        beliefBuffer.Dispose();
    }

    public Vector2 PixelToPosition(int pixelX, int pixelY)
    {
        return new Vector2(pixelX - pixelWidth * 0.5f + 0.5f, pixelY - pixelHeight * 0.5f + 0.5f) * scale;
    }
}
