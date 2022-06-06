using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BackgroundManager : BelieverManager
{
    [SerializeField] private ComputeShader backgroundShader;
    [SerializeField] private float squareUpdateRate;
    [SerializeField] private float neighourWeighting, squareWeighting;
    [SerializeField] private float scale;
    [SerializeField] private int pixelWidth, pixelHeight;

    private float updateTimer;
    private Sprite sprite;
    private Texture2D tex2d;
    private RenderTexture renderTexture;
    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;
    private PlayerManager playerManager;

    public int PixelWidth => pixelWidth;
    public int PixelHeight => pixelHeight;
    public float Scale => scale;

    public void Awake()
    {
        gameManager = GetComponent<GameManager>();
        playerManager = GetComponent<PlayerManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        updateTimer = 0;

        renderTexture = new RenderTexture(pixelWidth, pixelHeight, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.antiAliasing = 1;
        renderTexture.filterMode = FilterMode.Point;

        members = new Member[pixelWidth * pixelHeight];
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

    // Updates the sprite with the current renderTexture
    public override void UpdateColors()
    {
        tex2d = new Texture2D
        (
            renderTexture.width,
            renderTexture.height,
            TextureFormat.RGB24,
            false
        );

        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = renderTexture;
        tex2d.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex2d.Apply();

        sprite = Sprite.Create
        (
            tex2d,
            new Rect(0.0f, 0.0f, tex2d.width, tex2d.height),
            new Vector2(0.5f, 0.5f),
            1 / scale
        );

        spriteRenderer.sprite = sprite;
    }

    public override void Create()
    {
        for (int x = 0; x < pixelWidth; x++)
        {
            for (int y = 0; y < pixelHeight; y++)
            {
                int currentIndex = x * pixelHeight + y;
                Vector2 currentBeliefs = new Vector2(Random.value, Random.value);

                Member currentSquare = new Member
                {
                    position = new Vector2Int(x, y),
                    beliefScales = currentBeliefs
                };
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
        ComputeBuffer workingSquareBuffer = new ComputeBuffer(members.Length, squareSize);
        ComputeBuffer playerBuffer = new ComputeBuffer(bufferPlayers.Length, playerSize);
        ComputeBuffer beliefBuffer = new ComputeBuffer(Beliefs.Count, beliefSize);

        squareBuffer.SetData(members);
        workingSquareBuffer.SetData(members);
        playerBuffer.SetData(bufferPlayers);
        beliefBuffer.SetData(Beliefs.AllBeliefs);

        backgroundShader.SetBuffer(kernel, "squares", squareBuffer);
        backgroundShader.SetBuffer(kernel, "workingSquares", workingSquareBuffer);
        backgroundShader.SetBuffer(kernel, "players", playerBuffer);
        backgroundShader.SetBuffer(kernel, "beliefs", beliefBuffer);
        backgroundShader.SetInt("width", pixelWidth);
        backgroundShader.SetInt("height", pixelHeight);
        backgroundShader.SetFloat("scale", scale);
        backgroundShader.SetFloat("neighbourWeighting", neighourWeighting);
        backgroundShader.SetFloat("squareWeighting", squareWeighting);
        backgroundShader.SetTexture(kernel, "result", renderTexture);
        backgroundShader.Dispatch(kernel, members.Length / 32, 1, 1);

        squareBuffer.GetData(members);

        squareBuffer.Dispose();
        workingSquareBuffer.Dispose();
        playerBuffer.Dispose();
        beliefBuffer.Dispose();
    }

    public Vector2 PixelToPosition(int pixelX, int pixelY)
    {
        return new Vector2(pixelX - pixelWidth * 0.5f + 0.5f, pixelY - pixelHeight * 0.5f + 0.5f) * scale;
    }
}