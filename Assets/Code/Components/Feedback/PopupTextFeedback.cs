using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PopupTextFeedback : PooledObject
{
    [SerializeField] private TMP_Text textComp;
    protected TMP_Text TextComp { get => textComp; set => textComp = value; }

    [Header("Time Settings")]
    [SerializeField] private float playDuration;
    protected virtual float PlayDuration { get => playDuration; set => playDuration = value; }

    [SerializeField] private Timer timer;
    public Timer Timer { get => timer; set => timer = value; }

    [Header("Popup Settings")]
    [SerializeField] private float moveSpeed;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    #region Monobehaviour

    private void Awake()
    {
        timer = new Timer(PlayDuration);
    }
    private void OnEnable()
    {
        timer.ResetTimer();
        StartCoroutine(PlayRoutine());
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {
        timer.Tick();
        transform.position += new Vector3(0.0f, MoveSpeed) * Time.deltaTime;
    }

    protected virtual void PlayCompleted()
    {
        Repool();
    }

    IEnumerator PlayRoutine()
    {
        yield return new WaitUntil(() => timer.IsFinished);
        PlayCompleted();
    }
    protected override void Repool()
    {
        base.Repool();
    }
    #endregion
}
