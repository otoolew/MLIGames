﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Timer
{
    [SerializeField] private float startCount;
    /// <summary>
    /// Start the countdown at this value is seconds. 
    /// </summary>
    public float StartCount { get => startCount; set => startCount = value; }

    [SerializeField] private float currentCount;
    public float CurrentCount { get => currentCount; set => currentCount = value; }

    public bool IsFinished { get { if (currentCount < 0) return true; else return false; } }

    public UnityAction TimerAction;

    public Timer()
    {
        TimerAction = DefaultTimerAction;
    }

    public Timer(float startCount)
    {
        this.startCount = startCount;
        TimerAction = DefaultTimerAction;
    }

    public void Tick()
    {
        currentCount -= Time.deltaTime;
    }

    public void ResetTimer()
    {
        currentCount = startCount;
    }

    public void ResetTimer(float value)
    {
        startCount = value;
        currentCount = startCount;
    }

    private void DefaultTimerAction()
    {
        Debug.Log("No Action assigned to timer.");
    }
}
