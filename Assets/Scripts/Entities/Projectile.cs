using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ParticleSystem ps;
    public SphereCollider sc;

    public Collider curentCollider;

    public GameObject target;
    private GameObject prevtarget;

    public float dmg;
    public float knockback;
    public Unit dealer;
    public float aoe;


    private void Start()
    {
        sc = GetComponent<SphereCollider>();
        ps = GetComponentInChildren<ParticleSystem>();
        StartCoroutine(DestroyAfterDuration(10));
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
        }
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 0.2f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerUnit"))
        {
            print("Projectile hit player unit");
            return;
        }
        print("Trigger -> Projectile hit: " + other.name);
        curentCollider = other;
        ps.Play();
        GetComponent<MeshRenderer>().enabled = false;

        if (other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<EnemyAI>().unit.TakeDmg(dmg, knockback, dealer, aoe);
        }
        StartCoroutine(WaitAndDestroy());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("PlayerUnit"))
        {
            print("Projectile hit player unit");
            return;
        }
        print("Collision -> Projectile hit: " + collision.collider.name);
        curentCollider = collision.collider;
        ps.Play();
        GetComponent<MeshRenderer>().enabled = false;
        if (collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponentInParent<EnemyAI>().unit.TakeDmg(dmg, knockback, dealer, aoe);
        }

        StartCoroutine(WaitAndDestroy());
    }

    private IEnumerator WaitAndDestroy()
    {
        sc.enabled = false;
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }


    private IEnumerator DestroyAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
