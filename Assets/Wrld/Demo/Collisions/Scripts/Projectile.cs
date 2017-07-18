using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Lifetime = 10f;

    void Update ()
    {
        Lifetime -= Time.deltaTime;

        if(Lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
