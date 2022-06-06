using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderTest : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;

    [SerializeField] private RenderTexture renderTexture;

    [SerializeField] private Texture2D initialTexture2d;

    private Texture2D currentTexture2d;

    // Start is called before the first frame update
    void Start()
    {
        Render(initialTexture2d);
    }

    void Update()
    {
        Render(currentTexture2d);
    }

    void Render(Texture2D inputImage)
    {
        RenderTexture newTexture = new RenderTexture(renderTexture.width, renderTexture.height, 1);

        newTexture.enableRandomWrite = true;
        computeShader.SetTexture(0, "Result", newTexture);
        computeShader.SetTexture(0, "InputImage", inputImage);
        computeShader.SetInt("Resolution", renderTexture.width);
        computeShader.Dispatch(0, newTexture.width / 32, newTexture.height / 32, 1);

        Graphics.Blit(newTexture, renderTexture);
        currentTexture2d = TextureConversion.ConvertTexture(renderTexture);
    }
}