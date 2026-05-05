using UnityEngine;

[DisallowMultipleComponent]
public class ArenaVisualDirector : MonoBehaviour
{
    const string BackgroundName = "ArenaBackdrop";
    const string GridName = "ArenaGrid";
    const string BorderName = "ArenaBorder";
    const string MotesName = "ArenaMotes";

    [SerializeField] Vector2 halfExtents = new Vector2(4.35f, 8.35f);
    [SerializeField] Color baseColor = new Color(0.035f, 0.055f, 0.075f, 1f);
    [SerializeField] Color accentColor = new Color(0.25f, 0.85f, 1f, 1f);
    [SerializeField] Color dangerColor = new Color(1f, 0.32f, 0.18f, 1f);
    [SerializeField] int verticalLines = 8;
    [SerializeField] int horizontalLines = 14;
    [SerializeField] int moteCount = 24;
    [SerializeField] bool autoBuild = true;

    Transform gridRoot;
    Transform borderRoot;
    Transform motesRoot;
    Renderer backdropRenderer;
    Renderer[] gridRenderers = new Renderer[0];
    Renderer[] borderRenderers = new Renderer[0];
    Renderer[] moteRenderers = new Renderer[0];
    Vector3[] moteBasePositions = new Vector3[0];

    public static ArenaVisualDirector EnsureInScene(Vector2 playAreaHalfExtents, string levelTitle)
    {
        ArenaVisualDirector existing = FindObjectOfType<ArenaVisualDirector>();
        if (existing != null)
        {
            existing.Configure(playAreaHalfExtents, levelTitle);
            return existing;
        }

        var go = new GameObject("ArenaVisualDirector");
        var director = go.AddComponent<ArenaVisualDirector>();
        director.Configure(playAreaHalfExtents, levelTitle);
        return director;
    }

    public void Configure(Vector2 playAreaHalfExtents, string levelTitle)
    {
        halfExtents = new Vector2(
            Mathf.Max(2f, playAreaHalfExtents.x),
            Mathf.Max(3f, playAreaHalfExtents.y));

        if (!string.IsNullOrEmpty(levelTitle) && levelTitle.ToLowerInvariant().Contains("night"))
        {
            baseColor = new Color(0.045f, 0.035f, 0.08f, 1f);
            accentColor = new Color(0.8f, 0.42f, 1f, 1f);
            dangerColor = new Color(1f, 0.48f, 0.18f, 1f);
        }
        else
        {
            baseColor = new Color(0.035f, 0.075f, 0.055f, 1f);
            accentColor = new Color(0.28f, 0.95f, 0.65f, 1f);
            dangerColor = new Color(1f, 0.45f, 0.18f, 1f);
        }

        RebuildVisuals();
    }

    void Awake()
    {
        if (autoBuild && transform.childCount == 0)
            RebuildVisuals();
    }

    public void RebuildVisuals()
    {
        ClearGenerated();
        backdropRenderer = CreateQuad(BackgroundName, transform, Vector3.zero,
            new Vector2(halfExtents.x * 2.2f, halfExtents.y * 2.12f), baseColor, -50);

        gridRoot = new GameObject(GridName).transform;
        gridRoot.SetParent(transform, false);
        BuildGrid();

        borderRoot = new GameObject(BorderName).transform;
        borderRoot.SetParent(transform, false);
        BuildBorder();

        motesRoot = new GameObject(MotesName).transform;
        motesRoot.SetParent(transform, false);
        BuildMotes();
    }

    void Update()
    {
        float time = Time.unscaledTime;
        float pulse = 0.5f + Mathf.Sin(time * 1.4f) * 0.5f;

        if (backdropRenderer != null)
            backdropRenderer.material.color = Color.Lerp(baseColor, Color.Lerp(baseColor, accentColor, 0.18f), pulse * 0.45f);

        for (int i = 0; i < gridRenderers.Length; i++)
        {
            Color c = accentColor;
            c.a = Mathf.Lerp(0.08f, 0.22f, pulse);
            gridRenderers[i].material.color = c;
        }

        for (int i = 0; i < borderRenderers.Length; i++)
        {
            Color c = Color.Lerp(dangerColor, accentColor, pulse * 0.35f);
            c.a = Mathf.Lerp(0.45f, 0.85f, pulse);
            borderRenderers[i].material.color = c;
        }

        for (int i = 0; i < moteRenderers.Length; i++)
        {
            float local = time * (0.6f + i * 0.017f) + i;
            Transform mote = moteRenderers[i].transform;
            mote.localPosition = moteBasePositions[i] + new Vector3(
                Mathf.Sin(local * 1.7f) * 0.12f,
                Mathf.Cos(local * 1.1f) * 0.18f,
                0f);
            mote.localScale = Vector3.one * Mathf.Lerp(0.035f, 0.075f, 0.5f + Mathf.Sin(local * 2.1f) * 0.5f);

            Color c = accentColor;
            c.a = Mathf.Lerp(0.2f, 0.7f, 0.5f + Mathf.Sin(local) * 0.5f);
            moteRenderers[i].material.color = c;
        }
    }

