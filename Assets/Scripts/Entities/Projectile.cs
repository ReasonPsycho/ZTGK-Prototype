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
        //if ally unit shots, ignore other ally units
        if (dealer.type == UnitType.ALLY && other.CompareTag("PlayerUnit"))
        {
            return;
        }

        //if enemy unit shots, ignore other enemy units
        else if (dealer.type == UnitType.ENEMY && other.CompareTag("Enemy"))
        {
            return;
        }
 

        curentCollider = other;
        ps.Play();
        GetComponent<MeshRenderer>().enabled = false;

        if (dealer.type == UnitType.ALLY &&  other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<EnemyAI>().unit.TakeDmg(dmg, knockback, dealer, aoe);
        }

        else if (dealer.type == UnitType.ENEMY && other.CompareTag("PlayerUnit"))
        {
            other.GetComponentInParent<UnitAI>().unit.TakeDmg(dmg, knockback, dealer, aoe);
        }
        StartCoroutine(WaitAndDestroy());
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if ally unit shots, ignore other ally units
        if (dealer.type == UnitType.ALLY && collision.collider.CompareTag("PlayerUnit"))
        {
            return;
        }

        //if enemy unit shots, ignore other enemy units
        else if (dealer.type == UnitType.ENEMY && collision.collider.CompareTag("Enemy"))
        {
            return;
        }

        curentCollider = collision.collider;
        ps.Play();
        GetComponent<MeshRenderer>().enabled = false;
        if (collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponentInParent<EnemyAI>().unit.TakeDmg(dmg, knockback, dealer, aoe);
        }
        else if (dealer.type == UnitType.ENEMY && collision.collider.CompareTag("PlayerUnit"))
        {
            collision.collider.GetComponentInParent<UnitAI>().unit.TakeDmg(dmg, knockback, dealer, aoe);
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
