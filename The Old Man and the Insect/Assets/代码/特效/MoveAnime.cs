using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnime : MonoBehaviour
{
    
    [SerializeField]Vector3 StartPos;
    [SerializeField]Vector3 EndPos;
    [SerializeField]AnimationCurve moveCurve;
    [SerializeField]  float duration = 1f;
    public static bool FinishAnim = false;
    
    void Start()
    {
        transform.position = StartPos;
        StartCoroutine(MoveAnim());
    }

    
    void Update()
    {
        
    }

    private IEnumerator MoveAnim()
    {
        //呱：这里是为了等待这个遮幕消失
        yield return new WaitForSeconds(1f);

        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(StartPos, EndPos, moveCurve.Evaluate(timer / duration));
            yield return null;
        }
        
        transform.position = EndPos;
        yield return new WaitForSeconds(0.3f);

        FinishAnim = true;
    }
    
}
