using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    private Text titulo;

    void Start()
    {
        MenuActions actions = gameObject.AddComponent<MenuActions>();
        Canvas canvas = UIFactory.CriaCanvas("Menu");
        Transform t = canvas.transform;

        Image fundo = UIFactory.CriaImagem("Fundo", t, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1707f, 960f), Color.white, GameAssets.Sprite("menu_bg"));

        titulo = UIFactory.CriaTexto("Titulo", t, new Vector2(0.5f, 1f), new Vector2(0f, -130f), "NEON DASH", 150, UIFactory.rosa);
        UIFactory.CriaTexto("Subtitulo", t, new Vector2(0.5f, 1f), new Vector2(0f, -250f), "PULE.  DESVIE.  CHEGUE AO PORTAL.", 44, UIFactory.ciano);

        Button jogar = UIFactory.CriaBotao("Jogar", t, new Vector2(0.5f, 0f), new Vector2(0f, 330f), "JOGAR", UIFactory.rosa, actions.IniciaJogo);
        UIFactory.CriaTexto("Controles", t, new Vector2(0.5f, 0f), new Vector2(0f, 200f),
            "ESPACO / SETA / CLIQUE / BOTAO A  =  PULAR\nESC / START  =  PAUSAR        M  =  SOM", 26, Color.white);

        string recorde = "";
        if (GameController.melhorTempo > 0f)
        {
            recorde = "Melhor tempo: " + GameController.melhorTempo.ToString("0.00") + "s     Melhor score: " + GameController.melhorScore;
        }
        UIFactory.CriaTexto("Recorde", t, new Vector2(0.5f, 0f), new Vector2(0f, 120f), recorde, 30, UIFactory.amarelo);

#if !UNITY_WEBGL
        Button sair = UIFactory.CriaBotao("Sair", t, new Vector2(0.5f, 0f), new Vector2(0f, 330f), "SAIR", new Color(0.2f, 0.3f, 0.9f), actions.Sair);
        jogar.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300f, 330f);
        sair.GetComponent<RectTransform>().anchoredPosition = new Vector2(300f, 330f);
#endif

        UIFactory.Seleciona(jogar.gameObject);
        AudioManager.PlayMusic();
    }

    void Update()
    {
        // Titulo pulsando no ritmo da musica
        float pulso = 1f + Mathf.Sin(Time.time * 5f) * 0.03f;
        titulo.rectTransform.localScale = Vector3.one * pulso;

        // Se o jogador clicar fora dos botoes, mantem o foco para o controle funcionar
        if (UnityEngine.EventSystems.EventSystem.current != null
            && UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null
            && (Input.GetAxisRaw("Vertical") != 0f || Input.GetAxisRaw("Horizontal") != 0f))
        {
            Button primeiro = FindFirstObjectByType<Button>();
            if (primeiro != null)
            {
                UIFactory.Seleciona(primeiro.gameObject);
            }
        }
    }
}
