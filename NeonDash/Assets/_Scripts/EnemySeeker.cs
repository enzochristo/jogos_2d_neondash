using UnityEngine;

// Inimigo perseguidor: quando o jogador chega perto ele desce/sobe para a altura dele
public class EnemySeeker : MonoBehaviour
{
    public float alcance = 13f;
    public float velocidadeY = 2.2f;
    public float velocidadeX = 1.5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        PlayerMovement player = PlayerMovement.instance;
        if (player == null)
        {
            return;
        }

        Vector2 pos = rb.position;
        float distancia = pos.x - player.transform.position.x;
        if (distancia < alcance && distancia > -2f)
        {
            float alvoY = Mathf.Clamp(player.transform.position.y + 0.3f, 1.2f, 4.5f);
            pos.y = Mathf.MoveTowards(pos.y, alvoY, velocidadeY * Time.fixedDeltaTime);
            pos.x -= velocidadeX * Time.fixedDeltaTime;
            rb.MovePosition(pos);
        }
    }
}
