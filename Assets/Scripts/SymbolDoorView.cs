using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Refactor #5: Professional UX/UI Design
/// Beautiful layout with proper spacing, shadows, visual hierarchy
/// </summary>
public class SymbolDoorView : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image doorImage;
    [SerializeField] private CanvasGroup doorCanvasGroup;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip errorSound;

    private Dictionary<string, SymbolButton> symbolMap = new();
    private Dictionary<string, MeaningButton> meaningMap = new();
    private Dictionary<string, RectTransform> connections = new();
    private Coroutine animationCoroutine;

    private LayoutGroup symbolLayout;
    private LayoutGroup meaningLayout;
    private RectTransform symbolContainer;
    private RectTransform meaningContainer;
    private Button checkButton;
    private Button resetButton;

    [System.Serializable]
    private class SymbolButton
    {
        public Button button;
        public Text text;
        public Image image;
        public Shadow shadow;
    }

    [System.Serializable]
    private class MeaningButton
    {
        public Button button;
        public Text text;
        public Image image;
        public Shadow shadow;
    }

    public void Setup(List<string> symbols, List<string> meanings, SymbolDoorController controller)
    {
        if (symbols == null || meanings == null || controller == null)
        {
            Debug.LogError("[SymbolDoorView] Invalid setup parameters!");
            return;
        }

        CreateMainLayout(symbols, meanings, controller);
        ApplyProfessionalStyling();
    }

    private void CreateMainLayout(List<string> symbols, List<string> meanings, SymbolDoorController controller)
    {
        // Main container
        GameObject mainContainer = new GameObject("MainContainer");
        mainContainer.transform.SetParent(canvas.transform, false);
        RectTransform mainRect = mainContainer.AddComponent<RectTransform>();
        mainRect.anchorMin = Vector2.zero;
        mainRect.anchorMax = Vector2.one;
        mainRect.offsetMin = Vector2.zero;
        mainRect.offsetMax = Vector2.zero;

        Image mainBg = mainContainer.AddComponent<Image>();
        mainBg.color = SymbolDoorUIDesign.DarkBg;

        // Title
        CreateTitle(mainContainer.transform);

        // Content area (horizontal layout: symbols | door | meanings)
        GameObject contentArea = new GameObject("ContentArea");
        contentArea.transform.SetParent(mainContainer.transform, false);
        RectTransform contentRect = contentArea.AddComponent<RectTransform>();
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(-100, -300);
        contentRect.offsetMin = new Vector2(50, 100);
        contentRect.offsetMax = new Vector2(-50, -150);

        HorizontalLayoutGroup hLayout = contentArea.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 40;
        hLayout.padding = new RectOffset(20, 20, 20, 20);
        hLayout.childForceExpandWidth = true;
        hLayout.childForceExpandHeight = true;

        // Symbols panel
        CreateSymbolPanel(contentArea.transform, symbols, controller);

        // Door panel (center)
        CreateDoorPanel(contentArea.transform);

        // Meanings panel
        CreateMeaningPanel(contentArea.transform, meanings, controller);

        // Button area
        CreateButtonArea(mainContainer.transform, controller);
    }

    private void CreateTitle(Transform parent)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, -40);
        titleRect.sizeDelta = new Vector2(-100, 60);
        titleRect.offsetMin = new Vector2(50, 0);
        titleRect.offsetMax = new Vector2(-50, 0);
        titleRect.anchorMin = new Vector2(0.5f, 1);
        titleRect.anchorMax = new Vector2(0.5f, 1);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "SYMBOL DOOR";
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.fontSize = 32;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = SymbolDoorUIDesign.AccentGold;
    }

    private void CreateSymbolPanel(Transform parent, List<string> symbols, SymbolDoorController controller)
    {
        GameObject panel = new GameObject("SymbolsPanel");
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();

        VerticalLayoutGroup vLayout = panel.AddComponent<VerticalLayoutGroup>();
        vLayout.spacing = 15;
        vLayout.padding = new RectOffset(15, 15, 15, 15);
        vLayout.childForceExpandHeight = true;

        symbolContainer = rect;
        symbolLayout = vLayout;

        foreach (string symbol in symbols)
        {
            CreateSymbolButton(panel.transform, symbol, controller);
        }
    }

    private void CreateSymbolButton(Transform parent, string symbol, SymbolDoorController controller)
    {
        GameObject btnObj = new GameObject(symbol);
        btnObj.transform.SetParent(parent, false);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(-20, 80);

        Button btn = btnObj.AddComponent<Button>();
        Image img = btnObj.AddComponent<Image>();
        img.color = SymbolDoorUIDesign.DarkBg;

        Shadow shadow = btnObj.AddComponent<Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(3, -3);

        Outline outline = btnObj.AddComponent<Outline>();
        outline.effectColor = SymbolDoorUIDesign.BorderGold;
        outline.effectDistance = new Vector2(2, 2);

        Text txt = new GameObject("Text").AddComponent<Text>();
        txt.transform.SetParent(btnObj.transform, false);
        txt.text = symbol;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = 52;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = SymbolDoorUIDesign.SymbolColor;

        string sym = symbol;
        btn.onClick.AddListener(() => controller.OnSymbolClicked(sym));

        symbolMap[symbol] = new SymbolButton { button = btn, text = txt, image = img, shadow = shadow };
    }

    private void CreateDoorPanel(Transform parent)
    {
        GameObject panel = new GameObject("DoorPanel");
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();

        // Door background
        Image doorBg = panel.AddComponent<Image>();
        doorBg.color = SymbolDoorUIDesign.DarkBg;

        // Door outer glow (border)
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = SymbolDoorUIDesign.AccentGold;
        outline.effectDistance = new Vector2(4, 4);

        Shadow shadow = panel.AddComponent<Shadow>();
        shadow.effectColor = SymbolDoorUIDesign.AccentGold;
        shadow.effectDistance = new Vector2(8, 8);

        // Door icon/text
        Text doorText = new GameObject("DoorText").AddComponent<Text>();
        doorText.transform.SetParent(panel.transform, false);
        doorText.text = "🚪\nDOOR";
        doorText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        doorText.fontSize = 48;
        doorText.fontStyle = FontStyle.Bold;
        doorText.alignment = TextAnchor.MiddleCenter;
        doorText.color = SymbolDoorUIDesign.AccentGold;

        doorImage = doorBg;
        doorCanvasGroup = panel.AddComponent<CanvasGroup>();
    }

    private void CreateMeaningPanel(Transform parent, List<string> meanings, SymbolDoorController controller)
    {
        GameObject panel = new GameObject("MeaningsPanel");
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();

        VerticalLayoutGroup vLayout = panel.AddComponent<VerticalLayoutGroup>();
        vLayout.spacing = 15;
        vLayout.padding = new RectOffset(15, 15, 15, 15);
        vLayout.childForceExpandHeight = true;

        meaningContainer = rect;
        meaningLayout = vLayout;

        foreach (string meaning in meanings)
        {
            CreateMeaningButton(panel.transform, meaning, controller);
        }
    }

    private void CreateMeaningButton(Transform parent, string meaning, SymbolDoorController controller)
    {
        GameObject btnObj = new GameObject(meaning);
        btnObj.transform.SetParent(parent, false);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(-20, 70);

        Button btn = btnObj.AddComponent<Button>();
        Image img = btnObj.AddComponent<Image>();
        img.color = SymbolDoorUIDesign.DarkBg;

        Shadow shadow = btnObj.AddComponent<Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(2, -2);

        Outline outline = btnObj.AddComponent<Outline>();
        outline.effectColor = SymbolDoorUIDesign.BorderGold;
        outline.effectDistance = new Vector2(1, 1);

        Text txt = new GameObject("Text").AddComponent<Text>();
        txt.transform.SetParent(btnObj.transform, false);
        txt.text = meaning;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = 28;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = SymbolDoorUIDesign.SymbolColor;
        txt.horizontalOverflow = HorizontalWrapMode.Wrap;
        txt.verticalOverflow = VerticalWrapMode.Truncate;

        string mean = meaning;
        btn.onClick.AddListener(() => controller.OnMeaningClicked(mean));

        meaningMap[meaning] = new MeaningButton { button = btn, text = txt, image = img, shadow = shadow };
    }

    private void CreateButtonArea(Transform parent, SymbolDoorController controller)
    {
        GameObject buttonArea = new GameObject("ButtonArea");
        buttonArea.transform.SetParent(parent, false);
        RectTransform buttonRect = buttonArea.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0);
        buttonRect.anchorMax = new Vector2(0.5f, 0);
        buttonRect.anchoredPosition = new Vector2(0, 50);
        buttonRect.sizeDelta = new Vector2(400, 70);

        HorizontalLayoutGroup hLayout = buttonArea.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 20;
        hLayout.padding = new RectOffset(10, 10, 10, 10);
        hLayout.childForceExpandWidth = true;

        // Check button
        CreateActionButton(buttonArea.transform, "ПРОВЕРИТЬ", SymbolDoorUIDesign.AccentGold,
            () => controller.OnCheckClicked(), ref checkButton);

        // Reset button
        CreateActionButton(buttonArea.transform, "СБРОС", SymbolDoorUIDesign.AccentCyan,
            () => controller.OnResetClicked(), ref resetButton);
    }

    private void CreateActionButton(Transform parent, string text, Color color, UnityEngine.Events.UnityAction onClick, ref Button btnRef)
    {
        GameObject btnObj = new GameObject(text);
        btnObj.transform.SetParent(parent, false);

        Button btn = btnObj.AddComponent<Button>();
        Image img = btnObj.AddComponent<Image>();
        img.color = color;

        Shadow shadow = btnObj.AddComponent<Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(2, -2);

        Outline outline = btnObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(1, 1);

        Text txtComponent = new GameObject("Text").AddComponent<Text>();
        txtComponent.transform.SetParent(btnObj.transform, false);
        txtComponent.text = text;
        txtComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txtComponent.fontSize = 24;
        txtComponent.fontStyle = FontStyle.Bold;
        txtComponent.alignment = TextAnchor.MiddleCenter;
        txtComponent.color = Color.black;

        btn.onClick.AddListener(onClick);
        btnRef = btn;
    }

    private void ApplyProfessionalStyling()
    {
        // Fade in animation
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        StartCoroutine(FadeInUI(canvasGroup));
    }

    private IEnumerator FadeInUI(CanvasGroup cg)
    {
        cg.alpha = 0;
        float duration = 0.5f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = SymbolDoorUIDesign.EaseOutQuad(elapsed / duration);
            yield return null;
        }

        cg.alpha = 1f;
    }

    public void HighlightSymbol(string symbol, bool highlight)
    {
        if (!symbolMap.TryGetValue(symbol, out var btn))
            return;

        if (highlight)
        {
            StartAnimationCoroutine(AnimateButtonSelect(btn.image, SymbolDoorUIDesign.AccentGold));
        }
        else
        {
            btn.image.color = SymbolDoorUIDesign.DarkBg;
        }
    }

    public void ShowConnection(string symbol, string meaning)
    {
        if (!meaningMap.TryGetValue(meaning, out var btn))
            return;

        StartAnimationCoroutine(AnimateButtonSelect(btn.image, SymbolDoorUIDesign.AccentCyan));
    }

    public void HideConnection(string symbol, string meaning)
    {
        if (!meaningMap.TryGetValue(meaning, out var btn))
            return;

        btn.image.color = SymbolDoorUIDesign.DarkBg;
    }

    public void ClearAllConnections()
    {
        foreach (var btn in symbolMap.Values)
        {
            if (btn?.image != null)
                btn.image.color = SymbolDoorUIDesign.DarkBg;
        }

        foreach (var btn in meaningMap.Values)
        {
            if (btn?.image != null)
                btn.image.color = SymbolDoorUIDesign.DarkBg;
        }
    }

    public void PlaySuccessAnimation()
    {
        StartAnimationCoroutine(DoSuccessAnimation());
    }

    public void PlayErrorAnimation()
    {
        StartAnimationCoroutine(DoErrorAnimation());
    }

    public void ShowIncomplete()
    {
        if (checkButton == null) return;
        Image checkImg = checkButton.GetComponent<Image>();
        if (checkImg != null)
            StartAnimationCoroutine(AnimateButtonError(checkImg));
    }

    private void StartAnimationCoroutine(IEnumerator anim)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(anim);
    }

    private IEnumerator AnimateButtonSelect(Image img, Color targetColor)
    {
        float duration = SymbolDoorUIDesign.SelectAnimDuration;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = SymbolDoorUIDesign.EaseOutQuad(t);
            img.color = Color.Lerp(SymbolDoorUIDesign.DarkBg, targetColor, t);
            yield return null;
        }

        img.color = targetColor;
    }

    private IEnumerator AnimateButtonError(Image img)
    {
        Color original = img.color;

        for (int i = 0; i < 3; i++)
        {
            float elapsed = 0;
            while (elapsed < SymbolDoorUIDesign.ErrorFlashDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / SymbolDoorUIDesign.ErrorFlashDuration;
                img.color = Color.Lerp(original, SymbolDoorUIDesign.ErrorRed, t);
                yield return null;
            }

            elapsed = 0;
            while (elapsed < SymbolDoorUIDesign.ErrorFlashDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / SymbolDoorUIDesign.ErrorFlashDuration;
                img.color = Color.Lerp(SymbolDoorUIDesign.ErrorRed, original, t);
                yield return null;
            }
        }

        img.color = original;
    }

    private IEnumerator DoSuccessAnimation()
    {
        if (successSound != null)
            AudioSource.PlayClipAtPoint(successSound, Vector3.zero);

        if (doorCanvasGroup != null)
        {
            float elapsed = 0;

            while (elapsed < SymbolDoorUIDesign.SuccessPulseDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / SymbolDoorUIDesign.SuccessPulseDuration;
                t = SymbolDoorUIDesign.EaseOutQuad(t);

                doorCanvasGroup.alpha = Mathf.Lerp(1f, 0.6f, t * 0.4f);

                if (doorImage != null)
                    doorImage.color = Color.Lerp(SymbolDoorUIDesign.DarkBg, SymbolDoorUIDesign.SuccessGreen, t * 0.3f);

                yield return null;
            }

            doorCanvasGroup.alpha = 1f;
        }
    }

    private IEnumerator DoErrorAnimation()
    {
        if (errorSound != null)
            AudioSource.PlayClipAtPoint(errorSound, Vector3.zero);

        if (doorCanvasGroup == null)
            yield break;

        Color originalColor = doorImage != null ? doorImage.color : SymbolDoorUIDesign.DarkBg;

        for (int i = 0; i < 4; i++)
        {
            float elapsed = 0;
            float duration = 0.15f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                doorCanvasGroup.alpha = Mathf.Lerp(1f, 0.3f, t);
                if (doorImage != null)
                    doorImage.color = Color.Lerp(originalColor, SymbolDoorUIDesign.ErrorRed, t * 0.5f);

                yield return null;
            }

            elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                doorCanvasGroup.alpha = Mathf.Lerp(0.3f, 1f, t);
                if (doorImage != null)
                    doorImage.color = Color.Lerp(SymbolDoorUIDesign.ErrorRed, originalColor, t * 0.5f);

                yield return null;
            }
        }

        doorCanvasGroup.alpha = 1f;
        if (doorImage != null)
            doorImage.color = originalColor;
    }
}
