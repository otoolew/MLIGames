using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PatrolTask : AITask
{
    private AICharacter character;

    [SerializeField] private float sequenceTime;
    public float SequenceTime { get => sequenceTime; set => sequenceTime = value; }

    public override void Init(MonoBehaviour runner)
    {
        this.character = runner.GetComponent<AICharacter>();
        
    }
    public override void DelegateTask()
    {
        Debug.Log("DelegateTask IdleRoutine -> " + character.gameObject.name);
        character.StartCoroutine(TaskCoroutine());
    }
    public override IEnumerator TaskCoroutine()
    {
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            Debug.Log(character.name + "Waiting...");
            normalizedTime += Time.deltaTime / sequenceTime;
            yield return null;

          
        }

        Debug.Log(character.name + "Done Waiting...");        
    }
    public Func<bool> OnComplete()
    {

        return ()=> true;

    }
    public static IdleTask Create()
    {
        return ScriptableObject.CreateInstance<IdleTask>();
    }


}
