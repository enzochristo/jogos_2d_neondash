using UnityEngine;

// Inimigo que vai e volta entre dois pontos
public class EnemyPatrol : MonoBehaviour
{
    public Vector2 deslocamento = new Vector2(0f, 2f);
    public float velocidade = 2f;
    private Rigidbody2D rb;
    private Vector2 origem;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        origem = rb.position;
    }

    void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * velocidade / Mathf.Max(0.1f, deslocamento.magnitude), 1f);
        rb.MovePosition(origem + deslocamento * t);
    }
}
