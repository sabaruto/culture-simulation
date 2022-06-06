using System.Runtime.InteropServices;
using UnityEngine;

public class BackgroundManager : BelieverManager
{
    [SerializeField] private ComputeShader backgroundShader;
    [SerializeField] private float neighbourWeighting, squareWeighting;
    [SerializeField] private float scale;
    [SerializeField] private int pixelWidth, pixelHeight;
    private PlayerManager playerManager;
    private RenderTexture renderTexture;
    private Sprite sprite;
    private SpriteRenderer spriteRenderer;
    private Texture2D tex2d;

    public int PixelWidth => pixelWidth;
    public int PixelHeight => pixelHeight;
    public float Scale => scale;

    public void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        renderTexture = new RenderTexture(pixelWidth, pixelHeight, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.antiAliasing = 1;
        renderTexture.filterMode = FilterMode.Point;

        Members = new Member[pixelWidth * pixelHeight];
    }

    public void Start()
    {
        Create();
    }

    // Updates the sprite with the current renderTexture
    protected override void UpdateColors()
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
        for (var x = 0; x < pixelWidth; x++)
        for (var y = 0; y < pixelHeight; y++)
        {
            var currentIndex = x * pixelHeight + y;
            var currentBeliefs = new Vector2(Random.value, Random.value);

            var currentSquare = new Member
            {
                position = new Vector2Int(x, y),
                beliefScales = currentBeliefs
            };
            Members[currentIndex] = currentSquare;
        }
    }

    protected override void UpdateMembers()
    {
        var squareSize = Marshal.SizeOf(typeof(Member));
        var playerSize = Marshal.SizeOf(typeof(Member));
        var beliefSize = Marshal.SizeOf(typeof(Belief));

        var bufferPlayers = playerManager.GetBufferMembers();

        var kernel = backgroundShader.FindKernel("BackgroundUpdate");
        var squareBuffer = new ComputeBuffer(Members.Length, squareSize);
        var workingSquareBuffer = new ComputeBuffer(Members.Length, squareSize);
        var playerBuffer = new ComputeBuffer(bufferPlayers.Length, playerSize);
        var beliefBuffer = new ComputeBuffer(Beliefs.Count, beliefSize);

        squareBuffer.SetData(Members);
        workingSquareBuffer.SetData(Members);
        playerBuffer.SetData(bufferPlayers);
        beliefBuffer.SetData(Beliefs.AllBeliefs);

        backgroundShader.SetBuffer(kernel, "squares", squareBuffer);
        backgroundShader.SetBuffer(kernel, "workingSquares", workingSquareBuffer);
        backgroundShader.SetBuffer(kernel, "players", playerBuffer);
        backgroundShader.SetBuffer(kernel, "beliefs", beliefBuffer);
        backgroundShader.SetInt("width", pixelWidth);
        backgroundShader.SetInt("height", pixelHeight);
        backgroundShader.SetFloat("scale", scale);
        backgroundShader.SetFloat("neighbourWeighting", neighbourWeighting);
        backgroundShader.SetFloat("squareWeighting", squareWeighting);
        backgroundShader.SetTexture(kernel, "result", renderTexture);
        backgroundShader.Dispatch(kernel, Members.Length / 32, 1, 1);

        squareBuffer.GetData(Members);

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