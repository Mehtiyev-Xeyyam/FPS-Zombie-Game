using System;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float force = 500f;
    [SerializeField] int damage = 20;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<ZombieController>().TakeDamage(damage);
        }
    }
}