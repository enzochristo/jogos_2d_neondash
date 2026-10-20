using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float adiantamento = 5f;
    private static float tremor;
    private float y = 3.2f;

    public static void Shake(float duracao)
    {
        tremor = duracao;
    }

    void LateUpdate()
    {
        PlayerMovement player = PlayerMovement.instance;
        if (player == null)
        {
            return;
        }

        float alvoY = Mathf.Max(3.2f, player.transform.position.y - 0.8f);
        y = Mathf.Lerp(y, alvoY, 4f * Time.deltaTime);
        Vector3 pos = new Vector3(player.transform.position.x + adiantamento, y, -10f);

        if (tremor > 0f)
        {
            tremor -= Time.deltaTime;
            pos += (Vector3)(Random.insideUnitCircle * 0.25f * Mathf.Clamp01(tremor * 3f));
        }
        transform.position = pos;
    }
}
