using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageZoom : MonoBehaviour
{
    [SerializeField] float ZoomTimes;
    [SerializeField] Vector3 GoalPos;
    [SerializeField] Collider2D CageCollider;
    
    private Vector3 originalPos;
    private Vector3 originalScale;
    private bool isZoomed = false;
    public static bool CageHasZoomed;
    
    
    [SerializeField]  List<Draggable> draggableList;
  
    void Start()
    {
        originalPos = transform.position;
        originalScale = transform.localScale;
        // 初始状态：未放大，虫子不可拖拽
        CageHasZoomed = false;
        BanDrag();
    }
    
    void Update()
    {
        ClickDetection();
    }

    void NewPosition()
    {
        transform.position = GoalPos;
        transform.localScale = originalScale * ZoomTimes;
        isZoomed = true;
        CageHasZoomed = true;
        ReleaseDrag();
    }

    public void OriginPos()
    {
        transform.position = originalPos;
        transform.localScale = originalScale;
        isZoomed = false;
        CageHasZoomed = false;
            
    }
    
    void ClickDetection()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider == CageCollider)
            {
                if (!isZoomed ) 
                {
               
                    NewPosition();
                }
                else
                {
                    BanDrag(); 
                    OriginPos();
                }
            }
            
            
           
        }
    
        
    }
    
        
    void ReleaseDrag()
    {
        foreach (var draggable in draggableList)
        {
            if (draggable != null) draggable.enabled = true;  
        }          
                
    }
        
    void BanDrag()
    {
        foreach (var draggable in draggableList)
        {
            if (draggable != null) draggable.enabled = false;  
        }  
    }
}