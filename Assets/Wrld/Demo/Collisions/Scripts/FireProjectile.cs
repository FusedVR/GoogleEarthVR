using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [SerializeField]
    private GameObject m_projectile;

    [SerializeField]
    private float m_velocity = 500.0f;

    [SerializeField]
    private float m_lifetime = 10.0f;

    [SerializeField]
    private float m_distance = 10.0f;

    void Update ()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            var screenSpaceMousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
            var rayThroughCamera = Camera.main.ScreenPointToRay(screenSpaceMousePosition);            
            GameObject projectile = Instantiate(m_projectile, transform.position, Quaternion.LookRotation(rayThroughCamera.direction)) as GameObject;
            projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * m_velocity;
            projectile.GetComponent<Projectile>().Lifetime = m_lifetime;
        }
    }
}
