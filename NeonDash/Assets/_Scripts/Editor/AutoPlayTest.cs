using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Teste automatico: um bot joga a fase e imprime o resultado no log.
// Uso: Unity -batchmode -nographics -projectPath . -executeMethod AutoPlayTest.Run -logFile -
public static class AutoPlayTest
{
    public static void Run()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        SessionState.SetBool("AutoPlayTest", true);
        EditorApplication.EnterPlaymode();
    }

    // Tira um screenshot do menu (precisa de janela grafica, nao use -nographics)
    public static void ShotMenu()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        SessionState.SetString("AutoPlayShot", "menu");
        EditorApplication.EnterPlaymode();
    }

    [InitializeOnLoadMethod]
    static void Registra()
    {
        EditorApplication.playModeStateChanged += estado =>
        {
            if (estado == PlayModeStateChange.EnteredPlayMode && SessionState.GetString("AutoPlayShot", "") == "menu")
            {
                SessionState.SetString("AutoPlayShot", "");
                new GameObject("Shot").AddComponent<Shot>();
            }
            if (estado == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool("AutoPlayTest", false))
            {
                SessionState.SetBool("AutoPlayTest", false);
                new GameObject("Bot").AddComponent<Bot>();
            }
        };
    }

    public class Shot : MonoBehaviour
    {
        System.Collections.IEnumerator Start()
        {
            yield return new WaitForSeconds(2f);
            ScreenCapture.CaptureScreenshot(System.Environment.GetEnvironmentVariable("SHOT_DIR") + "/menu.png");
            yield return new WaitForSeconds(1f);
            EditorApplication.Exit(0);
        }
    }

    public class Bot : MonoBehaviour
    {
        private float fim;
        private int tiros;
        private float[] momentos = { 2f, 6f, 9f };

        private int mortes;
        private int ultimasVidas;
        private float relogio;

        void Start()
        {
            GameController.Init();
            Time.timeScale = 3f;
            ultimasVidas = GameController.vidas;
        }

        void FixedUpdate()
        {
            PlayerMovement p = PlayerMovement.instance;
            if (p == null)
            {
                return;
            }
            relogio += Time.fixedDeltaTime;
            string dir = System.Environment.GetEnvironmentVariable("SHOT_DIR");
            if (dir != null && tiros < momentos.Length && relogio > momentos[tiros])
            {
                ScreenCapture.CaptureScreenshot(dir + "/jogo" + tiros + ".png");
                tiros++;
            }

            if (GameController.vidas != ultimasVidas)
            {
                ultimasVidas = GameController.vidas;
                mortes++;
                Debug.Log("[Bot] morreu perto de x=" + p.transform.position.x.ToString("0.0") + " vidas=" + GameController.vidas);
            }

            Vector2 pos = p.transform.position;
            bool perigo = false;
            // espinho/serra/bloco a frente
            Collider2D[] cs = Physics2D.OverlapBoxAll(pos + new Vector2(1.6f, 0f), new Vector2(0.5f, 0.7f), 0f);
            foreach (Collider2D c in cs)
            {
                if (c.gameObject != p.gameObject && (c.tag == "Inimigo" || (!c.isTrigger && c.name == "Block")))
                {
                    perigo = true;
                }
            }
            // buraco a frente
            bool chaoAFrente = Physics2D.OverlapPoint(pos + new Vector2(1.0f, -0.8f)) != null;
            if (!chaoAFrente && pos.y < 1.0f)
            {
                perigo = true;
            }
            // pad na frente: nao precisa pular
            p.pulaAutomatico = perigo;

            if (GameController.gameOver || relogio > 120f)
            {
                if (dir != null && fim < 1.5f)
                {
                    fim += Time.fixedDeltaTime;
                    if (fim > 1.2f)
                    {
                        ScreenCapture.CaptureScreenshot(dir + "/fim.png");
                    }
                    return;
                }
                Debug.Log("[Bot] FIM estado=" + GameController.estado + " x=" + pos.x.ToString("0.0") + " score=" + GameController.score
                    + " orbes=" + GameController.orbes + " tempoDecorrido=" + GameController.tempoDecorrido.ToString("0.0") + " mortes=" + mortes);
                EditorApplication.Exit(0);
            }
        }
    }
}
