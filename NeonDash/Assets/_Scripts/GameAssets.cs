using System.Collections.Generic;
using UnityEngine;

// Carrega sprites e audios da pasta Resources (cache para nao carregar duas vezes)
public static class GameAssets
{
    private static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    private static Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    public static Sprite Sprite(string nome)
    {
        if (!sprites.ContainsKey(nome))
        {
            sprites[nome] = Resources.Load<Sprite>("Sprites/" + nome);
        }
        return sprites[nome];
    }

    public static AudioClip Clip(string nome)
    {
        if (!clips.ContainsKey(nome))
        {
            clips[nome] = Resources.Load<AudioClip>("Audio/" + nome);
        }
        return clips[nome];
    }

    public static GameObject CriaSprite(string nome, Vector2 posicao, int ordem, Transform pai = null)
    {
        GameObject go = new GameObject(nome);
        go.transform.SetParent(pai, false);
        go.transform.position = posicao;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite(nome);
        sr.sortingOrder = ordem;
        return go;
    }
}
