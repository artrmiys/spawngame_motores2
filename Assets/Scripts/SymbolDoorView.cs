using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Refactor #4: Visual Design & Polish
/// Mystical aesthetic with visible connections, smooth animations, gold/cyan color scheme
/// </summary>
public class SymbolDoorView : MonoBehaviour
{
    [SerializeField] private Transform symbolContainer;
    [SerializeField] private Transform meaningContainer;
    [SerializeField] private Image doorImage;
    [SerializeField] private CanvasGroup doorCanvasGroup;
    [SerializeField] private Button checkButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Transform lineContainer;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip errorSound;

    private Dictionary<string, SymbolButton> symbolMap = new();
    private Dictionary<string, MeaningButton> meaningMap = new();
    private Dictionary<string, SymbolConnectionLine> connections = new();
    private Coroutine animationCoroutine;

    [System.Serializable]
    private class SymbolButton
    {
        public Button button;
        public Text text;
        public Image image;
        public LayoutElement layout;
    }

    [System.Serializable]
    private class MeaningButton
    {
        public Button button;
        public Text text;
        public Image image;
    }

    public void Setup(List<string> symbols, List<string> meanings, SymbolDoorController controller)
    {
        if (symbols == null || meanings == null || controller == null)
        {
            Debug.LogError("[SymbolDoorView] Invalid setup parameters!");
            return;
        }

        CreateSymbolButtons(symbols, controller);
        CreateMeaningButtons(meanings, controller);
        StyleDoor();
        StyleButtons();

        if (checkButton != null)
            checkButton.onClick.AddListener(() => controller.OnCheckClicked());

        if (resetButton != null)
            resetButton.onClick.AddListener(() => controller.OnResetClicked());
    }

    private void CreateSymbolButtons(List<string> symbols, SymbolDoorController controller)
    {
        ClearContainer(symbolContainer);
        symbolMap.Clear();

        foreach (string symbol in symbols)
        {
            GameObject btnObj = new GameObject(symbol);
            btnObj.transform.SetParent(symbolContainer, false);

            Button btn = btnObj.AddComponent<Button>();
            Image img = btnObj.AddComponent<Image>();
            img.color = SymbolDoorUIDesign.DarkBg;

            Text txt = new GameObject("Text").AddComponent<Text>();
            txt.transform.SetParent(btnObj.transform, false);
            txt.text = symbol;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.fontSize = 48;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = SymbolDoorUIDesign.SymbolColor;

            LayoutElement layout = btnObj.AddComponent<LayoutElement>();
            layout.preferredWidth = 90;
            layout.preferredHeight = 90;

            // Add border effect
            Outline outline = btnObj.AddComponent<Outline>();
            outline.effectColor = SymbolDoorUIDesign.BorderGold;
            outline.effectDistance = new Vector2(2, 2);

            string sym = symbol;
            btn.onClick.AddListener(() => controller.OnSymbolClicked(sym));

            symbolMap[symbol] = new SymbolButton { button = btn, text = txt, image = img, layout = layout };
        }
    }

    private void CreateMeaningButtons(List<string> meanings, SymbolDoorController controller)
    {
        ClearContainer(meaningContainer);
        meaningMap.Clear();

        foreach (string meaning in meanings)
        {
            GameObject btnObj = new GameObject(meaning);
            btnObj.transform.SetParent(meaningContainer, false);

            Button btn = btnObj.AddComponent<Button>();
            Image img = btnObj.AddComponent<Image>();
            img.color = SymbolDoorUIDesign.DarkBg;

            Text txt = new GameObject("Text").AddComponent<Text>();
            txt.transform.SetParent(btnObj.transform, false);
            txt.text = meaning;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.fontSize = 32;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = SymbolDoorUIDesign.SymbolColor;
            txt.horizontalOverflow = HorizontalWrapMode.Wrap;
            txt.verticalOverflow = VerticalWrapMode.Truncate;

            LayoutElement layout = btnObj.AddComponent<LayoutElement>();
            layout.preferredWidth = 110;
            layout.preferredHeight = 70;

            // Add border
            Outline outline = btnObj.AddComponent<Outline>();
            outline.effectColor = SymbolDoorUIDesign.BorderGold;
            outline.effectDistance = new Vector2(1, 1);

            string mean = meaning;
            btn.onClick.AddListener(() => controller.OnMeaningClicked(mean));

            meaningMap[meaning] = new MeaningButton { button = btn, text = txt, image = img };
        }
    }

    private void StyleDoor()
    {
        if (doorImage == null) return;

        doorImage.color = SymbolDoorUIDesign.DarkBg;

        Outline outline = doorImage.GetComponent<Outline>();
        if (outline == null)
            outline = doorImage.gameObject.AddComponent<Outline>();

        outline.effectColor = SymbolDoorUIDesign.AccentGold;
        outline.effectDistance = new Vector2(4, 4);
    }

