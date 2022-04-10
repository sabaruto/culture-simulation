using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public struct Square
{
    public Vector2 position;
    public Color color;
}

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private ComputeShader backgroundShader;
    [SerializeField] private Sprite squareSprite;
    [SerializeField] private float squareUpdateRate;
    [SerializeField] private float scale;
    [SerializeField] private int pixelWidth, pixelHeight;

    public int PixelWidth => pixelWidth;
    public int PixelHeight => pixelHeight;
    public float Scale => scale;

    private float updateTimer;
    private Square[] squares;
    private GameObject[] squareObjects;
    private GameManager gameManager;
    public void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        updateTimer = 0;
    }
    public void Start()
    {
        CreateSquares();
        UpdateSquares();
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

    private void CreateSquares()
    {
        squares = new Square[pixelWidth * pixelHeight];
        squareObjects = new GameObject[pixelWidth * pixelHeight];

        for (int x = 0; x < pixelWidth; x++)
        {
            for (int y = 0; y < pixelHeight; y++)
            {
                int currentIndex = x * pixelHeight + y;

                GameObject currentObject = new GameObject("Cube - x:" + x + " y:" + y, typeof(SpriteRenderer));
                SpriteRenderer currentSpriteSpr = currentObject.GetComponent<SpriteRenderer>();
                Square currentSquare = new Square();

                currentSpriteSpr.sprite = squareSprite;
                currentSpriteSpr.sortingLayerName = "Background";

                currentObject.transform.localScale = new Vector2(scale, scale);

                squareObjects[currentIndex] = currentObject;

                currentSquare.position = PixelToPosition(x, y);
                currentSquare.color = Random.ColorHSV();

                
                squares[currentIndex] = currentSquare;
                UpdateObject(currentIndex);
            }
        }
    }

    private void UpdateSquares()
    {
        int squareSize = Marshal.SizeOf(typeof(Square));
        int beliefSize = Marshal.SizeOf(typeof(Belief));

        int kernel = backgroundShader.FindKernel("CSMain");
        ComputeBuffer squareBuffer = new ComputeBuffer(squares.Length, squareSize);

        squareBuffer.SetData(squares);
        backgroundShader.SetBuffer(kernel, "squares", squareBuffer);
        backgroundShader.SetInt("width", pixelWidth);
        backgroundShader.SetInt("height", pixelHeight);
        backgroundShader.Dispatch(kernel, squares.Length / 32, 1, 1);

        squareBuffer.GetData(squares);
        squareBuffer.Dispose();

        for (int x = 0; x < pixelWidth; x++)
        for (int y = 0; y < pixelHeight; y++)
        {
            UpdateObject(x * pixelHeight + y);
        }
    }

    private void UpdateObject(int index)
    {
        GameObject currentObject = squareObjects[index];
        Square currentSquare = squares[index];

        currentObject.transform.position = currentSquare.position;
        currentObject.GetComponent<SpriteRenderer>().color = currentSquare.color;

        squareObjects[index] = currentObject;
    }

    public Vector2 PixelToPosition(int pixelX, int pixelY)
    {
        return new Vector2(pixelX - pixelWidth * 0.5f + 0.5f, pixelY - pixelHeight * 0.5f + 0.5f) * scale;
    }
}
