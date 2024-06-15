using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float ImpactForce;

    [SerializeField]
    private GameObject bulletImpactFX;

    private Rigidbody rb;

    private TrailRenderer trailRenderer;

    private MeshRenderer meshRenderer;

    private BoxCollider coll;

    private Vector3 startPos;

    private float flyDistance;

    private bool bulletDisapled;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }
    private void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();

    }

    private void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0)
            ReturnBulletToPool();
    }

    private void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(startPos, transform.position) > flyDistance && !bulletDisapled)
        {
            coll.enabled = false;
            meshRenderer.enabled = false;
            bulletDisapled = true;
        }
    }

    private void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPos, transform.position) > flyDistance - 1.5f)
            trailRenderer.time -= 2 * Time.deltaTime;
    }

    public void BulletSetup(float flyDistance, float impactForce)
    {
        this.ImpactForce = impactForce;
        bulletDisapled = false;
        coll.enabled = true;
        meshRenderer.enabled = true;
        trailRenderer.time = 0.25f;
        startPos = transform.position;
        this.flyDistance = flyDistance + 1;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * ImpactForce;
            Rigidbody hitRB = collision.collider.attachedRigidbody; 
            enemy.GetHit();
            enemy.HitImpact(force, collision.contacts[0].point, hitRB);
        }
        CreatImpactFX(collision);
        ReturnBulletToPool();
    }

    private void ReturnBulletToPool() => ObjectPool.Instance.ReturnObject(gameObject);

    private void CreatImpactFX(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];

            GameObject newImpactFX = ObjectPool.Instance.GetObject(bulletImpactFX);
            newImpactFX.transform.position = contact.point;
            ObjectPool.Instance.ReturnObject(newImpactFX, 1);
        }
    }
}
