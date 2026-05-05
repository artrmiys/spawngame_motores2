using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Vertical gradient effect for UI Image elements.
/// Creates a smooth color transition from top to bottom.
/// </summary>
[RequireComponent(typeof(Graphic))]
public class UIGradient : BaseMeshEffect
{
    [SerializeField] private Color topColor = Color.white;
    [SerializeField] private Color bottomColor = Color.black;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentVertCount == 0) return;

        var verts = new System.Collections.Generic.List<UIVertex>();
        vh.GetUIVertexStream(verts);

        float bottomY = verts[0].position.y;
        float topY = verts[0].position.y;

        for (int i = 1; i < verts.Count; i++)
        {
            float y = verts[i].position.y;
            if (y > topY) topY = y;
            if (y < bottomY) bottomY = y;
        }

        float height = topY - bottomY;
        if (height <= 0) return;

        for (int i = 0; i < verts.Count; i++)
        {
            var vert = verts[i];
            float t = (vert.position.y - bottomY) / height;
            vert.color = Color.Lerp(bottomColor, topColor, t);
            verts[i] = vert;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(verts);
    }
}
