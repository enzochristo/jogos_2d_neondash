using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public float speed = 8f;
    public float jumpForce = 15.5f;
    public float padForce = 22f;
    public float invulnerabilidade = 1.2f;
    public bool pulaAutomatico; // usado so pelo teste automatico (Editor/AutoPlayTest)

    private Rigidbody2D rb;
    private Transform visual;
    private SpriteRenderer sprite;
    private bool noChao;
    private bool morto;
    private bool terminou;
    private float bufferPulo;
    private float invulneravel;
    private float rotacao;
    private float ultimoTick;
    private Vector2 checkpoint;
    private float inicioX;
    private float fimX = 230f;

    public void Configura(Vector2 inicio, float fim)
    {
        checkpoint = inicio;
        inicioX = inicio.x;
        fimX = fim;
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        visual = transform.Find("Visual");
        sprite = visual.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (morto || terminou)
        {
            return;
        }

        // Teclado, mouse/toque e controle (botao A do Xbox = "Jump" no Input Manager)
        bool pressionou = Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0)
            || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        if (pressionou)
        {
            bufferPulo = 0.12f;
        }
        bufferPulo -= Time.deltaTime;

        if (invulneravel > 0f)
        {
            invulneravel -= Time.deltaTime;
            sprite.enabled = Mathf.FloorToInt(Time.time * 16f) % 2 == 0 || invulneravel <= 0f;
        }
        else
        {
            sprite.enabled = true;
        }

        // Cubo gira no ar e encaixa de 90 em 90 graus ao pousar
        if (noChao)
        {
            rotacao = Mathf.MoveTowardsAngle(rotacao, Mathf.Round(rotacao / 90f) * 90f, 900f * Time.deltaTime);
        }
        else
        {
            rotacao -= 300f * Time.deltaTime;
        }
        visual.rotation = Quaternion.Euler(0f, 0f, rotacao);

        if (GameController.gameOver)
        {
            Congela();
            return;
        }

        GameController.progresso = Mathf.Clamp01((transform.position.x - inicioX) / (fimX - inicioX));

        // Contagem regressiva sonora nos ultimos 10 segundos
        if (GameController.tempoRestante <= 10f && Time.time - ultimoTick >= 1f)
        {
            ultimoTick = Time.time;
            AudioManager.Play("tick");
        }
    }

    void FixedUpdate()
    {
        if (morto || terminou)
        {
            return;
        }

        bool segurando = pulaAutomatico || Input.GetButton("Jump") || Input.GetMouseButton(0)
            || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        Vector2 vel = rb.linearVelocity;
        vel.x = speed;

        if (noChao && (segurando || bufferPulo > 0f))
        {
            vel.y = jumpForce;
            bufferPulo = 0f;
            AudioManager.Play("jump");
        }
        rb.linearVelocity = vel;
        noChao = false;

        if (transform.position.y < -6f)
        {
            Morre(true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        VerificaColisao(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        VerificaColisao(collision);
    }

    void VerificaColisao(Collision2D collision)
    {
        foreach (ContactPoint2D contato in collision.contacts)
        {
            if (contato.normal.y > 0.7f)
            {
                noChao = true;
            }
            // Bateu de frente em um bloco = morre. Pousar em cima e permitido.
            else if (contato.normal.x < -0.9f)
            {
                Morre();
                return;
            }
        }
    }

    // Se ainda estiver encostado num inimigo quando acabar a invulnerabilidade, morre
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Inimigo")
        {
            Morre();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (morto || terminou)
        {
            return;
        }

        if (collision.tag == "Coletavel")
        {
            AudioManager.Play("coin");
            GameController.Collect();
            Destroy(collision.gameObject);
        }
        else if (collision.tag == "Inimigo")
        {
            Morre();
        }
        else if (collision.tag == "Pad")
        {
            AudioManager.Play("pad");
            Vector2 vel = rb.linearVelocity;
            vel.y = padForce;
            rb.linearVelocity = vel;
        }
        else if (collision.tag == "Checkpoint")
        {
            Vector2 novo = new Vector2(collision.transform.position.x, 0.5f);
            if (novo.x > checkpoint.x)
            {
                checkpoint = novo;
                AudioManager.Play("checkpoint");
            }
        }
        else if (collision.tag == "Meta")
        {
            terminou = true;
            GameController.Finish();
            AudioManager.Play("win");
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
            Fragment.Explode(transform.position, new Color(1f, 0.88f, 0.29f), 24);
        }
    }

    void Congela()
    {
        if (!terminou && !morto)
        {
            terminou = true;
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
    }

    void Morre(bool ignoraInvulnerabilidade = false)
    {
        if (morto || terminou || GameController.gameOver || (invulneravel > 0f && !ignoraInvulnerabilidade))
        {
            return;
        }
        GameController.Hit();
        morto = true;
        AudioManager.Play("hit");
        CameraFollow.Shake(0.35f);
        Fragment.Explode(transform.position, new Color(0.1f, 0.9f, 1f), 18);
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        sprite.enabled = false;

        if (GameController.gameOver)
        {
            AudioManager.Play("lose");
            terminou = true;
        }
        else
        {
            StartCoroutine(Renasce());
        }
    }

    IEnumerator Renasce()
    {
        yield return new WaitForSeconds(0.6f);
        transform.position = checkpoint;
        rb.position = checkpoint;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = true;
        rotacao = 0f;
        invulneravel = invulnerabilidade;
        sprite.enabled = true;
        morto = false;
    }
}
