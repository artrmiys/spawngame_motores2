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

        Camera.main.backgroundColor = new Color(0.04f, 0.04f, 0.09f);
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 9.5f;

        var canvas = MakeCanvas("SplashCanvas");
        var safeArea = MakeSafeArea(canvas);

        // Gradient background
        MakeGradientBackground(safeArea,
            new Color(0.08f, 0.10f, 0.20f),
            new Color(0.02f, 0.02f, 0.06f));

        // Logo
        var title = MakeText(safeArea, "TitleText", "SWARM\nSURVIVOR",
            new Vector2(0, 100), new Vector2(900, 280), 92, FontStyle.Bold, Color.white);
        title.color = new Color(1f, 0.95f, 0.85f);
        title.fontStyle = FontStyles.Bold | FontStyles.UpperCase;
        title.outlineColor = new Color(0.95f, 0.78f, 0.25f, 1f);
        title.outlineWidth = 0.25f;
        title.characterSpacing = 8f;
        AddFadeIn(title.gameObject, 0.1f);

        // Decorative bar
        MakeAccentBar(safeArea, new Vector2(0, -80), new Vector2(380, 3),
            new Color(0.95f, 0.78f, 0.25f));

        var team = MakeText(safeArea, "TeamText",
            "Menshchikov Artem\nShrom Nadezhda",
            new Vector2(0, -160), new Vector2(620, 110), 28, FontStyle.Normal,
            new Color(0.8f, 0.85f, 0.95f));
        team.characterSpacing = 4f;
        AddFadeIn(team.gameObject, 0.5f);

        var manager = new GameObject("SplashManager");
        manager.AddComponent<SplashScreen>();

        SaveScene("SplashScreen");
        Debug.Log("[Motores2] Splash scene created with polished UI.");
    }

    [MenuItem("Tools/Motores2 - Setup MainMenu Scene")]
    static void SetupMainMenu()
    {
        NewScene();

        Camera.main.backgroundColor = new Color(0.04f, 0.04f, 0.09f);
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 10.5f;

        var canvas = MakeCanvas("MenuCanvas");
        var safeArea = MakeSafeArea(canvas);
        var menu = safeArea.AddComponent<MainMenuUI>();

        // Full-screen gradient background
        MakeGradientBackground(safeArea,
            new Color(0.08f, 0.10f, 0.20f),  // top: deep blue
            new Color(0.02f, 0.02f, 0.06f)); // bottom: near black

        // Decorative top accent bar (gold)
        MakeAccentBar(safeArea, new Vector2(0, 350), new Vector2(580, 4),
            new Color(0.95f, 0.78f, 0.25f));

        // Title with shadow + outline
        var title = MakeText(safeArea, "TitleText", "SWARM\nSURVIVOR",
            new Vector2(0, 220), new Vector2(900, 280), 92, FontStyle.Bold, Color.white);
        title.color = new Color(1f, 0.95f, 0.85f);
        title.fontStyle = FontStyles.Bold | FontStyles.UpperCase;
        title.outlineColor = new Color(0.95f, 0.78f, 0.25f, 1f);
        title.outlineWidth = 0.25f;
        title.characterSpacing = 8f;
        AddFadeIn(title.gameObject, 0.0f);

        // Subtitle
        var subtitle = MakeText(safeArea, "SubtitleText", "MOBILE  •  PORTRAIT  •  ARENA",
            new Vector2(0, 30), new Vector2(620, 40), 22, FontStyle.Normal,
            new Color(0.65f, 0.72f, 0.85f, 1f));
        subtitle.characterSpacing = 12f;
        AddFadeIn(subtitle.gameObject, 0.2f);

        // Decorative bottom accent bar
        MakeAccentBar(safeArea, new Vector2(0, -10), new Vector2(280, 2),
            new Color(0.95f, 0.78f, 0.25f, 0.5f));

        // Play button (large, centered)
        var playGo = MakeStyledButton(safeArea, "PlayButton", "PLAY",
            new Vector2(0, -130), new Vector2(380, 110),
            new Color(0.95f, 0.78f, 0.25f),    // gold
            new Color(0.78f, 0.6f, 0.12f),     // darker gold
            Color.black, fontSize: 42, pulse: true);
        var playButton = playGo.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(playButton.onClick, menu.PlayGame);
        AddFadeIn(playGo, 0.4f);

        // Authors footer
        var authors = MakeText(safeArea, "AuthorsText",
            "Menshchikov Artem  |  Shrom Nadezhda",
            new Vector2(0, -380), new Vector2(680, 44), 18, FontStyle.Normal,
            new Color(0.5f, 0.5f, 0.6f));
        authors.characterSpacing = 4f;
        AddFadeIn(authors.gameObject, 0.6f);

        SaveScene("MainMenu");
        Debug.Log("[Motores2] MainMenu scene created with polished UI.");
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
        ground.transform.localScale = new Vector3(10, 18, 1);
        ground.transform.position = Vector3.zero;
        SetColor(ground, groundColor);

        CreateWall("WallTop",    new Vector2(0, 9.15f),  new Vector2(10, 0.8f));
        CreateWall("WallBottom", new Vector2(0, -9.15f), new Vector2(10, 0.8f));
        CreateWall("WallLeft",   new Vector2(-5.15f, 0), new Vector2(0.8f, 18));
        CreateWall("WallRight",  new Vector2(5.15f, 0),  new Vector2(0.8f, 18));

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
        var rapidFirePrefab = CreatePowerUpPrefab("RapidFirePowerUpPrefab", PowerUpType.RapidFire, new Color(1f, 0.85f, 0.2f));
        var spreadShotPrefab = CreatePowerUpPrefab("SpreadShotPowerUpPrefab", PowerUpType.SpreadShot, new Color(0.95f, 0.35f, 1f));
        var blastShotPrefab = CreatePowerUpPrefab("BlastShotPowerUpPrefab", PowerUpType.BlastShot, new Color(1f, 0.45f, 0.1f));
        var powerSnackPrefab = CreatePowerUpPrefab("PowerSnackPrefab", PowerUpType.PowerSnack, new Color(1f, 0.55f, 0.12f));

        var remoteConfig = new GameObject("RemoteConfigManager");
        var remoteConfigManager = remoteConfig.AddComponent<RemoteConfigManager>();
        SetFloat(remoteConfigManager, "spawnInterval", 0.76f);
        SetFloat(remoteConfigManager, "enemySpeed", 3.35f);

        var player = CreatePlayer(projectilePrefab, out var playerController);

        var waveManagerObject = new GameObject("WaveManager");
        var waveManager = waveManagerObject.AddComponent<WaveManager>();
        SetObject(waveManager, "enemyPrefab", enemyPrefab);
        SetFloat(waveManager, "spawnRadius", spawnRadius);
        SetVector2(waveManager, "playAreaHalfExtents", new Vector2(4.35f, 8.35f));
        SetFloat(waveManager, "maxSpawnInterval", 0.76f);
        SetFloat(waveManager, "firstWaveDelay", 0.35f);
        SetFloat(waveManager, "waveEndDelay", 0.75f);
        SetInt(waveManager, "baseEnemies", baseEnemies);
        SetInt(waveManager, "levelWaveCount", waveCount);
        SetString(waveManager, "levelTitle", levelTitle);

        var gameManagerObject = new GameObject("GameManager");
        var gameManager = gameManagerObject.AddComponent<GameManager>();
        SetString(gameManager, "nextSceneName", nextSceneName);

        new GameObject("SymbolDoorEventManager").AddComponent<SymbolDoorEventManager>();
        new GameObject("SymbolDoor").AddComponent<SymbolDoor>();

        var powerUpSpawnerObject = new GameObject("PowerUpSpawner");
        var powerUpSpawner = powerUpSpawnerObject.AddComponent<PowerUpSpawner>();
        SetObject(powerUpSpawner, "healPrefab", healPrefab);
        SetObject(powerUpSpawner, "speedPrefab", speedPrefab);
        SetObject(powerUpSpawner, "rapidFirePrefab", rapidFirePrefab);
        SetObject(powerUpSpawner, "spreadShotPrefab", spreadShotPrefab);
        SetObject(powerUpSpawner, "blastShotPrefab", blastShotPrefab);
        SetObject(powerUpSpawner, "powerSnackPrefab", powerSnackPrefab);
        SetFloat(powerUpSpawner, "spawnInterval", 8f);
        SetVector2(powerUpSpawner, "spawnHalfExtents", new Vector2(4.1f, 8.1f));

        // Add ScreenShake to camera
        if (Camera.main.gameObject.GetComponent<ScreenShake>() == null)
            Camera.main.gameObject.AddComponent<ScreenShake>();

        var canvas = MakeCanvas("GameCanvas");
        var safeArea = MakeSafeArea(canvas);

        // Top bar with subtle dark background
        MakeTopBar(safeArea);

        var hpBar = MakeStyledHpBar(safeArea);
        var waveText = MakeText(safeArea, "WaveText", "Wave 0 / " + waveCount,
            new Vector2(0, -36), new Vector2(360, 54), 32, FontStyle.Bold, Color.white,
            anchorMin: new Vector2(0.5f, 1), anchorMax: new Vector2(0.5f, 1), pivot: new Vector2(0.5f, 1));
        waveText.outlineColor = Color.black;
        waveText.outlineWidth = 0.2f;
        waveText.characterSpacing = 2f;

        var weaponText = MakeText(safeArea, "WeaponText", "Weapon: Basic",
            new Vector2(-28, -32), new Vector2(320, 44), 22, FontStyle.Bold,
            new Color(1f, 0.9f, 0.45f),
            anchorMin: new Vector2(1, 1), anchorMax: new Vector2(1, 1), pivot: new Vector2(1, 1));
        weaponText.alignment = TextAlignmentOptions.Right;
        weaponText.outlineColor = Color.black;
        weaponText.outlineWidth = 0.2f;

        var messageText = MakeText(safeArea, "MessageText", "",
            Vector2.zero, new Vector2(720, 100), 48, FontStyle.Bold,
            new Color(1f, 0.9f, 0.4f));
        messageText.outlineColor = Color.black;
        messageText.outlineWidth = 0.3f;
        messageText.fontStyle = FontStyles.Bold | FontStyles.UpperCase;
        messageText.characterSpacing = 6f;

        var winPanel = MakeStyledPanel(safeArea, "WinScreen",
            new Color(0.1f, 0.4f, 0.15f, 0.95f),
            new Color(0.05f, 0.2f, 0.08f, 0.95f),
            "YOU WIN!", new Color(0.4f, 1f, 0.5f));
        var losePanel = MakeStyledPanel(safeArea, "LoseScreen",
            new Color(0.4f, 0.1f, 0.1f, 0.95f),
            new Color(0.2f, 0.05f, 0.05f, 0.95f),
            "GAME OVER", new Color(1f, 0.4f, 0.4f));

        var restartButton = MakeStyledButton(losePanel, "RestartButton", "RESTART",
            new Vector2(0, -80), new Vector2(280, 80),
            new Color(0.95f, 0.78f, 0.25f), new Color(0.78f, 0.6f, 0.12f),
            Color.black, fontSize: 32).GetComponent<Button>();
        var menuButton = MakeStyledButton(winPanel, "MenuButton", "MAIN MENU",
            new Vector2(0, -80), new Vector2(280, 80),
            new Color(0.95f, 0.78f, 0.25f), new Color(0.78f, 0.6f, 0.12f),
            Color.black, fontSize: 30).GetComponent<Button>();
        UnityEventTools.AddPersistentListener(restartButton.onClick, gameManager.RestartGame);
        UnityEventTools.AddPersistentListener(menuButton.onClick, gameManager.GoToMenu);
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        var uiManager = safeArea.AddComponent<UIManager>();
        SetObject(uiManager, "hpBar", hpBar);
        SetObject(uiManager, "waveText", waveText);
        SetObject(uiManager, "weaponText", weaponText);
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
        player.AddComponent<HitFlash>();
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
        go.AddComponent<HitFlash>();
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
        go.AddComponent<PowerUpFoodVisual>();
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
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        scaler.referencePixelsPerUnit = 100f;
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

    static void SetVector2(Object target, string fieldName, Vector2 value)
    {
        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty(fieldName);
        if (property == null)
        {
            Debug.LogWarning($"[Motores2] Missing serialized Vector2 '{fieldName}' on {target.name}.");
            return;
        }

        property.vector2Value = value;
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

    // ============================================================
    // VISUAL POLISH HELPERS - added in design refactor
    // ============================================================

    static GameObject MakeGradientBackground(GameObject parent, Color top, Color bottom)
    {
        var go = new GameObject("GradientBG");
        go.transform.SetParent(parent.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        var img = go.AddComponent<Image>();
        img.color = Color.white;
        var grad = go.AddComponent<UIGradient>();
        var gradSO = new SerializedObject(grad);
        gradSO.FindProperty("topColor").colorValue = top;
        gradSO.FindProperty("bottomColor").colorValue = bottom;
        gradSO.ApplyModifiedPropertiesWithoutUndo();
        go.transform.SetAsFirstSibling();
        return go;
    }

    static GameObject MakeAccentBar(GameObject parent, Vector2 pos, Vector2 size, Color color)
    {
        var go = new GameObject("AccentBar");
        go.transform.SetParent(parent.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;
        var img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    static GameObject MakeStyledButton(GameObject parent, string name, string label,
        Vector2 pos, Vector2 size, Color colorTop, Color colorBottom, Color textColor,
        float fontSize = 28, bool pulse = false)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;

        var image = go.AddComponent<Image>();
        image.color = Color.white;

        // Gradient
        var grad = go.AddComponent<UIGradient>();
        var gradSO = new SerializedObject(grad);
        gradSO.FindProperty("topColor").colorValue = colorTop;
        gradSO.FindProperty("bottomColor").colorValue = colorBottom;
        gradSO.ApplyModifiedPropertiesWithoutUndo();

        // Shadow
        var shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.6f);
        shadow.effectDistance = new Vector2(4, -4);

        var button = go.AddComponent<Button>();
        button.targetGraphic = image;

        // Animated button behavior
        var anim = go.AddComponent<UIAnimatedButton>();
        if (pulse)
        {
            var animSO = new SerializedObject(anim);
            animSO.FindProperty("pulseWhenIdle").boolValue = true;
            animSO.ApplyModifiedPropertiesWithoutUndo();
        }

        // Label
        var labelObject = new GameObject("Label");
        labelObject.transform.SetParent(go.transform, false);
        var labelRect = labelObject.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.sizeDelta = Vector2.zero;
        var text = labelObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = fontSize;
        text.color = textColor;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold | FontStyles.UpperCase;
        text.characterSpacing = 4f;

        return go;
    }

    static void AddFadeIn(GameObject target, float delay)
    {
        var canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = target.AddComponent<CanvasGroup>();
        var fade = target.AddComponent<UIFadeIn>();
        var so = new SerializedObject(fade);
        so.FindProperty("delay").floatValue = delay;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static GameObject MakeTopBar(GameObject parent)
    {
        var go = new GameObject("TopBar");
        go.transform.SetParent(parent.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector2(0, 0);
        rect.sizeDelta = new Vector2(0, 100);
        var img = go.AddComponent<Image>();
        img.color = Color.white;
        var grad = go.AddComponent<UIGradient>();
        var gradSO = new SerializedObject(grad);
        gradSO.FindProperty("topColor").colorValue = new Color(0, 0, 0, 0.6f);
        gradSO.FindProperty("bottomColor").colorValue = new Color(0, 0, 0, 0f);
        gradSO.ApplyModifiedPropertiesWithoutUndo();
        return go;
    }

    static Slider MakeStyledHpBar(GameObject parent)
    {
        var root = new GameObject("HPBar");
        root.transform.SetParent(parent.transform, false);
        var rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(28, -32);
        rect.sizeDelta = new Vector2(280, 36);

        // Outer dark frame
        var bg = root.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.08f, 0.85f);

        var outline = root.AddComponent<Outline>();
        outline.effectColor = new Color(0.95f, 0.78f, 0.25f, 0.8f);
        outline.effectDistance = new Vector2(2, 2);

        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(root.transform, false);
        var fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(4, 4);
        fillAreaRect.offsetMax = new Vector2(-4, -4);

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        var fillImage = fill.AddComponent<Image>();
        fillImage.color = Color.white;

        // HP gradient (green→light green)
        var hpGrad = fill.AddComponent<UIGradient>();
        var hpGradSO = new SerializedObject(hpGrad);
        hpGradSO.FindProperty("topColor").colorValue = new Color(0.4f, 1f, 0.5f);
        hpGradSO.FindProperty("bottomColor").colorValue = new Color(0.15f, 0.7f, 0.3f);
        hpGradSO.ApplyModifiedPropertiesWithoutUndo();

        // HP label
        var hpLabel = new GameObject("HPLabel");
        hpLabel.transform.SetParent(root.transform, false);
        var hpLabelRect = hpLabel.AddComponent<RectTransform>();
        hpLabelRect.anchorMin = Vector2.zero;
        hpLabelRect.anchorMax = Vector2.one;
        hpLabelRect.offsetMin = Vector2.zero;
        hpLabelRect.offsetMax = Vector2.zero;
        var hpText = hpLabel.AddComponent<TextMeshProUGUI>();
        hpText.text = "HP";
        hpText.fontSize = 18;
        hpText.color = Color.white;
        hpText.alignment = TextAlignmentOptions.Center;
        hpText.fontStyle = FontStyles.Bold;
        hpText.outlineColor = Color.black;
        hpText.outlineWidth = 0.2f;

        var slider = root.AddComponent<Slider>();
        slider.transition = Selectable.Transition.None;
        slider.minValue = 0;
        slider.maxValue = 5;
        slider.value = 5;
        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;
        return slider;
    }

    static GameObject MakeStyledPanel(GameObject parent, string name, Color topColor, Color bottomColor,
        string title, Color titleColor)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(560, 420);

        var image = go.AddComponent<Image>();
        image.color = Color.white;

        var grad = go.AddComponent<UIGradient>();
        var gradSO = new SerializedObject(grad);
        gradSO.FindProperty("topColor").colorValue = topColor;
        gradSO.FindProperty("bottomColor").colorValue = bottomColor;
        gradSO.ApplyModifiedPropertiesWithoutUndo();

        var outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(0.95f, 0.78f, 0.25f, 0.8f);
        outline.effectDistance = new Vector2(3, 3);

        var shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.7f);
        shadow.effectDistance = new Vector2(0, -8);

        // Title
        var titleText = MakeText(go, "Label", title,
            new Vector2(0, 90), new Vector2(490, 110),
            64, FontStyle.Bold, titleColor);
        titleText.fontStyle = FontStyles.Bold | FontStyles.UpperCase;
        titleText.outlineColor = Color.black;
        titleText.outlineWidth = 0.3f;
        titleText.characterSpacing = 6f;

        return go;
    }
}
