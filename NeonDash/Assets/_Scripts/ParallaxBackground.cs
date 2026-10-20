using UnityEngine;

// Duas copias do fundo que se repetem; cada camada anda numa fracao da camera
public class ParallaxBackground : MonoBehaviour
{
    public float fator = 0.3f;
    public float largura = 32f;
    private Transform cam;
    private Transform[] copias = new Transform[2];

    public static void Cria(Camera camera, float fator, float alfa, float escala, int ordem)
    {
        GameObject go = new GameObject("Parallax " + fator);
        ParallaxBackground pb = go.AddComponent<ParallaxBackground>();
        pb.fator = fator;
        pb.cam = camera.transform;
        pb.largura = 32f * escala;
        for (int i = 0; i < 2; i++)
        {
            GameObject c = GameAssets.CriaSprite("background", Vector2.zero, ordem, go.transform);
            c.transform.localScale = Vector3.one * escala;
            c.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alfa);
            pb.copias[i] = c.transform;
        }
    }

    void LateUpdate()
    {
        float deslocamento = Mathf.Repeat(cam.position.x * fator, largura);
        float x = cam.position.x - deslocamento;
        float y = cam.position.y * 0.5f + 1.5f;
        copias[0].position = new Vector3(x, y, 5f);
        copias[1].position = new Vector3(x + largura, y, 5f);
    }
}
