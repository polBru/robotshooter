﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARBulletController : MonoBehaviour
{
    [HideInInspector] public float damage;

    public float maxDuration;
    public float speed;    
    public float punish;

    public LayerMask mask;

    private float duration;
    private Vector3 previousPosition;

    void OnEnable()
    {
        previousPosition = transform.position;
        duration = maxDuration;
    }

    // Update is called once per frame
    void Update()
    {
        float multiplier = 1;
        duration -= Time.deltaTime;
        if (duration <= 0) gameObject.SetActive(false);        

        previousPosition = transform.position;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        RaycastHit[] hits = Physics.RaycastAll(previousPosition, (transform.position - previousPosition).normalized, (transform.position - previousPosition).magnitude, GameManager.instance.player.shootLayerMask);
        for (int i = 0; i < hits.Length; i++)
        {
            LayerMask layer = hits[i].collider.gameObject.layer;
            if (hits[i].collider.tag == "CriticalBox") multiplier = GameManager.instance.player.playerModel.criticalMultiplier;
            damage = damage * multiplier;
            //Debug.Log(hits[i].collider.name);
            if (layer == LayerMask.NameToLayer("Enemy")) 
            {
                GroundEnemy gEnemy = hits[i].collider.GetComponentInParent<GroundEnemy>();
                if (gEnemy != null)
                {
                    gEnemy.TakeDamage(damage);
                    if (multiplier == GameManager.instance.player.playerModel.criticalMultiplier)
                    {
                        //gEnemy.WhatHitSound("NormalHit");
                        GameManager.instance.player.IncreaseCash(gEnemy.criticalIncome);
                        //gEnemy.WhatHitSound("CriticalHit");
                        gEnemy.source.clip = AudioManager.instance.clips[4];
                        gEnemy.source.volume *= AudioManager.instance.fXVolume * AudioManager.instance.masterVolume;
                        gEnemy.source.Play();
                    }
                    else
                    {
                        gEnemy.source.clip = AudioManager.instance.clips[3];
                        gEnemy.source.volume *= AudioManager.instance.fXVolume * AudioManager.instance.masterVolume;
                        gEnemy.source.Play();
                    }
                }
                else
                {
                    FlyingEnemy fEnemy = hits[i].collider.GetComponentInParent<FlyingEnemy>();
                    if (fEnemy != null)
                    {
                        fEnemy.TakeDamage(damage);
                        if (multiplier == GameManager.instance.player.playerModel.criticalMultiplier)
                        {
                            //fEnemy.WhatHitSound("NormalHit");
                            GameManager.instance.player.IncreaseCash(fEnemy.criticalIncome);
                            //fEnemy.WhatHitSound("CriticalHit");
                            fEnemy.source.clip = AudioManager.instance.clips[4];
                            fEnemy.source.volume *= AudioManager.instance.fXVolume * AudioManager.instance.masterVolume;
                            fEnemy.source.Play();
                        }
                        else
                        {
                            fEnemy.source.clip = AudioManager.instance.clips[3];
                            fEnemy.source.volume *= AudioManager.instance.fXVolume * AudioManager.instance.masterVolume;
                            fEnemy.source.Play();
                        }
                    }
                    else
                    {
                        TankEnemy tEnemy = hits[i].collider.GetComponentInParent<TankEnemy>();
                        if (tEnemy != null)
                        {
                            tEnemy.hittedByAR = true;
                            tEnemy.TakeDamage(damage);
                            if (multiplier == GameManager.instance.player.playerModel.criticalMultiplier)
                            {
                                //tEnemy.WhatHitSound("BlockedHit");
                                GameManager.instance.player.IncreaseCash(tEnemy.criticalIncome);
                                //tEnemy.WhatHitSound("CriticalHit");
                                tEnemy.source.clip = AudioManager.instance.clips[4];
                                tEnemy.source.volume *= AudioManager.instance.fXVolume * AudioManager.instance.masterVolume;
                                tEnemy.source.Play();
                            }
                            else
                            {
                                tEnemy.source.clip = AudioManager.instance.clips[2];
                                tEnemy.source.volume *= AudioManager.instance.fXVolume * AudioManager.instance.masterVolume;
                                tEnemy.source.Play();
                            }
                        }
                    }
                }
                gameObject.SetActive(false);
                break;
            }
            if (layer == LayerMask.NameToLayer("Geometry"))
            {
                gameObject.SetActive(false);
            }
        }       
    }

    void FixedUpdate()
    {
        /*Debug.DrawRay(previousPosition, (transform.position - previousPosition).normalized, Color.red, 10);

        RaycastHit[] hits = Physics.RaycastAll(previousPosition, (transform.position - previousPosition).normalized, (transform.position - previousPosition).magnitude, GameManager.instance.player.shootLayerMask);
        for (int i = 0; i < hits.Length; i++)
        {
            LayerMask layer = hits[i].collider.gameObject.layer;
            //Debug.Log(hits[i].collider.name);
            Debug.Log(hits[i].point);
            if (layer == LayerMask.NameToLayer("Enemy"))
            {
                GroundEnemy gEnemy = hits[i].collider.GetComponent<GroundEnemy>();
                if (gEnemy != null) gEnemy.TakeDamage(damage);
                else
                {
                    FlyingEnemy fEnemy = hits[i].collider.GetComponent<FlyingEnemy>();
                    if (fEnemy != null) fEnemy.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
            if (layer == LayerMask.NameToLayer("Geometry"))
            {
                Destroy(gameObject);
            }
        }

        previousPosition = transform.position;*/
    }

    //void OnTriggerEnter(Collider col)
    //{
    //    if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    //    {
    //        Debug.Log("hit");
    //        GroundEnemy gEnemy = col.GetComponent<GroundEnemy>();
    //        if (gEnemy != null) gEnemy.TakeDamage(damage);
    //        else
    //        {
    //            FlyingEnemy fEnemy = col.GetComponent<FlyingEnemy>();
    //            if (fEnemy != null) fEnemy.TakeDamage(damage);
    //        }
    //        Destroy(gameObject);
    //    }
    //}
}
