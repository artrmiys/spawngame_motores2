using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visual design system for Symbol Door.
/// Handles colors, animations, and polish.
/// </summary>
public class SymbolDoorUIDesign
{
    public static readonly Color DarkBg = new Color(0.025f, 0.035f, 0.045f, 1f);
    public static readonly Color PanelBg = new Color(0.04f, 0.07f, 0.08f, 1f);
    public static readonly Color AccentGold = new Color(1f, 0.62f, 0.18f, 1f);
    public static readonly Color AccentCyan = new Color(0.16f, 0.86f, 1f, 1f);
    public static readonly Color ErrorRed = new Color(1f, 0.22f, 0.16f, 1f);
    public static readonly Color SuccessGreen = new Color(0.25f, 1f, 0.48f, 1f);
    public static readonly Color SymbolColor = new Color(0.92f, 0.98f, 1f, 1f);
    public static readonly Color BorderGold = new Color(0.95f, 0.46f, 0.14f, 1f);
    public static readonly Color WarningGlow = new Color(1f, 0.18f, 0.08f, 0.72f);

    // Animation parameters
    public const float HoverAnimDuration = 0.15f;
    public const float SelectAnimDuration = 0.2f;
    public const float ErrorFlashDuration = 0.15f;
    public const float SuccessPulseDuration = 0.4f;

    // Easing functions
    public static float EaseOutQuad(float t)
    {
        return 1f - (1f - t) * (1f - t);
    }

    public static float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
    }

    public static float EaseOutBounce(float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (t < 1f / d1)
            return n1 * t * t;
        else if (t < 2f / d1)
            return n1 * (t -= 1.5f / d1) * t + 0.75f;
        else if (t < 2.5f / d1)
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;
        else
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
    }
}
