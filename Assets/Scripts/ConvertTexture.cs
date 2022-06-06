using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertTexture : MonoBehaviour
{
    [SerializeField] private RenderTexture texture;
    [SerializeReference] private Texture2D tex2d;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        tex2d = TextureConversion.ConvertTexture(texture);

        sr.sprite = Sprite.Create(tex2d, new Rect(0.0f, 0.0f, tex2d.width, tex2d.height), new Vector2(0.5f, 0.5f),
            tex2d.width);
    }
}