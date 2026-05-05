using UnityEngine;

[DisallowMultipleComponent]
public class ActorVisualFx : MonoBehaviour
{
    public enum VisualRole { Player, Enemy, FastEnemy, Projectile }

    const string VisualRootName = "ActorVisualFx";
    const string AuraName = "Aura";
    const string CoreName = "CoreGlow";

    [SerializeField] VisualRole role = VisualRole.Enemy;
    [SerializeField] Color accentColor = Color.white;
    [SerializeField] float auraScale = 1.2f;
    [SerializeField] float pulseSpeed = 4f;
    [SerializeField] bool leaveTrail;

    Transform visualRoot;
    Renderer auraRenderer;
    Renderer coreRenderer;
    float trailTimer;
    bool configured;

    public static ActorVisualFx Ensure(GameObject target, VisualRole role, Color color)
    {
        if (target == null)
            return null;

        ActorVisualFx fx = target.GetComponent<ActorVisualFx>();
        if (fx == null)
            fx = target.AddComponent<ActorVisualFx>();

        fx.Configure(role, color);
        return fx;
    }

    public void Configure(VisualRole newRole, Color color)
    {
        role = newRole;
        accentColor = color;
        configured = true;

        switch (role)
        {
            case VisualRole.Player:
                auraScale = 1.55f;
                pulseSpeed = 4.5f;
                leaveTrail = true;
                break;
            case VisualRole.FastEnemy:
                auraScale = 1.35f;
                pulseSpeed = 7.5f;
                leaveTrail = true;
                break;
            case VisualRole.Projectile:
                auraScale = 1.9f;
                pulseSpeed = 9f;
                leaveTrail = true;
                break;
            default:
                auraScale = 1.15f;
                pulseSpeed = 3.4f;
                leaveTrail = false;
                break;
        }

        RebuildVisuals();
    }

    void Start()
    {
        if (configured)
            return;

        Renderer renderer = GetComponent<Renderer>();
        Color color = renderer != null ? renderer.material.color : Color.white;
        Configure(role, color);
    }

    public void RebuildVisuals()
    {
        ClearVisuals();

        visualRoot = new GameObject(VisualRootName).transform;
        visualRoot.SetParent(transform, false);
        visualRoot.localPosition = Vector3.zero;
        visualRoot.localRotation = Quaternion.identity;

        auraRenderer = CreateVisualQuad(AuraName, visualRoot, Vector3.zero, Vector2.one * auraScale, accentColor, -1);
        coreRenderer = CreateVisualQuad(CoreName, visualRoot, new Vector3(0f, 0f, -0.015f),
            Vector2.one * Mathf.Max(0.28f, auraScale * 0.42f), Color.Lerp(accentColor, Color.white, 0.35f), 1);
    }

    void Update()
    {
        if (visualRoot == null)
            return;

        float time = Time.time;
        float pulse = 0.5f + Mathf.Sin(time * pulseSpeed) * 0.5f;
        visualRoot.localRotation = Quaternion.Euler(0f, 0f, time * GetSpinSpeed());

        if (auraRenderer != null)
        {
            float scale = Mathf.Lerp(auraScale * 0.85f, auraScale * 1.15f, pulse);
            auraRenderer.transform.localScale = Vector3.one * scale;
            Color color = accentColor;
            color.a = Mathf.Lerp(0.16f, 0.38f, pulse);
            auraRenderer.material.color = color;
        }

        if (coreRenderer != null)
        {
            float scale = Mathf.Lerp(auraScale * 0.28f, auraScale * 0.46f, pulse);
            coreRenderer.transform.localScale = Vector3.one * scale;
            Color color = Color.Lerp(accentColor, Color.white, 0.4f);
            color.a = Mathf.Lerp(0.28f, 0.65f, pulse);
            coreRenderer.material.color = color;
        }

        if (leaveTrail)
            TickTrail();
    }

    void TickTrail()
    {
        trailTimer -= Time.deltaTime;
        if (trailTimer > 0f)
            return;

        trailTimer = role == VisualRole.Projectile ? 0.045f : 0.09f;
        VisualAfterimage.Spawn(transform, accentColor, role == VisualRole.Projectile ? 0.16f : 0.28f);
    }

    float GetSpinSpeed()
    {
        return role switch
        {
            VisualRole.Player => 42f,
            VisualRole.FastEnemy => -140f,
            VisualRole.Projectile => 220f,
            _ => 18f
        };
    }

    void ClearVisuals()
    {
        Transform existing = transform.Find(VisualRootName);
        if (existing != null)
            DestroyGeneratedObject(existing.gameObject);
    }

    static Renderer CreateVisualQuad(string name, Transform parent, Vector3 localPosition, Vector2 size, Color color, int sortingOrder)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = name;
        quad.transform.SetParent(parent, false);
        quad.transform.localPosition = localPosition;
        quad.transform.localScale = new Vector3(size.x, size.y, 1f);

        Collider collider = quad.GetComponent<Collider>();
        if (collider != null)
            DestroyGeneratedObject(collider);

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

public class VisualAfterimage : MonoBehaviour
{
    Renderer rend;
    Color color;
    float lifetime;
    float elapsed;
    Vector3 startScale;

    public static void Spawn(Transform source, Color color, float lifetime)
    {
        if (source == null)
            return;

        GameObject ghost = GameObject.CreatePrimitive(PrimitiveType.Quad);
        ghost.name = "Afterimage";
        ghost.transform.position = source.position;
        ghost.transform.rotation = source.rotation;
        ghost.transform.localScale = source.lossyScale * 1.15f;

        Collider collider = ghost.GetComponent<Collider>();
        if (collider != null)
            Destroy(collider);

        var afterimage = ghost.AddComponent<VisualAfterimage>();
        afterimage.Initialize(color, lifetime);
    }

    void Initialize(Color newColor, float newLifetime)
    {
        color = newColor;
        color.a = 0.32f;
        lifetime = Mathf.Max(0.05f, newLifetime);
        startScale = transform.localScale;
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
                shader = Shader.Find("Unlit/Color");
            if (shader == null)
                shader = Shader.Find("Standard");
            rend.material = new Material(shader);
            rend.material.color = color;
            rend.sortingOrder = -2;
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / lifetime);
        transform.localScale = Vector3.Lerp(startScale, startScale * 1.65f, t);

        if (rend != null)
        {
            Color c = color;
            c.a = Mathf.Lerp(color.a, 0f, t);
            rend.material.color = c;
        }

        if (t >= 1f)
            Destroy(gameObject);
    }
}
