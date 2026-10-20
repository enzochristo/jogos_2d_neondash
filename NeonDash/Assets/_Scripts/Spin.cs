using UnityEngine;

public class Spin : MonoBehaviour
{
    public float graus = 360f;

    void Update()
    {
        transform.Rotate(0f, 0f, graus * Time.deltaTime);
    }
}
