using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable] 
public enum TimerStatus { RUNNING, COMPLETE }

[Serializable]
public class Timer
{
    [SerializeField] private TimerStatus status;
    public TimerStatus Status { get => status; set => status = value; }

    [SerializeField] private float startCount;
    public float StartCount { get => startCount; set => startCount = value; }

    [SerializeField] private float currentCount;
    public float CurrentCount { get => currentCount; set => currentCount = value; }

    [SerializeField] private bool loopTimer;
    public bool LoopTimer { get => loopTimer; set => loopTimer = value; }

    public bool IsFinished { get { if (currentCount < 0) return true; else return false; } }

    public UnityAction TimerCompleteAction;

    public Timer()
    {
        currentCount = startCount;
        status = TimerStatus.RUNNING;
        TimerCompleteAction = DefaultTimerAction;
    }

    public Timer(float startCount)
    {
        this.startCount = startCount;
        currentCount = startCount;
        status = TimerStatus.RUNNING;
        TimerCompleteAction = DefaultTimerAction;
    }

    public void Tick()
    {
        if(currentCount > 0)
        {
            currentCount -= Time.deltaTime;
        }
        else
        {
            //TimerCompleteAction.Invoke();

            if (loopTimer)
            {
                ResetTimer();
            }
            else
            {
                status = TimerStatus.COMPLETE;
            }
        }
    }

    public void ResetTimer()
    {
        currentCount = startCount;
        status = TimerStatus.RUNNING;
    }

    public void ResetTimer(float value)
    {
        startCount = value;
        currentCount = startCount;
        status = TimerStatus.RUNNING;
    }

    private void DefaultTimerAction()
    {
        Debug.Log("No Action assigned to timer.");
    }
}
