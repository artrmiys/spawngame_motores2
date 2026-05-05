using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Motores2SceneSetup
{
    const string ScenesFolder = "Assets/Scenes";
    const string GeneratedFolder = "Assets/Generated";

    [MenuItem("Tools/Motores2 - Setup All Scenes")]
    static void SetupAll()
    {
        SetupSplash();
        SetupMainMenu();
        SetupLevel1();
        SetupLevel2();

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene($"{ScenesFolder}/SplashScreen.unity", true),
            new EditorBuildSettingsScene($"{ScenesFolder}/MainMenu.unity", true),
            new EditorBuildSettingsScene($"{ScenesFolder}/Level1.unity", true),
            new EditorBuildSettingsScene($"{ScenesFolder}/Level2.unity", true)
        };

        AssetDatabase.SaveAssets();
        Debug.Log("[Motores2] All portrait mobile scenes created and added to Build Settings.");
    }

    [MenuItem("Tools/Motores2 - Setup Splash Scene")]
    static void SetupSplash()
    {
        NewScene();

        var bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
        bg.name = "Background";
        bg.transform.localScale = new Vector3(12, 22, 1);
        SetColor(bg, new Color(0.05f, 0.05f, 0.1f));

        var canvas = MakeCanvas("SplashCanvas");
        var safeArea = MakeSafeArea(canvas);

        MakeText(safeArea, "TitleText", "SWARM SURVIVOR",
            new Vector2(0, 110), new Vector2(760, 130), 64, FontStyle.Bold, Color.white);

        MakeText(safeArea, "TeamText",
            "Menshchikov Artem\nShrom Nadezhda",
            new Vector2(0, -50), new Vector2(620, 110), 28, FontStyle.Normal,
            new Color(0.8f, 0.8f, 0.8f));

        Camera.main.backgroundColor = new Color(0.05f, 0.05f, 0.1f);
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 10.5f;

        var manager = new GameObject("SplashManager");
        manager.AddComponent<SplashScreen>();

        SaveScene("SplashScreen");
        Debug.Log("[Motores2] Splash scene created.");
    }

    [MenuItem("Tools/Motores2 - Setup MainMenu Scene")]
    static void SetupMainMenu()
    {
        NewScene();

        Camera.main.backgroundColor = new Color(0.05f, 0.05f, 0.1f);
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 10.5f;

        var canvas = MakeCanvas("MenuCanvas");
        var safeArea = MakeSafeArea(canvas);
        var menu = safeArea.AddComponent<MainMenuUI>();

        MakeText(safeArea, "TitleText", "SWARM SURVIVOR",
            new Vector2(0, 180), new Vector2(760, 130), 64, FontStyle.Bold, Color.white);

        MakeText(safeArea, "SubtitleText", "Portrait survival arena",
            new Vector2(0, 70), new Vector2(620, 60), 26, FontStyle.Normal,
            new Color(0.75f, 0.82f, 0.9f));

        var playButton = MakeButton(safeArea, "PlayButton", "PLAY",
            new Vector2(0, -70), new Vector2(260, 84)).GetComponent<Button>();
        UnityEventTools.AddPersistentListener(playButton.onClick, menu.PlayGame);

        MakeText(safeArea, "AuthorsText",
            "Menshchikov Artem  |  Shrom Nadezhda",
            new Vector2(0, -310), new Vector2(680, 44), 20, FontStyle.Normal,
            new Color(0.6f, 0.6f, 0.6f));

        SaveScene("MainMenu");
        Debug.Log("[Motores2] MainMenu scene created.");
    }

    [MenuItem("Tools/Motores2 - Setup Level 1 Scene")]
    static void SetupLevel1()
    {
        SetupLevel(
            sceneName: "Level1",
            nextSceneName: "Level2",
            levelTitle: "Level 1 - Garden Rush",
            cameraColor: new Color(0.08f, 0.12f, 0.09f),
            groundColor: new Color(0.14f, 0.23f, 0.15f),
            obstacleColor: new Color(0.22f, 0.36f, 0.24f),
            baseEnemies: 4,
            waveCount: 3,
            spawnRadius: 8f,
            hasObstacles: false);
    }

    [MenuItem("Tools/Motores2 - Setup Level 2 Scene")]
    static void SetupLevel2()
    {
        SetupLevel(
            sceneName: "Level2",
            nextSceneName: "",
            levelTitle: "Level 2 - Night Swarm",
            cameraColor: new Color(0.055f, 0.045f, 0.085f),
            groundColor: new Color(0.11f, 0.10f, 0.16f),
            obstacleColor: new Color(0.25f, 0.19f, 0.34f),
            baseEnemies: 6,
            waveCount: 5,
            spawnRadius: 9.5f,
            hasObstacles: true);
    }

    static void SetupLevel(
        string sceneName,
        string nextSceneName,
        string levelTitle,
        Color cameraColor,
        Color groundColor,
        Color obstacleColor,
        int baseEnemies,
        int waveCount,
        float spawnRadius,
        bool hasObstacles)
    {
        NewScene();
        EnsureTag("Enemy");
        EnsureGeneratedFolder();

        Camera.main.backgroundColor = cameraColor;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 10.5f;
        Camera.main.transform.position = new Vector3(0, 0, -10);

        var ground = GameObject.CreatePrimitive(PrimitiveType.Quad);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(12, 24, 1);
        ground.transform.position = Vector3.zero;
        SetColor(ground, groundColor);

        CreateWall("WallTop",    new Vector2(0, 12),  new Vector2(12, 1));
        CreateWall("WallBottom", new Vector2(0, -12), new Vector2(12, 1));
        CreateWall("WallLeft",   new Vector2(-6, 0),  new Vector2(1, 24));
        CreateWall("WallRight",  new Vector2(6, 0),   new Vector2(1, 24));

        if (hasObstacles)
        {
            CreateObstacle("BlockTopLeft", new Vector2(-2.8f, 4.6f), new Vector2(1.4f, 3.2f), obstacleColor);
            CreateObstacle("BlockCenterRight", new Vector2(2.7f, 0.8f), new Vector2(1.5f, 3.4f), obstacleColor);
            CreateObstacle("BlockBottomLeft", new Vector2(-2.2f, -5.3f), new Vector2(1.2f, 2.8f), obstacleColor);
        }

        var projectilePrefab = CreateProjectilePrefab();
        var enemyPrefab = CreateEnemyPrefab();
        var healPrefab = CreatePowerUpPrefab("HealPowerUpPrefab", PowerUpType.Heal, new Color(0.2f, 1f, 0.35f));
        var speedPrefab = CreatePowerUpPrefab("SpeedPowerUpPrefab", PowerUpType.SpeedBoost, new Color(0.25f, 0.8f, 1f));

        var remoteConfig = new GameObject("RemoteConfigManager");
        remoteConfig.AddComponent<RemoteConfigManager>();

        var player = CreatePlayer(projectilePrefab, out var playerController);

        var waveManagerObject = new GameObject("WaveManager");
        var waveManager = waveManagerObject.AddComponent<WaveManager>();
        SetObject(waveManager, "enemyPrefab", enemyPrefab);
        SetFloat(waveManager, "spawnRadius", spawnRadius);
        SetInt(waveManager, "baseEnemies", baseEnemies);
        SetInt(waveManager, "levelWaveCount", waveCount);
        SetString(waveManager, "levelTitle", levelTitle);

        var gameManagerObject = new GameObject("GameManager");
        var gameManager = gameManagerObject.AddComponent<GameManager>();
        SetString(gameManager, "nextSceneName", nextSceneName);

        var powerUpSpawnerObject = new GameObject("PowerUpSpawner");
        var powerUpSpawner = powerUpSpawnerObject.AddComponent<PowerUpSpawner>();
        SetObject(powerUpSpawner, "healPrefab", healPrefab);
        SetObject(powerUpSpawner, "speedPrefab", speedPrefab);

        var canvas = MakeCanvas("GameCanvas");
        var safeArea = MakeSafeArea(canvas);
        var hpBar = MakeHpBar(safeArea);
        var waveText = MakeText(safeArea, "WaveText", "Wave 0 / " + waveCount,
            new Vector2(0, -34), new Vector2(360, 54), 28, FontStyle.Bold, Color.white,
            anchorMin: new Vector2(0.5f, 1), anchorMax: new Vector2(0.5f, 1), pivot: new Vector2(0.5f, 1));
        var messageText = MakeText(safeArea, "MessageText", "",
            Vector2.zero, new Vector2(560, 76), 36, FontStyle.Bold, Color.yellow);

        var winPanel = MakePanel(safeArea, "WinScreen", new Color(0, 0.5f, 0, 0.85f), "YOU WIN!");
        var losePanel = MakePanel(safeArea, "LoseScreen", new Color(0.5f, 0, 0, 0.85f), "GAME OVER");
        var restartButton = MakeButton(losePanel, "RestartButton", "RESTART", new Vector2(0, -60), new Vector2(220, 60)).GetComponent<Button>();
        var menuButton = MakeButton(winPanel, "MenuButton", "MAIN MENU", new Vector2(0, -60), new Vector2(240, 60)).GetComponent<Button>();
        UnityEventTools.AddPersistentListener(restartButton.onClick, gameManager.RestartGame);
        UnityEventTools.AddPersistentListener(menuButton.onClick, gameManager.GoToMenu);
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        var uiManager = safeArea.AddComponent<UIManager>();
        SetObject(uiManager, "hpBar", hpBar);
        SetObject(uiManager, "waveText", waveText);
        SetObject(uiManager, "messageText", messageText);
        SetObject(uiManager, "winScreen", winPanel);
        SetObject(uiManager, "loseScreen", losePanel);

        var joystick = MakeJoystick(safeArea);
        SetObject(playerController, "joystick", joystick);

        SaveScene(sceneName);
        Debug.Log($"[Motores2] {sceneName} created for portrait mobile.");
    }

    static GameObject CreatePlayer(GameObject projectilePrefab, out PlayerController playerController)
    {
        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = Vector3.zero;
        player.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        SetColor(player, new Color(0.2f, 0.6f, 1f));
        Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());

        var playerCollider = player.AddComponent<CircleCollider2D>();
        playerCollider.radius = 0.5f;
        var playerRb = player.AddComponent<Rigidbody2D>();
        playerRb.gravityScale = 0;
        playerRb.freezeRotation = true;

        player.AddComponent<PlayerHealth>();
        playerController = player.AddComponent<PlayerController>();
        var autoAttack = player.AddComponent<AutoAttack>();

        var firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(player.transform, false);
        firePoint.transform.localPosition = new Vector3(0, 0.55f, 0);
        SetObject(autoAttack, "projectilePrefab", projectilePrefab);
        SetObject(autoAttack, "firePoint", firePoint.transform);

        return player;
    }

    static GameObject CreateProjectilePrefab()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "ProjectilePrefab";
        go.transform.localScale = Vector3.one * 0.15f;
        SetColor(go, Color.yellow);
        Object.DestroyImmediate(go.GetComponent<SphereCollider>());
        var collider = go.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        go.AddComponent<Projectile>();
        return SaveAsGeneratedPrefab(go, "ProjectilePrefab");
    }

    static GameObject CreateEnemyPrefab()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.name = "EnemyPrefab";
        go.tag = "Enemy";
        go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        SetColor(go, new Color(1f, 0.2f, 0.2f));
        Object.DestroyImmediate(go.GetComponent<CapsuleCollider>());
        var collider = go.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;
        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        go.AddComponent<EnemyHealth>();
        go.AddComponent<EnemyAI>();
        return SaveAsGeneratedPrefab(go, "EnemyPrefab");
    }

    static GameObject CreatePowerUpPrefab(string name, PowerUpType type, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.localScale = Vector3.one * 0.35f;
        SetColor(go, color);
        Object.DestroyImmediate(go.GetComponent<SphereCollider>());
        var collider = go.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        var powerUp = go.AddComponent<PowerUp>();
        SetEnum(powerUp, "type", (int)type);
        return SaveAsGeneratedPrefab(go, name);
    }

    static GameObject SaveAsGeneratedPrefab(GameObject template, string name)
    {
        EnsureGeneratedFolder();
        var path = $"{GeneratedFolder}/{name}.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(template, path);
        Object.DestroyImmediate(template);
        AssetDatabase.SaveAssets();
        return prefab;
    }

    static GameObject MakeSafeArea(GameObject canvas)
    {
        var safeArea = new GameObject("SafeArea");
        safeArea.transform.SetParent(canvas.transform, false);
        var rect = safeArea.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        safeArea.AddComponent<SafeAreaFitter>();
        return safeArea;
    }

    static VirtualJoystick MakeJoystick(GameObject parent)
    {
        var joystickBg = new GameObject("JoystickBG");
        joystickBg.transform.SetParent(parent.transform, false);
        var bgRect = joystickBg.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(0, 0);
        bgRect.pivot = new Vector2(0, 0);
        bgRect.anchoredPosition = new Vector2(52, 52);
        bgRect.sizeDelta = new Vector2(160, 160);
        var bgImage = joystickBg.AddComponent<Image>();
        bgImage.color = new Color(1, 1, 1, 0.15f);
        var joystick = joystickBg.AddComponent<VirtualJoystick>();

        var handle = new GameObject("Handle");
        handle.transform.SetParent(joystickBg.transform, false);
        var handleRect = handle.AddComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0.5f, 0.5f);
        handleRect.anchorMax = new Vector2(0.5f, 0.5f);
        handleRect.pivot = new Vector2(0.5f, 0.5f);
        handleRect.sizeDelta = new Vector2(64, 64);
        var handleImage = handle.AddComponent<Image>();
        handleImage.color = new Color(1, 1, 1, 0.5f);

        SetObject(joystick, "background", bgRect);
        SetObject(joystick, "handle", handleRect);
        return joystick;
    }

    static Slider MakeHpBar(GameObject parent)
    {
        var root = new GameObject("HPBar");
        root.transform.SetParent(parent.transform, false);
        var rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(22, -26);
        rect.sizeDelta = new Vector2(230, 30);
        var bg = root.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.05f, 0.75f);

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(root.transform, false);
        var fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(3, 3);
        fillAreaRect.offsetMax = new Vector2(-3, -3);

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        var fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.9f, 0.35f);

        var slider = root.AddComponent<Slider>();
        slider.transition = Selectable.Transition.None;
        slider.minValue = 0;
        slider.maxValue = 5;
        slider.value = 5;
        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;
        return slider;
    }

    static void NewScene()
    {
        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
    }

    static void SaveScene(string name)
    {
        EnsureScenesFolder();
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), $"{ScenesFolder}/{name}.unity");
    }

    static GameObject MakeCanvas(string name)
    {
        var go = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();

        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        return go;
    }

    static TextMeshProUGUI MakeText(GameObject parent, string name, string content,
        Vector2 anchoredPos, Vector2 size, float fontSize, FontStyle style, Color color,
        Vector2? anchorMin = null, Vector2? anchorMax = null, Vector2? pivot = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin ?? new Vector2(0.5f, 0.5f);
        rect.anchorMax = anchorMax ?? new Vector2(0.5f, 0.5f);
        rect.pivot = pivot ?? new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;
        var text = go.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = style == FontStyle.Bold ? FontStyles.Bold : FontStyles.Normal;
        return text;
    }

    static GameObject MakeButton(GameObject parent, string name, string label,
        Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;
        var image = go.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.8f);
        var button = go.AddComponent<Button>();
        button.targetGraphic = image;

        var labelObject = new GameObject("Label");
        labelObject.transform.SetParent(go.transform, false);
        var labelRect = labelObject.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.sizeDelta = Vector2.zero;
        var text = labelObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 28;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        return go;
    }

    static GameObject MakePanel(GameObject parent, string name, Color bgColor, string labelText)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(430, 300);
        var image = go.AddComponent<Image>();
        image.color = bgColor;
        MakeText(go, "Label", labelText, new Vector2(0, 60), new Vector2(390, 80),
            48, FontStyle.Bold, Color.white);
        return go;
    }

    static void CreateWall(string name, Vector2 position, Vector2 size)
    {
        var wall = new GameObject(name);
        wall.transform.position = position;
        var collider = wall.AddComponent<BoxCollider2D>();
        collider.size = size;
    }

    static void CreateObstacle(string name, Vector2 position, Vector2 size, Color color)
    {
        var obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacle.name = name;
        obstacle.transform.position = position;
        obstacle.transform.localScale = new Vector3(size.x, size.y, 0.3f);
        SetColor(obstacle, color);
        Object.DestroyImmediate(obstacle.GetComponent<BoxCollider>());
        obstacle.AddComponent<BoxCollider2D>().size = Vector2.one;
    }

    static void SetColor(GameObject go, Color color)
    {
        var renderer = go.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = color;
    }

    static void SetObject(Object target, string fieldName, Object value)
    {
        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty(fieldName);
        if (property == null)
        {
            Debug.LogWarning($"[Motores2] Missing serialized field '{fieldName}' on {target.name}.");
            return;
        }

        property.objectReferenceValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    static void SetInt(Object target, string fieldName, int value)
    {
        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty(fieldName);
        if (property == null)
        {
            Debug.LogWarning($"[Motores2] Missing serialized int '{fieldName}' on {target.name}.");
            return;
        }

        property.intValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    static void SetFloat(Object target, string fieldName, float value)
    {
        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty(fieldName);
        if (property == null)
        {
            Debug.LogWarning($"[Motores2] Missing serialized float '{fieldName}' on {target.name}.");
            return;
        }

        property.floatValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    static void SetString(Object target, string fieldName, string value)
    {
        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty(fieldName);
        if (property == null)
        {
            Debug.LogWarning($"[Motores2] Missing serialized string '{fieldName}' on {target.name}.");
            return;
        }

        property.stringValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    static void SetEnum(Object target, string fieldName, int value)
    {
        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty(fieldName);
        if (property == null)
        {
            Debug.LogWarning($"[Motores2] Missing serialized enum '{fieldName}' on {target.name}.");
            return;
        }

        property.enumValueIndex = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    static void EnsureScenesFolder()
    {
        if (!AssetDatabase.IsValidFolder(ScenesFolder))
            AssetDatabase.CreateFolder("Assets", "Scenes");
    }

    static void EnsureGeneratedFolder()
    {
        if (!AssetDatabase.IsValidFolder(GeneratedFolder))
            AssetDatabase.CreateFolder("Assets", "Generated");
    }

    static void EnsureTag(string tag)
    {
        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var tags = tagManager.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++)
        {
            if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                return;
        }

        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedPropertiesWithoutUndo();
    }
}
