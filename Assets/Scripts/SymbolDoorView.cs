using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime UI for the Symbol Door puzzle.
/// Builds scalable button-like tiles that stay inside the screen.
/// </summary>
public class SymbolDoorView : MonoBehaviour
{
    private const float TileTextPadding = 10f;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Image doorImage;
    [SerializeField] private CanvasGroup doorCanvasGroup;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip errorSound;

    private readonly Dictionary<string, SymbolButton> symbolMap = new();
    private readonly Dictionary<string, MeaningButton> meaningMap = new();
    private readonly Dictionary<string, RectTransform> connections = new();
    private Coroutine animationCoroutine;

    private LayoutGroup symbolLayout;
    private LayoutGroup meaningLayout;
    private RectTransform symbolContainer;
    private RectTransform meaningContainer;
    private Button checkButton;
    private Button resetButton;

    void Awake()
    {
        EnsureCanvas();
    }

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

        EnsureCanvas();
        ClearExistingLayout();
        CreateMainLayout(symbols, meanings, controller);
        ApplyProfessionalStyling();
    }

    private void EnsureCanvas()
    {
        if (canvas == null)
            canvas = GetComponent<Canvas>();

        if (canvas == null)
            canvas = gameObject.AddComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = GetComponent<CanvasScaler>();
        if (scaler == null)
            scaler = gameObject.AddComponent<CanvasScaler>();

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();
    }

    private void ClearExistingLayout()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        for (int i = canvas.transform.childCount - 1; i >= 0; i--)
            DestroyObject(canvas.transform.GetChild(i).gameObject);

        symbolMap.Clear();
        meaningMap.Clear();
        connections.Clear();

        symbolLayout = null;
        meaningLayout = null;
        symbolContainer = null;
        meaningContainer = null;
        checkButton = null;
        resetButton = null;
        doorImage = null;
        doorCanvasGroup = null;
    }

    private static void DestroyObject(GameObject obj)
    {
        if (obj == null)
            return;

        if (Application.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }

    private static void StretchToParent(RectTransform rect, float padding = 0f)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(padding, padding);
        rect.offsetMax = new Vector2(-padding, -padding);
        rect.pivot = new Vector2(0.5f, 0.5f);
    }

    private static LayoutElement AddLayoutElement(GameObject obj, float minHeight, float preferredHeight, float preferredWidth = -1f)
    {
        LayoutElement layout = obj.AddComponent<LayoutElement>();
        layout.minHeight = minHeight;
        layout.preferredHeight = preferredHeight;
        layout.flexibleWidth = 1f;

        if (preferredWidth > 0f)
            layout.preferredWidth = preferredWidth;

        return layout;
    }

    private static Text CreateText(Transform parent, string name)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);

        Text text = obj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
        return text;
    }

    private static void ApplyTileStyle(Button button, Image image, Outline outline, Shadow shadow, Color normalColor, Color accentColor)
    {
        image.color = normalColor;
        image.raycastTarget = true;

        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;

        ColorBlock colors = button.colors;
        colors.normalColor = normalColor;
        colors.highlightedColor = Color.Lerp(normalColor, accentColor, 0.28f);
        colors.pressedColor = Color.Lerp(normalColor, Color.black, 0.22f);
        colors.selectedColor = Color.Lerp(normalColor, accentColor, 0.4f);
        colors.disabledColor = new Color(normalColor.r, normalColor.g, normalColor.b, 0.35f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        if (outline != null)
        {
            outline.effectColor = SymbolDoorUIDesign.BorderGold;
            outline.effectDistance = new Vector2(2, 2);
        }

        if (shadow != null)
        {
            shadow.effectColor = new Color(0f, 0f, 0f, 0.65f);
            shadow.effectDistance = new Vector2(3, -3);
        }
    }

    private void CreateMainLayout(List<string> symbols, List<string> meanings, SymbolDoorController controller)
    {
        GameObject mainContainer = new GameObject("MainContainer");
        mainContainer.transform.SetParent(canvas.transform, false);
        RectTransform mainRect = mainContainer.AddComponent<RectTransform>();
        StretchToParent(mainRect);

        Image mainBg = mainContainer.AddComponent<Image>();
        mainBg.color = SymbolDoorUIDesign.DarkBg;

        CreateTitle(mainContainer.transform);

        GameObject contentArea = new GameObject("ContentArea");
        contentArea.transform.SetParent(mainContainer.transform, false);
        RectTransform contentRect = contentArea.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 0.18f);
        contentRect.anchorMax = new Vector2(1f, 0.84f);
        contentRect.offsetMin = new Vector2(48, 0);
        contentRect.offsetMax = new Vector2(-48, 0);

        HorizontalLayoutGroup hLayout = contentArea.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 28;
        hLayout.padding = new RectOffset(16, 16, 16, 16);
        hLayout.childAlignment = TextAnchor.MiddleCenter;
        hLayout.childControlWidth = true;
        hLayout.childControlHeight = true;
        hLayout.childForceExpandWidth = true;
        hLayout.childForceExpandHeight = true;

        CreateSymbolPanel(contentArea.transform, symbols, controller);
        CreateDoorPanel(contentArea.transform);
        CreateMeaningPanel(contentArea.transform, meanings, controller);
        CreateButtonArea(mainContainer.transform, controller);
    }

    private void CreateTitle(Transform parent)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.offsetMin = new Vector2(48, -96);
        titleRect.offsetMax = new Vector2(-48, -24);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "SYMBOL DOOR";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 32;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = SymbolDoorUIDesign.AccentGold;
        titleText.raycastTarget = false;
    }

    private void CreateSymbolPanel(Transform parent, List<string> symbols, SymbolDoorController controller)
    {
        GameObject panel = new GameObject("SymbolsPanel");
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();

        LayoutElement panelLayout = panel.AddComponent<LayoutElement>();
        panelLayout.minWidth = 190;
        panelLayout.preferredWidth = 250;
        panelLayout.flexibleWidth = 1f;

        VerticalLayoutGroup vLayout = panel.AddComponent<VerticalLayoutGroup>();
        vLayout.spacing = 18;
        vLayout.padding = new RectOffset(10, 10, 10, 10);
        vLayout.childAlignment = TextAnchor.MiddleCenter;
        vLayout.childControlWidth = true;
        vLayout.childControlHeight = true;
        vLayout.childForceExpandWidth = true;
        vLayout.childForceExpandHeight = false;

        symbolContainer = rect;
        symbolLayout = vLayout;

        foreach (string symbol in symbols)
            CreateSymbolButton(panel.transform, symbol, controller);
    }

    private void CreateSymbolButton(Transform parent, string symbol, SymbolDoorController controller)
    {
        GameObject btnObj = new GameObject(symbol);
        btnObj.transform.SetParent(parent, false);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 108);
        AddLayoutElement(btnObj, 92, 108);

        Image img = btnObj.AddComponent<Image>();
        Button btn = btnObj.AddComponent<Button>();
        Shadow shadow = btnObj.AddComponent<Shadow>();
        Outline outline = btnObj.AddComponent<Outline>();
        ApplyTileStyle(btn, img, outline, shadow, SymbolDoorUIDesign.DarkBg, SymbolDoorUIDesign.AccentGold);

        Text txt = CreateText(btnObj.transform, "Text");
        StretchToParent(txt.rectTransform, TileTextPadding);
        txt.text = symbol;
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

        LayoutElement layout = panel.AddComponent<LayoutElement>();
        layout.minWidth = 210;
        layout.preferredWidth = 250;
        layout.flexibleWidth = 0.75f;

        Image doorBg = panel.AddComponent<Image>();
        doorBg.color = SymbolDoorUIDesign.DarkBg;

        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = SymbolDoorUIDesign.AccentGold;
        outline.effectDistance = new Vector2(4, 4);

        Shadow shadow = panel.AddComponent<Shadow>();
        shadow.effectColor = SymbolDoorUIDesign.AccentGold;
        shadow.effectDistance = new Vector2(8, 8);

        Text doorText = CreateText(panel.transform, "DoorText");
        RectTransform doorTextRect = doorText.rectTransform;
        doorTextRect.anchorMin = new Vector2(0f, 0.36f);
        doorTextRect.anchorMax = new Vector2(1f, 0.92f);
        doorTextRect.offsetMin = new Vector2(12, 0);
        doorTextRect.offsetMax = new Vector2(-12, 0);
        doorText.text = "LOCK\nDOOR";
        doorText.fontSize = 42;
        doorText.fontStyle = FontStyle.Bold;
        doorText.alignment = TextAnchor.MiddleCenter;
        doorText.color = SymbolDoorUIDesign.AccentGold;

        CreateDoorSlots(panel.transform);

        doorImage = doorBg;
        doorCanvasGroup = panel.AddComponent<CanvasGroup>();
    }

    private void CreateDoorSlots(Transform parent)
    {
        GameObject row = new GameObject("SymbolSlots");
        row.transform.SetParent(parent, false);

        RectTransform rowRect = row.AddComponent<RectTransform>();
        rowRect.anchorMin = new Vector2(0.5f, 0.18f);
        rowRect.anchorMax = new Vector2(0.5f, 0.18f);
        rowRect.pivot = new Vector2(0.5f, 0.5f);
        rowRect.anchoredPosition = Vector2.zero;
        rowRect.sizeDelta = new Vector2(190, 40);

        HorizontalLayoutGroup slotsLayout = row.AddComponent<HorizontalLayoutGroup>();
        slotsLayout.spacing = 12;
        slotsLayout.childAlignment = TextAnchor.MiddleCenter;
        slotsLayout.childControlWidth = true;
        slotsLayout.childControlHeight = true;
        slotsLayout.childForceExpandWidth = false;
        slotsLayout.childForceExpandHeight = false;

        for (int i = 0; i < 3; i++)
        {
            GameObject slot = new GameObject("Slot");
            slot.transform.SetParent(row.transform, false);
            AddLayoutElement(slot, 34, 34, 50);

            Image image = slot.AddComponent<Image>();
            image.color = Color.Lerp(SymbolDoorUIDesign.DarkBg, SymbolDoorUIDesign.AccentGold, 0.18f);

            Outline slotOutline = slot.AddComponent<Outline>();
            slotOutline.effectColor = SymbolDoorUIDesign.BorderGold;
            slotOutline.effectDistance = new Vector2(1, 1);
        }
    }

    private void CreateMeaningPanel(Transform parent, List<string> meanings, SymbolDoorController controller)
    {
        GameObject panel = new GameObject("MeaningsPanel");
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();

        LayoutElement panelLayout = panel.AddComponent<LayoutElement>();
        panelLayout.minWidth = 210;
        panelLayout.preferredWidth = 280;
        panelLayout.flexibleWidth = 1f;

        VerticalLayoutGroup vLayout = panel.AddComponent<VerticalLayoutGroup>();
        vLayout.spacing = 18;
        vLayout.padding = new RectOffset(10, 10, 10, 10);
        vLayout.childAlignment = TextAnchor.MiddleCenter;
        vLayout.childControlWidth = true;
        vLayout.childControlHeight = true;
        vLayout.childForceExpandWidth = true;
        vLayout.childForceExpandHeight = false;

        meaningContainer = rect;
        meaningLayout = vLayout;

        foreach (string meaning in meanings)
            CreateMeaningButton(panel.transform, meaning, controller);
    }

    private void CreateMeaningButton(Transform parent, string meaning, SymbolDoorController controller)
    {
        GameObject btnObj = new GameObject(meaning);
        btnObj.transform.SetParent(parent, false);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 92);
        AddLayoutElement(btnObj, 78, 92);

        Image img = btnObj.AddComponent<Image>();
        Button btn = btnObj.AddComponent<Button>();
        Shadow shadow = btnObj.AddComponent<Shadow>();
        Outline outline = btnObj.AddComponent<Outline>();
        ApplyTileStyle(btn, img, outline, shadow, SymbolDoorUIDesign.DarkBg, SymbolDoorUIDesign.AccentCyan);

        Text txt = CreateText(btnObj.transform, "Text");
        StretchToParent(txt.rectTransform, TileTextPadding);
        txt.text = meaning;
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
        buttonRect.pivot = new Vector2(0.5f, 0f);
        buttonRect.anchoredPosition = new Vector2(0, 34);
        buttonRect.sizeDelta = new Vector2(460, 84);

        HorizontalLayoutGroup hLayout = buttonArea.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 18;
        hLayout.padding = new RectOffset(10, 10, 8, 8);
        hLayout.childAlignment = TextAnchor.MiddleCenter;
        hLayout.childControlWidth = true;
        hLayout.childControlHeight = true;
        hLayout.childForceExpandWidth = true;
        hLayout.childForceExpandHeight = true;

        CreateActionButton(buttonArea.transform, "CHECK", SymbolDoorUIDesign.AccentGold,
            () => controller.OnCheckClicked(), ref checkButton);

        CreateActionButton(buttonArea.transform, "RESET", SymbolDoorUIDesign.AccentCyan,
            () => controller.OnResetClicked(), ref resetButton);
    }

    private void CreateActionButton(Transform parent, string text, Color color, UnityEngine.Events.UnityAction onClick, ref Button btnRef)
    {
        GameObject btnObj = new GameObject(text);
        btnObj.transform.SetParent(parent, false);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 64);
        AddLayoutElement(btnObj, 58, 64);

        Image img = btnObj.AddComponent<Image>();
        Button btn = btnObj.AddComponent<Button>();
        Shadow shadow = btnObj.AddComponent<Shadow>();
        Outline outline = btnObj.AddComponent<Outline>();
        ApplyTileStyle(btn, img, outline, shadow, color, Color.white);
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(1, 1);

        Text txtComponent = CreateText(btnObj.transform, "Text");
        StretchToParent(txtComponent.rectTransform, 8);
        txtComponent.text = text;
        txtComponent.fontSize = 24;
        txtComponent.fontStyle = FontStyle.Bold;
        txtComponent.alignment = TextAnchor.MiddleCenter;
        txtComponent.color = Color.black;

        btn.onClick.AddListener(onClick);
        btnRef = btn;
    }

    private void ApplyProfessionalStyling()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (Application.isPlaying && isActiveAndEnabled)
            StartCoroutine(FadeInUI(canvasGroup));
        else
            canvasGroup.alpha = 1f;
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
        if (checkButton == null)
            return;

        Image checkImg = checkButton.GetComponent<Image>();
        if (checkImg != null)
            StartAnimationCoroutine(AnimateButtonError(checkImg));
    }

    private void StartAnimationCoroutine(IEnumerator anim)
    {
        if (!Application.isPlaying || !isActiveAndEnabled)
            return;

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
