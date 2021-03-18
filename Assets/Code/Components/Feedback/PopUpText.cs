using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PopUpText : MonoBehaviour, IPoolable
{
    [SerializeField] private TextMesh textComp;
    protected TextMesh TextComp { get => textComp; set => textComp = value; }

    [Header("Time Settings")]
    [SerializeField] private float playDuration;
    protected virtual float PlayDuration { get => playDuration; set => playDuration = value; }

    [SerializeField] private Timer timer;
    public Timer Timer { get => timer; set => timer = value; }

    [Header("Popup Settings")]
    [SerializeField] private float moveSpeed;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    public GameObject GameObject => gameObject;

    //public Transform PoolTransform { get; set; }

    public UnityAction OnPlayCompleted;

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
        //transform.LookAt(Camera.main.transform, -Vector3.right);
    }
    public void ChangeText(int value)
    {
        if (value < 0)
        {
            if (ColorUtility.TryParseHtmlString("#FF0000", out Color color))
            {
                textComp.color = color;
            }
        }
        else if (value > 0)
        {
            if (ColorUtility.TryParseHtmlString("#3CDE46", out Color color))
            {
                textComp.color = color;
            }
        }
        else
        {
            if (ColorUtility.TryParseHtmlString("#FFFFFF", out Color color))
            {
                textComp.color = color;
            }
        }

        textComp.text = value.ToString();
    }
    public void ChangeText(int value, Color color)
    {
        textComp.color = color;
        textComp.text = value.ToString();
    }
    public void ChangeText(string value)
    {
        textComp.text = value;
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
    IEnumerator PlayRoutine2()
    {
        yield return new WaitUntil(() => timer.IsFinished);
        PlayCompleted();
    }
    public void Repool()
    {
        GameAssetManager.Instance.PopUpPool.ReturnToPool(this);
    }
    #endregion
}
