using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject endGamePanel;
    public GameObject pausePanel;

    private Text scoreText;
    private Text timeText;
    private Text endTitle;
    private Text endStats;
    private Image[] hearts;
    private Image progressFill;
    private MenuActions actions;
    private Button retryButton;
    private Button resumeButton;
    private bool pausado;
    private bool endShown;

    void Start()
    {
        // Permite apertar Play direto na cena do jogo, sem passar pelo menu
        if (GameController.vidas <= 0 && !GameController.gameOver)
        {
            GameController.Init();
        }
        actions = gameObject.AddComponent<MenuActions>();
        Canvas canvas = UIFactory.CriaCanvas("HUD");
        MontaHud(canvas.transform);
        MontaFimDeJogo(canvas.transform);
        MontaPausa(canvas.transform);
    }

    void MontaHud(Transform pai)
    {
        scoreText = UIFactory.CriaTexto("Score", pai, new Vector2(0f, 1f), new Vector2(60f, -60f), "", 56, UIFactory.amarelo, TextAnchor.MiddleLeft);
        scoreText.rectTransform.pivot = new Vector2(0f, 0.5f);
        scoreText.rectTransform.anchoredPosition = new Vector2(50f, -60f);

        timeText = UIFactory.CriaTexto("Tempo", pai, new Vector2(0.5f, 1f), new Vector2(0f, -60f), "", 64, Color.white);

        hearts = new Image[GameController.vidasIniciais];
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i] = UIFactory.CriaImagem("Vida" + i, pai, new Vector2(1f, 1f), new Vector2(-70f - i * 80f, -60f), new Vector2(72f, 72f), Color.white, GameAssets.Sprite("heart"));
        }

        // Barra de progresso da fase
        UIFactory.CriaImagem("ProgressoFundo", pai, new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(900f, 22f), new Color(0f, 0f, 0f, 0.55f));
        progressFill = UIFactory.CriaImagem("Progresso", pai, new Vector2(0.5f, 0f), new Vector2(-450f, 40f), new Vector2(900f, 14f), UIFactory.ciano);
        progressFill.rectTransform.pivot = new Vector2(0f, 0.5f);
        progressFill.type = Image.Type.Filled;
        progressFill.fillMethod = Image.FillMethod.Horizontal;
        progressFill.fillOrigin = 0;
        progressFill.sprite = GameAssets.Sprite("pixel");
    }

    void MontaFimDeJogo(Transform pai)
    {
        Image fundo = UIFactory.CriaImagem("EndGamePanel", pai, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1920f, 1080f), new Color(0.03f, 0.01f, 0.12f, 0.88f));
        fundo.rectTransform.sizeDelta = new Vector2(4000f, 3000f);
        endGamePanel = fundo.gameObject;
        endTitle = UIFactory.CriaTexto("Titulo", endGamePanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, 300f), "", 120, UIFactory.rosa);
        endStats = UIFactory.CriaTexto("Stats", endGamePanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, 60f), "", 48, Color.white);
        retryButton = UIFactory.CriaBotao("Retry", endGamePanel.transform, new Vector2(0.5f, 0.5f), new Vector2(-300f, -230f), "JOGAR DE NOVO", UIFactory.rosa, actions.IniciaJogo);
        UIFactory.CriaBotao("Menu", endGamePanel.transform, new Vector2(0.5f, 0.5f), new Vector2(300f, -230f), "MENU", new Color(0.2f, 0.3f, 0.9f), actions.Menu);
        endGamePanel.SetActive(false);
    }

    void MontaPausa(Transform pai)
    {
        Image fundo = UIFactory.CriaImagem("PausePanel", pai, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(4000f, 3000f), new Color(0.03f, 0.01f, 0.12f, 0.85f));
        pausePanel = fundo.gameObject;
        UIFactory.CriaTexto("Titulo", pausePanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, 250f), "PAUSADO", 120, UIFactory.ciano);
        resumeButton = UIFactory.CriaBotao("Continuar", pausePanel.transform, new Vector2(0.5f, 0.5f), new Vector2(-300f, -50f), "CONTINUAR", UIFactory.rosa, Pausa);
        UIFactory.CriaBotao("Menu", pausePanel.transform, new Vector2(0.5f, 0.5f), new Vector2(300f, -50f), "MENU", new Color(0.2f, 0.3f, 0.9f), actions.Menu);
        pausePanel.SetActive(false);
    }

    public void Pausa()
    {
        if (GameController.gameOver)
        {
            return;
        }
        pausado = !pausado;
        Time.timeScale = pausado ? 0f : 1f;
        pausePanel.SetActive(pausado);
        if (pausado)
        {
            UIFactory.Seleciona(resumeButton.gameObject);
        }
    }

    void Update()
    {
        // Esc no teclado ou Start no controle
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            Pausa();
        }

        GameController.Tick(Time.deltaTime);
        AtualizaHud();
    }

    void AtualizaHud()
    {
        scoreText.text = "SCORE  " + GameController.score;

        float restante = GameController.tempoRestante;
        timeText.text = restante.ToString("0.0") + "s";
        timeText.color = restante <= 10f && Mathf.FloorToInt(Time.time * 4f) % 2 == 0 ? UIFactory.rosa : Color.white;

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].color = i < GameController.vidas ? Color.white : new Color(1f, 1f, 1f, 0.15f);
        }
        progressFill.fillAmount = GameController.progresso;
    }

    void FixedUpdate()
    {
        if (GameController.gameOver && !endShown)
        {
            endShown = true;
            Invoke("MostraFim", 0.8f);
        }
    }

    void MostraFim()
    {
        switch (GameController.estado)
        {
            case GameState.Venceu:
                endTitle.text = "FASE COMPLETA!";
                endTitle.color = UIFactory.amarelo;
                break;
            case GameState.TempoEsgotado:
                endTitle.text = "TEMPO ESGOTADO";
                endTitle.color = UIFactory.rosa;
                AudioManager.Play("lose");
                break;
            default:
                endTitle.text = "SEM VIDAS";
                endTitle.color = UIFactory.rosa;
                break;
        }

        string stats = "Score: " + GameController.score + "     Orbes: " + GameController.orbes + "\n";
        stats += "Tempo da partida: " + GameController.tempoDecorrido.ToString("0.00") + "s\n";
        if (GameController.melhorTempo > 0f)
        {
            stats += "Melhor tempo: " + GameController.melhorTempo.ToString("0.00") + "s";
            if (GameController.novoRecorde)
            {
                stats += "  (NOVO RECORDE!)";
            }
        }
        endStats.text = stats;
        endGamePanel.SetActive(true);
        UIFactory.Seleciona(retryButton.gameObject);
    }
}
