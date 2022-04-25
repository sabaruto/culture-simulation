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
    [SerializeField] private RenderTexture renderTexture;

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

        updateTimer = 0;

        renderTexture = new RenderTexture(pixelWidth, pixelHeight, 24);
        renderTexture.enableRandomWrite = true;

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

    public override void UpdateColors()
    {
        return;
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
                    position = PixelToPosition(x, y), 
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
