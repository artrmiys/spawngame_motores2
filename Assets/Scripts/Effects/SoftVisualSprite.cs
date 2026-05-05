using UnityEngine;

public static class SoftVisualSprite
{
    const int TextureSize = 96;

    static Sprite softCircleSprite;

    public static Sprite SoftCircle
    {
        get
        {
            if (softCircleSprite == null)
                softCircleSprite = CreateSoftCircleSprite();

            return softCircleSprite;
        }
    }

    public static SpriteRenderer CreateRenderer(string name, Transform parent, Vector3 localPosition, Vector2 size, Color color, int sortingOrder)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = SoftCircle;
        renderer.color = color;
        renderer.sortingOrder = sortingOrder;
        return renderer;
    }

    public static void SetRendererColor(Renderer renderer, Color color)
    {
        if (renderer == null)
            return;

        if (renderer is SpriteRenderer spriteRenderer)
        {
            spriteRenderer.color = color;
            return;
        }

        renderer.material.color = color;
    }

    public static float Smooth01(float value)
    {
        float t = Mathf.Clamp01(value);
        return t * t * (3f - 2f * t);
    }

    public static float EaseOut(float value)
    {
        float t = Mathf.Clamp01(value);
        float inverse = 1f - t;
        return 1f - inverse * inverse * inverse;
    }

    static Sprite CreateSoftCircleSprite()
    {
        var texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false)
        {
            name = "SoftVisualCircleTexture",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };

        var pixels = new Color32[TextureSize * TextureSize];
        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                float u = ((x + 0.5f) / TextureSize) * 2f - 1f;
                float v = ((y + 0.5f) / TextureSize) * 2f - 1f;
                float distance = Mathf.Sqrt(u * u + v * v);
                float edgeFade = 1f - Mathf.SmoothStep(0.22f, 1f, distance);
                float softAlpha = Mathf.Clamp01(edgeFade * edgeFade);
                pixels[y * TextureSize + x] = new Color(1f, 1f, 1f, softAlpha);
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply(false, true);

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, TextureSize, TextureSize),
            new Vector2(0.5f, 0.5f),
            TextureSize);
        sprite.name = "SoftVisualCircle";
        sprite.hideFlags = HideFlags.HideAndDontSave;
        return sprite;
    }
}