    void BuildGrid()
    {
        int count = Mathf.Max(2, verticalLines) + Mathf.Max(2, horizontalLines);
        gridRenderers = new Renderer[count];
        int index = 0;

        for (int i = 0; i < Mathf.Max(2, verticalLines); i++)
        {
            float t = verticalLines <= 1 ? 0.5f : i / (verticalLines - 1f);
            float x = Mathf.Lerp(-halfExtents.x, halfExtents.x, t);
            Color color = accentColor;
            color.a = i == 0 || i == verticalLines - 1 ? 0.25f : 0.12f;
            gridRenderers[index++] = CreateQuad("GridV", gridRoot, new Vector3(x, 0f, 0.05f),
                new Vector2(0.025f, halfExtents.y * 2f), color, -40);
        }

        for (int i = 0; i < Mathf.Max(2, horizontalLines); i++)
        {
            float t = horizontalLines <= 1 ? 0.5f : i / (horizontalLines - 1f);
            float y = Mathf.Lerp(-halfExtents.y, halfExtents.y, t);
            Color color = accentColor;
            color.a = i == 0 || i == horizontalLines - 1 ? 0.25f : 0.1f;
            gridRenderers[index++] = CreateQuad("GridH", gridRoot, new Vector3(0f, y, 0.05f),
                new Vector2(halfExtents.x * 2f, 0.025f), color, -40);
        }
    }

    void BuildBorder()
    {
        borderRenderers = new Renderer[4];
        float width = 0.08f;
        borderRenderers[0] = CreateQuad("BorderTop", borderRoot, new Vector3(0f, halfExtents.y, -0.02f),
            new Vector2(halfExtents.x * 2f, width), dangerColor, -30);
        borderRenderers[1] = CreateQuad("BorderBottom", borderRoot, new Vector3(0f, -halfExtents.y, -0.02f),
            new Vector2(halfExtents.x * 2f, width), dangerColor, -30);
        borderRenderers[2] = CreateQuad("BorderLeft", borderRoot, new Vector3(-halfExtents.x, 0f, -0.02f),
            new Vector2(width, halfExtents.y * 2f), dangerColor, -30);
        borderRenderers[3] = CreateQuad("BorderRight", borderRoot, new Vector3(halfExtents.x, 0f, -0.02f),
            new Vector2(width, halfExtents.y * 2f), dangerColor, -30);
    }

    void BuildMotes()
    {
        int count = Mathf.Max(0, moteCount);
        moteRenderers = new Renderer[count];
        moteBasePositions = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            float angle = i * 137.508f * Mathf.Deg2Rad;
            float radius = Mathf.Lerp(0.2f, 1f, (i + 1f) / count);
            Vector3 pos = new Vector3(
                Mathf.Cos(angle) * halfExtents.x * radius,
                Mathf.Sin(angle) * halfExtents.y * radius,
                -0.04f);
            Color color = accentColor;
            color.a = 0.35f;
            moteBasePositions[i] = pos;
            moteRenderers[i] = CreateQuad("Mote", motesRoot, pos, Vector2.one * 0.055f, color, -35);
        }
    }

    void ClearGenerated()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyGeneratedObject(transform.GetChild(i).gameObject);
    }

    static Renderer CreateQuad(string name, Transform parent, Vector3 localPosition, Vector2 size, Color color, int sortingOrder)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = name;
        quad.transform.SetParent(parent, false);
        quad.transform.localPosition = localPosition;
        quad.transform.localScale = new Vector3(size.x, size.y, 1f);

        Collider collider = quad.GetComponent<Collider>();
        if (collider != null)
            DestroyGeneratedObject(collider.gameObject.GetComponent<Collider>());

        Renderer renderer = quad.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = CreateTransparentMaterial(color);
            renderer.sortingOrder = sortingOrder;
        }

        return renderer;
    }

    static Material CreateTransparentMaterial(Color color)
    {
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
            shader = Shader.Find("Unlit/Color");
        if (shader == null)
            shader = Shader.Find("Standard");

        var material = new Material(shader);
        material.color = color;
        return material;
    }

    static void DestroyGeneratedObject(Object obj)
    {
        if (obj == null)
            return;

        if (Application.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }
}
