using UnityEngine;

// Flutua para cima e para baixo (usado nos orbes)
public class Bob : MonoBehaviour
{
    public float amplitude = 0.15f;
    private Vector3 origem;
    private float fase;

    void Start()
    {
        origem = transform.position;
        fase = Random.value * 6.28f;
    }

    void Update()
    {
        transform.position = origem + Vector3.up * Mathf.Sin(Time.time * 3f + fase) * amplitude;
    }
}
