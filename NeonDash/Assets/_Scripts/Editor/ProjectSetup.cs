using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Menu Tools > Neon Dash: recria as cenas, configura o build e gera o WebGL
public static class ProjectSetup
{
    private const string cenaMenu = "Assets/Scenes/MainMenu.unity";
    private const string cenaJogo = "Assets/Scenes/SampleScene.unity";

    [MenuItem("Tools/Neon Dash/Criar Cenas")]
    public static void CriaCenas()
    {
        System.IO.Directory.CreateDirectory("Assets/Scenes");

        Scene menu = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CriaCamera(false);
        GameObject menuGo = new GameObject("MenuController");
        menuGo.AddComponent<MenuUI>();
        EditorSceneManager.SaveScene(menu, cenaMenu);

        Scene jogo = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CriaCamera(true);
        GameObject gm = new GameObject("GameManager");
        gm.AddComponent<LevelBuilder>();
        gm.AddComponent<UIManager>();
        EditorSceneManager.SaveScene(jogo, cenaJogo);

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(cenaMenu, true),
            new EditorBuildSettingsScene(cenaJogo, true)
        };
        AssetDatabase.SaveAssets();
        Debug.Log("Cenas criadas.");
    }

    static void CriaCamera(bool segueJogador)
    {
        GameObject go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        Camera cam = go.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.04f, 0.02f, 0.14f);
        go.transform.position = new Vector3(0f, 3.2f, -10f);
        go.AddComponent<AudioListener>();
        if (segueJogador)
        {
            go.AddComponent<CameraFollow>();
        }
    }

    public static void ConfiguraProjeto()
    {
        PlayerSettings.companyName = "Insper";
        PlayerSettings.productName = "Neon Dash";
        PlayerSettings.defaultWebScreenWidth = 600;
        PlayerSettings.defaultWebScreenHeight = 480;
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        PlayerSettings.WebGL.decompressionFallback = false;
        PlayerSettings.runInBackground = true;
    }

    // Uso em linha de comando: Unity -batchmode -executeMethod ProjectSetup.CriaTudo -quit
    public static void CriaTudo()
    {
        ConfiguraProjeto();
        CriaCenas();
    }

    [MenuItem("Tools/Neon Dash/Build WebGL")]
    public static void BuildWebGL()
    {
        ConfiguraProjeto();
        BuildPlayerOptions opcoes = new BuildPlayerOptions();
        opcoes.scenes = new[] { cenaMenu, cenaJogo };
        opcoes.locationPathName = "Build/WebGL";
        opcoes.target = BuildTarget.WebGL;
        opcoes.options = BuildOptions.None;
        var resultado = BuildPipeline.BuildPlayer(opcoes);
        Debug.Log("Build WebGL: " + resultado.summary.result);
        if (resultado.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            EditorApplication.Exit(1);
        }
    }
}