    private void StyleButtons()
    {
        if (checkButton != null)
        {
            Image checkImg = checkButton.GetComponent<Image>();
            if (checkImg != null)
            {
                checkImg.color = SymbolDoorUIDesign.AccentGold;
            }

            Text checkTxt = checkButton.GetComponentInChildren<Text>();
            if (checkTxt == null)
            {
                GameObject txtObj = new GameObject("Text");
                txtObj.transform.SetParent(checkButton.transform, false);
                checkTxt = txtObj.AddComponent<Text>();
                checkTxt.text = "ПРОВЕРИТЬ";
                checkTxt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                checkTxt.alignment = TextAnchor.MiddleCenter;
                checkTxt.color = Color.black;
                checkTxt.fontStyle = FontStyle.Bold;
            }
        }

        if (resetButton != null)
        {
            Image resetImg = resetButton.GetComponent<Image>();
            if (resetImg != null)
            {
                resetImg.color = SymbolDoorUIDesign.AccentCyan;
            }

            Text resetTxt = resetButton.GetComponentInChildren<Text>();
            if (resetTxt == null)
            {
                GameObject txtObj = new GameObject("Text");
                txtObj.transform.SetParent(resetButton.transform, false);
                resetTxt = txtObj.AddComponent<Text>();
                resetTxt.text = "СБРОС";
                resetTxt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                resetTxt.alignment = TextAnchor.MiddleCenter;
                resetTxt.color = Color.black;
                resetTxt.fontStyle = FontStyle.Bold;
            }
        }
    }

    public void HighlightSymbol(string symbol, bool highlight)
    {
        if (!symbolMap.TryGetValue(symbol, out var btn))
            return;

        if (highlight)
        {
            StartAnimationCoroutine(AnimateSymbolSelect(btn));
        }
        else
        {
            btn.image.color = SymbolDoorUIDesign.DarkBg;
        }
    }

    public void ShowConnection(string symbol, string meaning)
    {
        if (!symbolMap.TryGetValue(symbol, out var symBtn) || !meaningMap.TryGetValue(meaning, out var meanBtn))
            return;

        string key = $"{symbol}→{meaning}";

        if (connections.ContainsKey(key))
            return;

        // Animate meaning button
        meanBtn.image.color = SymbolDoorUIDesign.AccentCyan;

        // Draw connection line
        if (lineContainer != null)
        {
            GameObject lineObj = new GameObject($"Line_{symbol}_{meaning}");
            lineObj.transform.SetParent(lineContainer, false);

            SymbolConnectionLine line = lineObj.AddComponent<SymbolConnectionLine>();

            RectTransform symRect = symBtn.button.GetComponent<RectTransform>();
            RectTransform meanRect = meanBtn.button.GetComponent<RectTransform>();

            if (symRect != null && meanRect != null)
            {
                Vector3 from = symRect.position;
                Vector3 to = meanRect.position;
                line.ConnectPoints(from, to, 0.3f);
            }

            connections[key] = line;
        }
    }

    public void HideConnection(string symbol, string meaning)
    {
        if (!meaningMap.TryGetValue(meaning, out var btn))
            return;

        string key = $"{symbol}→{meaning}";

        btn.image.color = SymbolDoorUIDesign.DarkBg;

        if (connections.TryGetValue(key, out var line))
        {
            line.Disconnect(0.2f);
            connections.Remove(key);
        }
    }

    public void ClearAllConnections()
    {
        foreach (var line in connections.Values)
        {
            if (line != null)
                Destroy(line.gameObject);
        }
        connections.Clear();

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

    private void ClearContainer(Transform container)
    {
        if (container == null) return;
        foreach (Transform child in container)
            Destroy(child.gameObject);
    }

    private void StartAnimationCoroutine(IEnumerator anim)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(anim);
    }

    private IEnumerator AnimateSymbolSelect(SymbolButton btn)
    {
        float elapsed = 0f;
        Color targetColor = SymbolDoorUIDesign.AccentGold;

        while (elapsed < SymbolDoorUIDesign.SelectAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / SymbolDoorUIDesign.SelectAnimDuration;
            t = SymbolDoorUIDesign.EaseOutQuad(t);
            btn.image.color = Color.Lerp(SymbolDoorUIDesign.DarkBg, targetColor, t);
            yield return null;
        }

        btn.image.color = targetColor;
    }

    private IEnumerator AnimateButtonError(Image img)
    {
        Color original = img.color;

        for (int i = 0; i < 3; i++)
        {
            float elapsed = 0f;
            while (elapsed < SymbolDoorUIDesign.ErrorFlashDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / SymbolDoorUIDesign.ErrorFlashDuration;
                img.color = Color.Lerp(original, SymbolDoorUIDesign.ErrorRed, t);
                yield return null;
            }

            elapsed = 0f;
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
            float elapsed = 0f;

            // Pulse animation
            while (elapsed < SymbolDoorUIDesign.SuccessPulseDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / SymbolDoorUIDesign.SuccessPulseDuration;
                t = SymbolDoorUIDesign.EaseOutQuad(t);

                doorCanvasGroup.alpha = Mathf.Lerp(1f, 0.5f, t * 0.5f);

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
            float elapsed = 0f;
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

            elapsed = 0f;
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
