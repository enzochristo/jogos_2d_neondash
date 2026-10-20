using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private AudioSource musica;
    private AudioSource efeitos;
    private string musicaAtual = "";
    private bool mudo;

    private static AudioManager Get()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("AudioManager");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<AudioManager>();
            instance.musica = go.AddComponent<AudioSource>();
            instance.musica.loop = true;
            instance.musica.volume = 0.45f;
            instance.efeitos = go.AddComponent<AudioSource>();
            instance.efeitos.volume = 0.8f;
        }
        return instance;
    }

    public static void Play(string nome)
    {
        AudioClip clip = GameAssets.Clip(nome);
        if (clip != null)
        {
            Get().efeitos.PlayOneShot(clip);
        }
    }

    // Uma unica musica para o jogo todo. Se existir Resources/Audio/music_custom, ela substitui a padrao.
    public static void PlayMusic()
    {
        string nome = GameAssets.Clip("music_custom") != null ? "music_custom" : "music";
        AudioManager am = Get();
        if (am.musicaAtual == nome && am.musica.isPlaying)
        {
            return;
        }
        am.musicaAtual = nome;
        am.musica.clip = GameAssets.Clip(nome);
        am.musica.Play();
    }

    void Update()
    {
        // M liga/desliga o som
        if (Input.GetKeyDown(KeyCode.M))
        {
            mudo = !mudo;
            AudioListener.volume = mudo ? 0f : 1f;
        }
    }
}
