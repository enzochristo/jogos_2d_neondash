using System.Collections.Generic;
using UnityEngine;

// Monta a fase inteira por codigo. Cada tile ocupa [x, x+1] x [y, y+1] e o chao termina em y = 0.
public class LevelBuilder : MonoBehaviour
{
    public const float inicioX = 2f;
    public const float fimX = 232f;
    private const float fimChao = 250f;
    private const float inicioChao = -12f;

    private Transform raiz;
    private List<Vector2> buracos = new List<Vector2>();

    void Awake()
    {
        Camera cam = Camera.main;
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        cam.backgroundColor = new Color(0.04f, 0.02f, 0.14f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        ParallaxBackground.Cria(cam, 0.15f, 0.7f, 1.2f, -20);
        ParallaxBackground.Cria(cam, 0.4f, 0.18f, 1.0f, -19);

        raiz = new GameObject("Level").transform;
        MontaFase();
        MontaChao();
        MontaJogador();
        AudioManager.PlayMusic();
    }

    void MontaFase()
    {
        // Introducao: espinhos simples
        Spike(14);
        Spike(22); Spike(23);
        Orb(23, 2);

        // Primeiros degraus e plataforma
        Block(30, 0, 3, 1);
        Spike(37);

        // Primeiro buraco
        Gap(42, 3);
        Orb(43, 2);
        Checkpoint(52);

        // Pad de pulo e plataforma alta
        Pad(58);
        Block(62, 3, 6, 1);
        Orb(64, 5);
        Spike(74);

        // Serra vertical: passe por baixo ou por cima
        Saw(82, 0.5f, 0f, 2.2f);
        Spike(92); Spike(93);
        Orb(93, 2);
        Checkpoint(100);

        // Perseguidor
        Seeker(112, 2.5f);
        Orb(118, 2);

        // Buracos em sequencia
        Gap(128, 3);
        Spike(133);
        Gap(138, 4);
        Orb(140, 3);
        Checkpoint(147);

        // Escada de blocos
        Block(152, 0, 2, 1);
        Block(156, 0, 2, 2);
        Spike(161); Spike(162);
        Orb(162, 2);

        // Serras: vertical e horizontal
        Saw(170, 0.5f, 0f, 2.2f);
        Saw(177, 0.5f, 3f, 0f);
        Checkpoint(184);

        // Pad com orbe no alto e espinhos
        Pad(188);
        Orb(193, 5);
        Spike(196); Spike(197);

        // Reta final
        Seeker(204, 2.5f);
        Spike(210);
        Checkpoint(214);
        Gap(218, 3);
        Spike(224); Spike(225);
        Finish(232);
    }

    // ------------------------------------------------------------ construtores

    void Gap(float x, float largura)
    {
        buracos.Add(new Vector2(x, x + largura));
    }

    void Block(float x, float y, int w, int h)
    {
        GameObject go = new GameObject("Block");
        go.transform.SetParent(raiz, false);
        go.transform.position = new Vector2(x + w / 2f, y + h / 2f);
        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(w, h);
        col.sharedMaterial = SemAtrito();
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                GameAssets.CriaSprite("block", new Vector2(x + i + 0.5f, y + j + 0.5f), 2, go.transform);
            }
        }
    }

    void Spike(float x)
    {
        GameObject go = GameAssets.CriaSprite("spike", new Vector2(x + 0.5f, 0.5f), 3, raiz);
        go.tag = "Inimigo";
        PolygonCollider2D col = go.AddComponent<PolygonCollider2D>();
        col.isTrigger = true;
        col.points = new Vector2[]
        {
            new Vector2(-0.38f, -0.45f), new Vector2(0.38f, -0.45f), new Vector2(0f, 0.32f)
        };
    }

    void Orb(float x, float y)
    {
        GameObject go = GameAssets.CriaSprite("orb", new Vector2(x + 0.5f, y + 0.5f), 4, raiz);
        go.tag = "Coletavel";
        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.4f;
        go.AddComponent<Bob>();
    }

    void Pad(float x)
    {
        GameObject go = GameAssets.CriaSprite("pad", new Vector2(x + 0.5f, 0.25f), 3, raiz);
        go.tag = "Pad";
        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.9f, 0.4f);
    }

    void Checkpoint(float x)
    {
        GameObject go = GameAssets.CriaSprite("checkpoint", new Vector2(x + 0.5f, 1f), 1, raiz);
        go.tag = "Checkpoint";
        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.6f, 14f);
    }

    void Finish(float x)
    {
        GameObject go = GameAssets.CriaSprite("finish", new Vector2(x + 0.5f, 2.5f), 1, raiz);
        go.transform.localScale = new Vector3(1.6f, 1.6f, 1f);
        go.tag = "Meta";
        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.6f, 20f);
        go.AddComponent<Bob>().amplitude = 0.1f;
    }

    // dx/dy: quanto a serra anda a partir da posicao inicial (vai e volta)
    void Saw(float x, float y, float dx, float dy)
    {
        GameObject go = new GameObject("Saw");
        go.transform.SetParent(raiz, false);
        go.transform.position = new Vector2(x + 0.5f, y + 0.5f);
        go.tag = "Inimigo";
        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.48f;
        EnemyPatrol patrulha = go.AddComponent<EnemyPatrol>();
        patrulha.deslocamento = new Vector2(dx, dy);

        GameObject visual = GameAssets.CriaSprite("saw", go.transform.position, 3, go.transform);
        visual.transform.localScale = Vector3.one * 0.55f;
        visual.AddComponent<Spin>();
    }

    void Seeker(float x, float y)
    {
        GameObject go = new GameObject("Seeker");
        go.transform.SetParent(raiz, false);
        go.transform.position = new Vector2(x + 0.5f, y + 0.5f);
        go.tag = "Inimigo";
        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.38f;
        go.AddComponent<EnemySeeker>();
        GameObject visual = GameAssets.CriaSprite("seeker", go.transform.position, 3, go.transform);
        visual.transform.localScale = Vector3.one * 0.9f;
        visual.AddComponent<Bob>().amplitude = 0.08f;
    }

    // ------------------------------------------------------------ chao e jogador

    void MontaChao()
    {
        buracos.Sort((a, b) => a.x.CompareTo(b.x));
        float cursor = inicioChao;
        foreach (Vector2 buraco in buracos)
        {
            Segmento(cursor, buraco.x);
            cursor = buraco.y;
        }
        Segmento(cursor, fimChao);
    }

    void Segmento(float de, float ate)
    {
        float largura = ate - de;
        GameObject go = new GameObject("Ground");
        go.transform.SetParent(raiz, false);
        go.transform.position = new Vector2(de + largura / 2f, -1f);
        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(largura, 2f);
        col.sharedMaterial = SemAtrito();

        for (int i = 0; i < Mathf.RoundToInt(largura); i++)
        {
            GameAssets.CriaSprite("ground", new Vector2(de + i + 0.5f, -0.5f), 2, go.transform);
        }
        // Preenchimento escuro abaixo do chao
        GameObject fundo = GameAssets.CriaSprite("pixel", new Vector2(de + largura / 2f, -5.5f), 1, go.transform);
        fundo.transform.localScale = new Vector3(largura, 9f, 1f);
        fundo.GetComponent<SpriteRenderer>().color = new Color(0.05f, 0.04f, 0.2f);
    }

    void MontaJogador()
    {
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector2(inicioX + 0.5f, 0.5f);
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 5f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        BoxCollider2D col = player.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.9f, 0.9f);
        col.sharedMaterial = SemAtrito();

        GameObject visual = GameAssets.CriaSprite("player", player.transform.position, 10, player.transform);
        visual.name = "Visual";

        PlayerMovement pm = player.AddComponent<PlayerMovement>();
        pm.Configura(player.transform.position, fimX);
        PlayerMovement.instance = pm;
    }

    PhysicsMaterial2D SemAtrito()
    {
        PhysicsMaterial2D m = new PhysicsMaterial2D("SemAtrito");
        m.friction = 0f;
        m.bounciness = 0f;
        return m;
    }
}
