using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum E_State
{
    loop,
    random,
    single
}
//这是一个路径移动的小框架，single完成路线后停止，loop循环路线，random会在路径点随机移动
public class LoopMove : MonoBehaviour
{   
    public float speed;
    public List<GameObject> paths;//用空的gameobject标记路线即可
    public float arrivalThreshold = 0.1f;
    public E_State state = E_State.loop;
    
    private int currentPathIndex = 0;
    private bool isMoving = true;

    void Update()
    {
        if (!isMoving || paths.Count == 0) return;
        
        // 获取当前目标点
        Vector3 targetPosition = paths[currentPathIndex].transform.position;
        
        // 移动到目标点
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition, 
            speed * Time.deltaTime
        );
        
        // 检查是否到达目标点
        if (Vector3.Distance(transform.position, targetPosition) <= arrivalThreshold)
        {
            HandleWaypointReached();
        }
    }

    void HandleWaypointReached()
    {   
        Debug.Log($"到达路径点 {currentPathIndex}: {transform.position}");
        
        switch (state)
        {
            case E_State.loop:
                HandleLoopState();
                break;
                
            case E_State.random:
                HandleRandomState();
                break;
                
            case E_State.single:
                HandleSingleState();
                break;
        }
    }

    void HandleLoopState()
    {
        currentPathIndex++;
        
        if (currentPathIndex >= paths.Count)
        {
            currentPathIndex = 0;
            Debug.Log("重新开始路径循环");
        }
    }

    void HandleRandomState()
    {
        // 避免随机到同一个点
        int newIndex;
        do
        {
            newIndex = Random.Range(0, paths.Count);
        } while (newIndex == currentPathIndex && paths.Count > 1);
        
        currentPathIndex = newIndex;
    }

    void HandleSingleState()
    {
        currentPathIndex++;
        
        if (currentPathIndex >= paths.Count)
        {
            isMoving = false;
            Debug.Log("到达路径终点");
        }
    }

    // 在Scene视图中绘制路径
    void OnDrawGizmos()
    {
        if (paths == null || paths.Count < 2) return;
        
        // 随机状态不绘制路径
        if (state == E_State.random) return;

        Gizmos.color = state == E_State.loop ? Color.cyan : Color.yellow;
        
        for (int i = 0; i < paths.Count; i++)
        {
            if (paths[i] == null) continue;

            // 绘制路径点
            Gizmos.DrawSphere(paths[i].transform.position, 0.2f);

            // 绘制连接线
            if (i < paths.Count - 1 && paths[i + 1] != null)
            {
                Gizmos.DrawLine(
                    paths[i].transform.position,
                    paths[i + 1].transform.position
                );
            }

            // 循环状态下绘制从最后一个点到第一个点的连线
            if (state == E_State.loop && i == paths.Count - 1 && paths[0] != null)
            {
                Gizmos.DrawLine(
                    paths[i].transform.position,
                    paths[0].transform.position
                );
            }
        }
    }
}