using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float m_ThrowForce;
    [SerializeField] private float m_ExplosiveDamage;
    [SerializeField] private float m_ExplosiveDelay;
    [SerializeField] private float m_ExplosionRadius;
    [SerializeField] private float m_ExplosionForce;
    [SerializeField] private GameObject m_ExplosionEffect;
    private float m_Countdown;
    private bool m_Exploded = false;
    private AudioManager m_AudioManager;

    void Start()
    {
        m_Countdown = m_ExplosiveDelay;
        m_AudioManager = FindObjectOfType<AudioManager>();
        ThrowGrenade();
    }

    void Update()
    {
        m_Countdown -= Time.deltaTime;
        if(m_Countdown <= 0f && !m_Exploded)
        {
            m_Exploded = true;
            Explode();
        }
    }

    private void Explode()
    {
        Collider[] objectsWithinExplosion = Physics.OverlapSphere(transform.position, m_ExplosionRadius);
        foreach (Collider nearbyObject in objectsWithinExplosion)
        {
            if (nearbyObject.gameObject.tag == "Enemy" || nearbyObject.gameObject.tag == "Civilian")
            {
                Vector3 middleOfPerson = nearbyObject.transform.position;
                Vector3 direction = middleOfPerson - transform.position;
                float distanceToTarget = direction.magnitude;
                float distanceFromImpactToEnemy = distanceToTarget - m_ExplosionRadius;
                distanceFromImpactToEnemy = -distanceFromImpactToEnemy;
                float damageMultiplier = distanceFromImpactToEnemy / m_ExplosionRadius;
                float damage = m_ExplosiveDamage * damageMultiplier;
                nearbyObject.GetComponent<IDamageable>().TakeDamage(damage, "Grenade");
            }

            Rigidbody rigidBody = nearbyObject.GetComponent<Rigidbody>();
            if(rigidBody != null)
            {
                rigidBody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
            }
        }

        m_AudioManager.RandomizePitchSound("GrenadeExplosion");
        m_AudioManager.PlaySound("GrenadeExplosion");
        GameObject explosion = Instantiate(m_ExplosionEffect, transform.position, transform.rotation);
        Destroy(explosion.gameObject, 2f);
        Destroy(gameObject);
    }

    private void ThrowGrenade()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        rigidBody.AddForce(transform.forward * m_ThrowForce, ForceMode.VelocityChange);
    }
}
