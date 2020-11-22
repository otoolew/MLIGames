using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFeedbackComponent : PoolComponent
{
    [SerializeField] private PopupTextFeedback feedbackPrefab;
    public PopupTextFeedback FeedbackPrefab { get => feedbackPrefab; set => feedbackPrefab = value; }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayFeedback()
    {

    }
}
