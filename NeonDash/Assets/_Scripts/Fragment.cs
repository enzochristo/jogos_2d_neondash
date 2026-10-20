using UnityEngine;

// Pedacinhos que voam quando o cubo explode
public class Fragment : MonoBehaviour
{
    private Vector2 velocidade;
    private float vida = 0.8f;
    private SpriteRenderer sr;

    public static void Explode(Vector2 posicao, Color cor, int quantidade)
    {
        for (int i = 0; i < quantidade; i++)
        {
            GameObject go = GameAssets.CriaSprite("pixel", posicao, 20);
            go.transform.localScale = Vector3.one * Random.Range(0.08f, 0.22f);
            Fragment f = go.AddComponent<Fragment>();
            f.sr = go.GetComponent<SpriteRenderer>();
            f.sr.color = cor;
            f.velocidade = Random.insideUnitCircle.normalized * Random.Range(3f, 9f);
        }
    }

    void Update()
    {
        vida -= Time.deltaTime;
        velocidade.y -= 20f * Time.deltaTime;
        transform.position += (Vector3)(velocidade * Time.deltaTime);
        Color c = sr.color;
        c.a = Mathf.Clamp01(vida / 0.8f);
        sr.color = c;
        if (vida <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
