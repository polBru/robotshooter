﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    //public GameObject explosionEffect;

    public float delay;
    public float explosionRadius;
    public float force;
    public float maxDamage;
    public float maxDamageToPlayer;
    public GameObject explosionParticles;

    private float countdownDelay;
    private bool hasExploded = false;

    // Start is called before the first frame update
    void Start()
    {
        countdownDelay = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdownDelay -= Time.deltaTime;
        if (countdownDelay <= 0 && !hasExploded)
        {
            Explode();
        }
    }

    private void Explode()
    {
        hasExploded = true;
        //Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            /*Rigidbody rgb = nearbyObject.GetComponent<Rigidbody>();
            if (rgb != null)
            {
                rgb.AddExplosionForce(force, transform.position, explosionRadius);
            }*/
            float dist = Vector3.Distance(transform.position, nearbyObject.transform.position);
            float ratio = Mathf.Clamp01(1 - dist / explosionRadius);

            GroundEnemy gEnemy = nearbyObject.GetComponentInParent<GroundEnemy>();
            if (gEnemy != null) gEnemy.TakeDamage(maxDamage * ratio); 
            else
            {
                FlyingEnemy fEnemy = nearbyObject.GetComponentInParent<FlyingEnemy>();
                if (fEnemy != null) fEnemy.TakeDamage(maxDamage * ratio);
                else
                {
                    TankEnemy tEnemy = nearbyObject.GetComponentInParent<TankEnemy>();
                    if (tEnemy != null) tEnemy.TakeDamage(maxDamage * ratio);
                    else
                    {
                        PlayerController player = nearbyObject.GetComponent<PlayerController>();
                        if (player != null) player.TakeDamage(maxDamageToPlayer * ratio, 0);
                    }
                }
            }
        }
        AudioManager.instance.PlayOneShotSound("GranadeExplosion", transform.position);
        Instantiate(explosionParticles, transform.position, transform.rotation);
        Destroy(gameObject);        
    }
}
