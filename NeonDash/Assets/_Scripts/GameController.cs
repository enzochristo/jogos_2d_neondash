using UnityEngine;

public enum GameState
{
    Jogando,
    Venceu,
    TempoEsgotado,
    SemVidas
}

public static class GameController
{
    public const int vidasIniciais = 5;
    public const float tempoInicial = 40f;
    public const int pontosOrbe = 100;
    public const float bonusTempoOrbe = 2f;

    private const string chaveMelhorTempo = "NeonDash.melhorTempo";
    private const string chaveMelhorScore = "NeonDash.melhorScore";

    public static GameState estado;
    public static int score;
    public static int vidas;
    public static int orbes;
    public static float tempoRestante;
    public static float tempoDecorrido;
    public static float progresso;
    public static bool novoRecorde;

    public static bool gameOver
    {
        get { return estado != GameState.Jogando; }
    }

    public static float melhorTempo
    {
        get { return PlayerPrefs.GetFloat(chaveMelhorTempo, 0f); }
    }

    public static int melhorScore
    {
        get { return PlayerPrefs.GetInt(chaveMelhorScore, 0); }
    }

    public static void Init()
    {
        estado = GameState.Jogando;
        score = 0;
        orbes = 0;
        vidas = vidasIniciais;
        tempoRestante = tempoInicial;
        tempoDecorrido = 0f;
        progresso = 0f;
        novoRecorde = false;
    }

    // Chamado todo frame pelo UIManager: o tempo e uma mecanica, nao so um enfeite
    public static void Tick(float dt)
    {
        if (gameOver)
        {
            return;
        }
        tempoDecorrido += dt;
        tempoRestante -= dt;
        if (tempoRestante <= 0f)
        {
            tempoRestante = 0f;
            estado = GameState.TempoEsgotado;
        }
    }

    public static void Collect()
    {
        orbes++;
        score += pontosOrbe;
        tempoRestante += bonusTempoOrbe;
    }

    public static void Hit()
    {
        vidas--;
        if (vidas <= 0)
        {
            vidas = 0;
            estado = GameState.SemVidas;
        }
    }

    public static void Finish()
    {
        if (gameOver)
        {
            return;
        }
        estado = GameState.Venceu;
        score += Mathf.RoundToInt(tempoRestante * 10f) + vidas * 50;
        progresso = 1f;

        float melhor = melhorTempo;
        if (melhor <= 0f || tempoDecorrido < melhor)
        {
            PlayerPrefs.SetFloat(chaveMelhorTempo, tempoDecorrido);
            novoRecorde = true;
        }
        if (score > melhorScore)
        {
            PlayerPrefs.SetInt(chaveMelhorScore, score);
        }
        PlayerPrefs.Save();
    }
}
