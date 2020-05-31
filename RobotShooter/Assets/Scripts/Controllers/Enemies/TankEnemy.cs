﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class TankEnemy : MonoBehaviour
{
    public enum State { INITIAL, CHASE, ATTACK, HIT, STUNNED, DEATH }
    public State currentState = State.INITIAL;
    NavMeshAgent agent;
    NavMeshObstacle obstacle;
    Rigidbody rb;
    Animator anim;
    //PlayerController player;
    GameObject player;
    public GameObject bullet;
    public Transform[] Cannons;
    public Transform[] Arms;
    public LayerMask mask;
    bool rightCannon = true;
    float elapsedTime = 0;
    float empTimeStun;
    public Transform armInitForward;
    [HideInInspector]
    public GameObject target;

    [HideInInspector] public bool hittedByAR;

    [Header("Stats")]
    public float health;
    public float damage;
    public float minDistAttack;
    public float maxDistAttack;
    public float speed;
    public float fireRate;
    public float repathTime;
    public float hitTime;
    public float deathTime;
    public int hitIncome;
    public int punishHitIncome;
    public int criticalIncome;
    public int killIncome;
    public float targetRadiusDetection;
    public float rotSpeed;

    [Header("StatIncrements")]
    public float healthInc;
    public float damageInc;
    public float maxDamage;
    public float speedInc;
    public float maxSpeed;

    // Start is called before the first frame update
    void Start()
    {/*
        //player = GameManager.instance.player;
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        agent.speed = speed;
        agent.stoppingDistance = minDistAttack;
        armInitForward = Arms[0].forward;

        IncrementStats();*/
    }

    private void OnEnable()
    {
        //player = GameManager.instance.player;
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        agent.speed = speed;
        agent.stoppingDistance = minDistAttack;
        armInitForward = Arms[0];

        //IncrementStats();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.INITIAL:
                ChangeState(State.CHASE);
                break;
            case State.CHASE:
                if (target == null)
                {
                    ChangeState(State.CHASE);
                    break;
                }
                if (DistanceToTargetSquared(gameObject, target) <= minDistAttack * minDistAttack && !PlayerHitLeft() && !PlayerHitRight())
                {
                    ChangeState(State.ATTACK);
                    break;
                }
                //transform.forward = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
                Vector3 newDirectionChase = Vector3.RotateTowards(transform.forward, new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z), rotSpeed * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDirectionChase);
                break;
            case State.ATTACK:
                if (target == null)
                {
                    ChangeState(State.CHASE);
                    break;
                }
                if (DistanceToTargetSquared(gameObject, target) >= maxDistAttack * maxDistAttack || PlayerHitLeft() || PlayerHitRight())
                {
                    ChangeState(State.CHASE);
                    break;
                }
                //transform.forward = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);                
                Vector3 newDirectionAttack = Vector3.RotateTowards(transform.forward, new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z), rotSpeed * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDirectionAttack);

                Vector3 tPos0 = (new Vector3(target.transform.position.x, target.transform.position.y - 1, target.transform.position.z) - Arms[0].position).normalized;
                Vector3 tPos1 = (new Vector3(target.transform.position.x, target.transform.position.y - 1, target.transform.position.z) - Arms[1].position).normalized;
                //Arms[0].forward = (tPos - Arms[0].transform.position).normalized;
                Debug.Log(tPos0);
                if (tPos0.x <= 0 && tPos0.z <= 0)
                {
                    Arms[0].localRotation = Quaternion.Euler(Mathf.Atan2(-tPos0.y, -tPos0.x) * Mathf.Rad2Deg, 0, 0);
                }
                else if (tPos0.x >= 0 && tPos0.z <= 0)
                {
                    Arms[0].localRotation = Quaternion.Euler(Mathf.Atan2(-tPos0.y, -tPos0.z) * Mathf.Rad2Deg, 0, 0);
                }
                else if (tPos0.x <= 0 && tPos0.z >= 0)
                {
                    Arms[0].localRotation = Quaternion.Euler(Mathf.Atan2(-tPos0.y, tPos0.z) * Mathf.Rad2Deg, 0, 0);
                }
                else if (tPos0.x >= 0 && tPos0.z >= 0)
                {
                    Arms[0].localRotation = Quaternion.Euler(Mathf.Atan2(-tPos0.y, tPos0.x) * Mathf.Rad2Deg, 0, 0);
                }
                
                Arms[1].localRotation = Quaternion.Euler(Mathf.Atan2(-tPos1.y, -tPos1.z) * Mathf.Rad2Deg, 0, 0);

                Debug.DrawRay(Arms[0].position, Arms[0].forward * 10, Color.red);                
                break;
            case State.HIT:
                if (health <= 0) ChangeState(State.DEATH);
                if (hittedByAR) hittedByAR = false;
                break;
            case State.STUNNED:
                break;
            case State.DEATH:                
                break;
        }
    }

    void ChangeState(State newState)
    {
        switch (currentState)
        {
            case State.CHASE:                
                CancelInvoke("GoToTarget");
                agent.enabled = false;
                anim.SetBool("Moving", false);
                break;
            case State.ATTACK:
                rb.constraints = RigidbodyConstraints.None;
                obstacle.enabled = false;
                CancelInvoke("InstanceBullet");
                break;
            case State.HIT:
                rb.constraints = RigidbodyConstraints.None;
                obstacle.enabled = false;
                break;
            case State.STUNNED:
                anim.SetBool("Stunned", false);
                rb.constraints = RigidbodyConstraints.None;
                obstacle.enabled = false;
                break;
            case State.DEATH:
                gameObject.SetActive(false);
                break;
        }

        switch (newState)
        {
            case State.CHASE:
                Arms[0].localRotation = Quaternion.Euler(90, 0, 0);
                Arms[1].localRotation = Quaternion.Euler(90, 0, 0);
                anim.SetBool("Moving", true);
                agent.enabled = true;
                InvokeRepeating("GoToTarget", 0, repathTime);
                break;
            case State.ATTACK:                
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePosition;
                obstacle.enabled = true;
                InvokeRepeating("InstanceBullet", 0.3f, fireRate);
                break;
            case State.HIT:
                rb.constraints = RigidbodyConstraints.FreezeAll;
                obstacle.enabled = true;
                if (hittedByAR) GameManager.instance.player.IncreaseCash(punishHitIncome);
                else GameManager.instance.player.IncreaseCash(hitIncome);
                Invoke("ChangeToChase", hitTime);
                break;
            case State.STUNNED:
                anim.SetBool("Stunned", true);
                rb.constraints = RigidbodyConstraints.FreezeAll;
                obstacle.enabled = true;
                Invoke("ChangeToChase", empTimeStun);
                break;
            case State.DEATH:
                GameManager.instance.player.IncreaseCash(killIncome);
                GameManager.instance.roundController.DecreaseEnemyCount();
                Invoke("DisableEnemy", deathTime);
                break;
        }

        currentState = newState;
    }

    void DisableEnemy()
    {
        gameObject.SetActive(false);
    }

    public void TakeDamage(float damage/*, GameObject attacker*/)
    {
        if (currentState == State.DEATH) return;
        health -= damage;
        if (health <= 0) ChangeState(State.DEATH);
        else ChangeState(State.HIT);
        /*if (attacker.GetType() != typeof(PlayerController))
        {
            target = attacker;
        }   */
        
    }

    void IncrementStats()
    {
        health += healthInc * (GameManager.instance.roundController.currentRound - 1);
        speed += speedInc * (GameManager.instance.roundController.currentRound - 1);
        if (speed > maxSpeed) speed = maxSpeed;
        damage += damageInc * (GameManager.instance.roundController.currentRound - 1);
        if (damage > maxDamage) damage = maxDamage;
    }

    float DistanceToTarget(GameObject me, GameObject target)
    {
        return (target.transform.position - me.transform.position).magnitude;
    }

    float DistanceToTargetSquared(GameObject me, GameObject target)
    {
        return (target.transform.position - me.transform.position).sqrMagnitude;
    }

    void GoToTarget()
    {
        target = FindInstanceWithinRadius(gameObject, "Player", "AirTurret", "GroundTurret", targetRadiusDetection);
        agent.destination = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
    }

    void ChangeToChase()
    {
        ChangeState(State.CHASE);
    }

    public void ActivateStun(float time)
    {
        empTimeStun = time;
        ChangeState(State.STUNNED);
    }

    bool PlayerHitLeft()
    {
        RaycastHit rayHit;
        Ray playerRay = new Ray();
        playerRay.origin = Cannons[1].position;
        playerRay.direction = player.transform.position - Cannons[1].position;
        if (Physics.Raycast(playerRay, out rayHit, maxDistAttack + 10, mask.value))
        {
            return true;
        }
        //Debug.DrawRay(playerRay.origin, playerRay.direction * 50, Color.blue);
        return false;
    }

    bool PlayerHitRight()
    {
        RaycastHit rayHit;
        Ray playerRay = new Ray();
        playerRay.origin = Cannons[0].position;
        playerRay.direction = player.transform.position - Cannons[0].position;
        if (Physics.Raycast(playerRay, out rayHit, maxDistAttack + 10, mask.value))
        {
            return true;
        }
        //Debug.DrawRay(playerRay.origin, playerRay.direction * 50, Color.blue);
        return false;
    }

    void InstanceBullet()
    {
        GameObject b;
        if (rightCannon)
        {
            b = Instantiate(bullet, Cannons[0].position, Quaternion.identity);
            //b = gc.objectPoolerManager.tankEnemyBulletOP.GetPooledObject
            //b.transform.position = Cannons[0].position;
            b.transform.forward = Arms[0].forward;
            //b.SetActive(true);
        }
        else
        {
            b = Instantiate(bullet, Cannons[1].position, Quaternion.identity);
            //b = gc.objectPoolerManager.tankEnemyBulletOP.GetPooledObject
            //b.transform.position = Cannons[1].position;
            b.transform.forward = Arms[1].forward;
            //b.SetActive(true);
        }

        b.GetComponent<TankBullet>().damage = damage;
        rightCannon = !rightCannon;
    }

    public static GameObject FindInstanceWithinRadius(GameObject me, string tag, string tag2, string tag3, float radius)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag).Concat(GameObject.FindGameObjectsWithTag(tag2)).Concat(GameObject.FindGameObjectsWithTag(tag3)).ToArray();

        if (targets.Length == 0) return null;

        float dist = 0;
        GameObject closest = targets[0];
        float minDistance = (closest.transform.position - me.transform.position).magnitude;

        for (int i = 1; i < targets.Length; i++)
        {
            dist = (targets[i].transform.position - me.transform.position).magnitude;
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = targets[i];
            }
        }

        if (minDistance < radius) return closest;
        else return null;
    }
}
