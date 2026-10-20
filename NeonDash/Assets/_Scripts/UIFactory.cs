using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Ajuda a montar a interface por codigo (Canvas, textos, botoes)
public static class UIFactory
{
    public static readonly Color rosa = new Color(1f, 0.18f, 0.58f);
    public static readonly Color ciano = new Color(0.1f, 0.9f, 1f);
    public static readonly Color amarelo = new Color(1f, 0.88f, 0.29f);

    private static readonly Vector2 meio = new Vector2(0.5f, 0.5f);

    private static Font fonte;

    public static Font Fonte
    {
        get
        {
            if (fonte == null)
            {
                fonte = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            return fonte;
        }
    }

    public static Canvas CriaCanvas(string nome)
    {
        GameObject go = new GameObject(nome);
        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1200f, 960f);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();

        // Necessario para clicar e navegar com teclado/controle
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
        return canvas;
    }

    public static RectTransform CriaRect(string nome, Transform pai, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 tamanho)
    {
        GameObject go = new GameObject(nome, typeof(RectTransform));
        go.transform.SetParent(pai, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = tamanho;
        return rt;
    }

    public static Text CriaTexto(string nome, Transform pai, Vector2 anchor, Vector2 pos, string texto, int tamanho, Color cor, TextAnchor alinhamento = TextAnchor.MiddleCenter)
    {
        RectTransform rt = CriaRect(nome, pai, anchor, meio, pos, new Vector2(1400f, tamanho * 1.6f));
        Text t = rt.gameObject.AddComponent<Text>();
        t.font = Fonte;
        t.text = texto;
        t.fontSize = tamanho;
        t.fontStyle = FontStyle.Bold;
        t.color = cor;
        t.alignment = alinhamento;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        Outline o = rt.gameObject.AddComponent<Outline>();
        o.effectColor = new Color(0f, 0f, 0f, 0.85f);
        o.effectDistance = new Vector2(3f, -3f);
        return t;
    }

    public static Image CriaImagem(string nome, Transform pai, Vector2 anchor, Vector2 pos, Vector2 tamanho, Color cor, Sprite sprite = null)
    {
        RectTransform rt = CriaRect(nome, pai, anchor, meio, pos, tamanho);
        Image img = rt.gameObject.AddComponent<Image>();
        img.color = cor;
        img.sprite = sprite;
        img.raycastTarget = false;
        return img;
    }

    public static Button CriaBotao(string nome, Transform pai, Vector2 anchor, Vector2 pos, string texto, Color cor, UnityEngine.Events.UnityAction acao)
    {
        RectTransform rt = CriaRect(nome, pai, anchor, meio, pos, new Vector2(520f, 110f));
        Image img = rt.gameObject.AddComponent<Image>();
        img.color = Color.white;
        Button b = rt.gameObject.AddComponent<Button>();
        ColorBlock cores = b.colors;
        cores.normalColor = cor;
        cores.highlightedColor = Color.Lerp(cor, Color.white, 0.45f);
        cores.selectedColor = Color.Lerp(cor, Color.white, 0.45f);
        cores.pressedColor = Color.Lerp(cor, Color.black, 0.3f);
        b.colors = cores;
        b.targetGraphic = img;
        b.onClick.AddListener(acao);
        Text t = CriaTexto("Texto", rt, new Vector2(0.5f, 0.5f), Vector2.zero, texto, 52, Color.white);
        t.rectTransform.sizeDelta = new Vector2(520f, 110f);
        return b;
    }

    public static void Seleciona(GameObject go)
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(go);
        }
    }
}
