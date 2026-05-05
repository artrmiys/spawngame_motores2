#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

public class VisualDesignEditModeTests
{
    [Test]
    public void ArenaVisualDirectorBuildsBackdropGridBorderAndMotes()
    {
        GameObject arenaObject = new GameObject("ArenaVisualDirectorTest");

        try
        {
            ArenaVisualDirector director = arenaObject.AddComponent<ArenaVisualDirector>();
            director.Configure(new Vector2(4.35f, 8.35f), "Level 2 - Night Swarm");

            Assert.IsNotNull(arenaObject.transform.Find("ArenaBackdrop"));
            Assert.IsNotNull(arenaObject.transform.Find("ArenaGrid"));
            Assert.IsNotNull(arenaObject.transform.Find("ArenaBorder"));
            Assert.IsNotNull(arenaObject.transform.Find("ArenaMotes"));
            Assert.Greater(arenaObject.GetComponentsInChildren<Renderer>(true).Length, 12);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(arenaObject);
        }
    }

    [Test]
    public void ActorVisualFxBuildsAuraAndCore()
    {
        GameObject actor = GameObject.CreatePrimitive(PrimitiveType.Capsule);

        try
        {
            ActorVisualFx fx = ActorVisualFx.Ensure(actor, ActorVisualFx.VisualRole.Player, new Color(0.25f, 0.8f, 1f));

            Assert.IsNotNull(fx);
            Assert.IsNotNull(actor.transform.Find("ActorVisualFx"));
            Assert.IsNotNull(actor.transform.Find("ActorVisualFx/Aura"));
            Assert.IsNotNull(actor.transform.Find("ActorVisualFx/CoreGlow"));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(actor);
        }
    }
}
#endif
