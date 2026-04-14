using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbacusAnim : MonoBehaviour
{
    
    [Header("算盘珠")]
    [SerializeField] Transform ball;

    [Header("音效")]
    [SerializeField] AudioClip endSound;
    
    [Header("起点和终点")]
    [SerializeField] Vector3 startPoint;
    [SerializeField] Vector3 endPoint;

    [Header("珠子移速")]
    [SerializeField] float speed = 5f;

    public static bool Finsihed = false;
    private bool isMoving = false;
    private int clickCount = 0;

    void OnMouseDown()
    {
        if (isMoving) return; 
        clickCount++;
        bool toEnd = (clickCount % 2 == 1); 
        Vector3 target = toEnd ? endPoint : startPoint;
        StartCoroutine(MoveTo(target));
    }

    IEnumerator MoveTo(Vector3 target)
    {
        isMoving = true;
        AudioMgr.Instance.PlaySFX(endSound);
        while (Vector3.Distance(ball.position, target) > 0.01f)
        {
            ball.position = Vector3.MoveTowards(ball.position, target, speed * Time.deltaTime);
            yield return null;
        }
        ball.position = target;
        isMoving = false;
        Finsihed = true;
    }

    
    
}
