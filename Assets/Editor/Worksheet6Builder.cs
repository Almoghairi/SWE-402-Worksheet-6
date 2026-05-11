using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Worksheet6Builder
{
    public static void BuildProject()
    {
        CreateFolders();
        EnsureTag("Enemy");
        EnsureTag("Powerup");
        ConfigureAudio();
        CreateMaterials();
        CreatePrefabs();
        CreateScene();
        ConfigurePlayerSettings();
        WriteReadme();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreateFolders()
    {
        foreach (string path in new[] {"Assets/Scenes", "Assets/Scripts", "Assets/Prefabs", "Assets/Materials", "Assets/Physics Materials", "Assets/Audio"})
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
                string name = System.IO.Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, name);
            }
        }
    }

    private static void EnsureTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tags = tagManager.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++)
        {
            if (tags.GetArrayElementAtIndex(i).stringValue == tag)
            {
                return;
            }
        }
        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }

    private static Material MakeMaterial(string name, Color color)
    {
        Material material = new Material(Shader.Find("Standard"));
        material.color = color;
        AssetDatabase.CreateAsset(material, $"Assets/Materials/{name}.mat");
        return material;
    }

    private static void CreateMaterials()
    {
        MakeMaterial("Arena_Material", new Color(0.1f, 0.23f, 0.32f));
        MakeMaterial("Player_Material", new Color(0.1f, 0.45f, 1f));
        MakeMaterial("Enemy_Material", new Color(0.95f, 0.16f, 0.12f));
        MakeMaterial("Powerup_Material", new Color(1f, 0.75f, 0.1f));
        MakeMaterial("Indicator_Material", new Color(0.1f, 1f, 0.8f));
    }

    private static void ConfigureAudio()
    {
        AssetDatabase.ImportAsset("Assets/Audio/Powerup.wav");
        AssetDatabase.ImportAsset("Assets/Audio/Hit.wav");
    }

    private static void CreatePrefabs()
    {
        PhysicsMaterial bouncy = new PhysicsMaterial("Bouncy_Multiply");
        bouncy.bounciness = 1.1f;
        bouncy.bounceCombine = PhysicsMaterialCombine.Multiply;
        AssetDatabase.CreateAsset(bouncy, "Assets/Physics Materials/Bouncy_Multiply.physicsMaterial");

        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        enemy.name = "Enemy";
        enemy.tag = "Enemy";
        enemy.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Enemy_Material.mat");
        enemy.GetComponent<Collider>().material = bouncy;
        enemy.AddComponent<Rigidbody>();
        enemy.AddComponent<Enemy>();
        PrefabUtility.SaveAsPrefabAsset(enemy, "Assets/Prefabs/Enemy.prefab");
        Object.DestroyImmediate(enemy);

        GameObject powerup = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        powerup.name = "Powerup";
        powerup.tag = "Powerup";
        powerup.transform.localScale = new Vector3(0.7f, 0.2f, 0.7f);
        powerup.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Powerup_Material.mat");
        powerup.GetComponent<Collider>().isTrigger = true;
        PrefabUtility.SaveAsPrefabAsset(powerup, "Assets/Prefabs/Powerup.prefab");
        Object.DestroyImmediate(powerup);
    }

    private static TextMeshProUGUI AddTMP(Transform parent, string name, string text, float size, TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 sizeDelta)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI label = obj.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = size;
        label.alignment = alignment;
        label.color = Color.white;
        RectTransform rect = label.rectTransform;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = sizeDelta;
        return label;
    }

    private static Button AddButton(Transform parent, string name, string label, Vector2 position)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Image image = obj.AddComponent<Image>();
        image.color = new Color(0.12f, 0.47f, 0.82f);
        Button button = obj.AddComponent<Button>();
        button.targetGraphic = image;
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(210, 62);
        TextMeshProUGUI text = AddTMP(obj.transform, "Label", label, 25, TextAlignmentOptions.Center, new Vector2(0, 0), new Vector2(1, 1), Vector2.zero, Vector2.zero);
        text.color = Color.black;
        return button;
    }

    private static void CreateScene()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        PhysicsMaterial bouncy = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Physics Materials/Bouncy_Multiply.physicsMaterial");

        GameObject arena = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        arena.name = "Floating Arena";
        arena.transform.localScale = new Vector3(8f, 0.35f, 8f);
        arena.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Arena_Material.mat");

        GameObject focal = new GameObject("Focal Point");
        focal.AddComponent<RotateCamera>();
        Camera camera = new GameObject("Main Camera").AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.transform.SetParent(focal.transform);
        camera.transform.localPosition = new Vector3(0, 7f, -10f);
        camera.transform.localRotation = Quaternion.Euler(35, 0, 0);

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0, 1f, 0);
        player.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Player_Material.mat");
        player.GetComponent<Collider>().material = bouncy;
        player.AddComponent<Rigidbody>();
        player.AddComponent<AudioSource>();

        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        indicator.name = "Powerup Indicator";
        indicator.transform.SetParent(player.transform);
        indicator.transform.localPosition = Vector3.zero;
        indicator.transform.localScale = new Vector3(1.45f, 0.05f, 1.45f);
        indicator.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Indicator_Material.mat");

        GameObject spawnerObject = new GameObject("Spawn Manager");
        SpawnManager spawner = spawnerObject.AddComponent<SpawnManager>();
        GameObject managerObject = new GameObject("Game Manager");
        GameManager manager = managerObject.AddComponent<GameManager>();

        PlayerController playerController = player.AddComponent<PlayerController>();
        playerController.Configure(manager, focal.transform, indicator, AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Powerup.wav"), AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Hit.wav"));

        spawner.Configure(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy.prefab"), AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Powerup.prefab"), manager, player.transform);

        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        canvasObject.AddComponent<GraphicRaycaster>();

        TextMeshProUGUI score = AddTMP(canvasObject.transform, "Score Text", "Score: 0", 28, TextAlignmentOptions.Left, new Vector2(0, 1), new Vector2(0, 1), new Vector2(105, -36), new Vector2(210, 48));
        TextMeshProUGUI wave = AddTMP(canvasObject.transform, "Wave Text", "Wave: 0", 28, TextAlignmentOptions.Center, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -36), new Vector2(220, 48));

        GameObject title = new GameObject("Title Screen");
        title.transform.SetParent(canvasObject.transform, false);
        CanvasGroup titleGroup = title.AddComponent<CanvasGroup>();
        AddTMP(title.transform, "Title Text", "Knockout Arena UI", 54, TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 130), new Vector2(720, 90));
        Button easy = AddButton(title.transform, "Easy Button", "Easy", new Vector2(-240, 10));
        Button medium = AddButton(title.transform, "Medium Button", "Medium", new Vector2(0, 10));
        Button hard = AddButton(title.transform, "Hard Button", "Hard", new Vector2(240, 10));
        DifficultyButton easyScript = easy.gameObject.AddComponent<DifficultyButton>();
        DifficultyButton mediumScript = medium.gameObject.AddComponent<DifficultyButton>();
        DifficultyButton hardScript = hard.gameObject.AddComponent<DifficultyButton>();
        easyScript.Configure(manager, 1);
        mediumScript.Configure(manager, 2);
        hardScript.Configure(manager, 3);
        easy.onClick.AddListener(easyScript.SetDifficulty);
        medium.onClick.AddListener(mediumScript.SetDifficulty);
        hard.onClick.AddListener(hardScript.SetDifficulty);

        GameObject gameOverPanel = new GameObject("Game Over Panel");
        gameOverPanel.transform.SetParent(canvasObject.transform, false);
        CanvasGroup gameOverGroup = gameOverPanel.AddComponent<CanvasGroup>();
        gameOverGroup.alpha = 0f;
        GameObject gameOverText = AddTMP(gameOverPanel.transform, "Game Over Text", "GAME OVER", 64, TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 80), new Vector2(620, 100)).gameObject;
        Button restart = AddButton(gameOverPanel.transform, "Restart Button", "Restart", new Vector2(0, -20));
        restart.onClick.AddListener(manager.RestartGame);
        GameOverUI gameOverUI = gameOverPanel.AddComponent<GameOverUI>();
        gameOverUI.Configure(gameOverGroup, gameOverText, restart.gameObject);
        gameOverText.SetActive(false);
        restart.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);

        manager.Configure(score, wave, player.transform, title, titleGroup, spawner, gameOverPanel, gameOverGroup, restart.gameObject);

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        Light light = new GameObject("Directional Light").AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(50, -35, 0);
        light.intensity = 1.1f;

        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/KnockoutArena_UI.unity", true) };
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "Assets/Scenes/KnockoutArena_UI.unity");
    }

    private static void ConfigurePlayerSettings()
    {
        PlayerSettings.productName = "Knockout Arena UI";
        PlayerSettings.companyName = "SWE402";
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
    }

    private static void WriteReadme()
    {
        System.IO.File.WriteAllText("README.md",
@"# SWE-402 Worksheet 6 - Knockout Arena UI, Polish & Publishing

Open Assets/Scenes/KnockoutArena_UI.unity. The game starts idle on the title screen; choose Easy, Medium, or Hard to begin.

Implemented requirements:
- Canvas uses Screen Space - Overlay.
- TextMeshProUGUI score and wave text are anchored at the top-left and top-center.
- GameManager owns score, wave text, StartGame(int), UpdateScore(int), GameOver(), RestartGame(), and the OnGameOver event.
- Game over UI and restart button start inactive; GameOverUI subscribes/unsubscribes in OnEnable/OnDisable and fades in with a coroutine.
- Title Screen groups title text and three difficulty buttons. DifficultyButton passes an inspector-set difficulty to GameManager.StartGame(int).
- Spawning, player input, score, and enemy movement are gated by GameManager.IsGameActive.
- Serialized fields use [SerializeField], [Range], [Tooltip], and [HideInInspector] according to the worksheet.
- Coroutine UI polish includes game-over fade, title fade-out, score punch, and wave text slide.

Publishing note:
The scene is added to Build Settings and Player Settings are configured. Producing WebGL/desktop builds and uploading to Unity Play or itch.io still requires a signed-in local Unity account and the matching build module.
");
    }
}
