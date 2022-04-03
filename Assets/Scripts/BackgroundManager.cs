using UnityEngine;

public struct Square 
{
    public Vector2 position;
    public Color color;
    
}
public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private float scale;
    [SerializeField] private Sprite squareSprite;

    private Square[] squares;
    private GameObject[] squareObjects;
    public void Start()
    {
        CreateSquares(100, 100);
    }

    private void CreateSquares(int width, int height)
    {
        squares = new Square[width * height];
        squareObjects = new GameObject[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int currentIndex = x * height + y;

                GameObject currentObject = new GameObject("Cube - x:" + x + " y:" + y, typeof(SpriteRenderer));
                Square currentSquare = new Square();
                
                currentObject.GetComponent<SpriteRenderer>().sprite = squareSprite;
                currentObject.transform.localScale = new Vector2(scale, scale);

                squareObjects[currentIndex] = currentObject;

                currentSquare.position = new Vector2(x - width * 0.5f + 0.5f, y - height * 0.5f + 0.5f) * scale;
                currentSquare.color = Random.ColorHSV();
                
                squares[currentIndex] = currentSquare;
                UpdateObject(currentIndex);
            }
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


}
