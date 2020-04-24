﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    public enum State { INITIAL, PREPARATION, SPAWN, FIGHT, CLEAR};
    public State currentState = State.INITIAL;

    [Header("General")]
    public int maxEnemies;
    public int maxPeaks;
    public int numMaps; //currentRound % numMaps = mapNumber

    [Header("Timers")]
    public float preparationTime;
    public float spawnTime;
    public float fightTime;

    [Header("Debug")]
    public int currentPeak;
    public int currentRound;

    private int currentEnemies; //Total enemies on screen
    private int minEnemies; //Change depending on peak
    private int extraEnemies; //Number of enemies that could not be spawned due to totalEnemies > maxEnemies

    private float elapsedTime;


    // Start is called before the first frame update
    void Start()
    {
        currentRound = 0;
        currentPeak = 0;
        elapsedTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.INITIAL:
                ChangeState(State.PREPARATION);
                break;
            case State.PREPARATION:
                if (elapsedTime >= preparationTime) ChangeState(State.SPAWN);
                break;
            case State.SPAWN:
                if (elapsedTime >= spawnTime) ChangeState(State.FIGHT);
                break;
            case State.FIGHT:
                if (elapsedTime >= fightTime)
                {
                    if (currentPeak < maxPeaks) ChangeState(State.SPAWN);
                    else ChangeState(State.CLEAR);
                }
                break;
            case State.CLEAR:
                if (currentEnemies <= 0) ChangeState(State.PREPARATION);
                break;
        }

        elapsedTime += Time.deltaTime;
    }

    void ChangeState(State newState)
    {
        // exit logic
        switch (currentState)
        {
            case State.INITIAL:
                break;
            case State.PREPARATION:
                break;
            case State.SPAWN:
                break;
            case State.FIGHT:
                break;
            case State.CLEAR:
                break;
        }

        // enter logic
        switch (newState)
        {
            case State.INITIAL:
                break;
            case State.PREPARATION:
                currentPeak = 1;
                currentRound++;
                break;
            case State.SPAWN:
                currentPeak++;
                break;
            case State.FIGHT:
                break;
            case State.CLEAR:
                break;
        }

        elapsedTime = 0;
        currentState = newState;
    }
}
