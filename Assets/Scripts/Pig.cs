using UnityEngine;

public class Pig : MonoBehaviour
{
    public int scoreValue = 100;
    public float minImpactSpeed = 2f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= minImpactSpeed)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterPigDestroyed(scoreValue);
            }
            Destroy(gameObject);
        }
    }
}
