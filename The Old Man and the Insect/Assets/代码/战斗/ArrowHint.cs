using UnityEngine;

public class ArrowHint : MonoBehaviour
{
    [SerializeField] Vector2 pos1;
    [SerializeField] Vector2 pos2;
    [SerializeField] Vector2 pos3;
    [SerializeField] GameObject bugA1;
    [SerializeField] GameObject bugA2;
    [SerializeField] GameObject bugB;
    private SpriteRenderer SR;
    private bool bugsPlaced = false;  

    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        SetAlpha(0f);
    }

    void Update()
    {
        
        if (!bugA1.activeSelf && !bugB.activeSelf)
            bugsPlaced = true;

        
        if (Draggable.IsDragging && !bugsPlaced)
        {
            
            if (Draggable.nowBug == bugA1)
                transform.position = pos1;
            else if (Draggable.nowBug == bugA2)
                transform.position = pos3;
            else if (Draggable.nowBugType == E_BugType.B)
                transform.position = pos2;
            else
                return;

            SetAlpha(1f);
        }
        else
        {
           
            SetAlpha(0f);
        }
    }

    void SetAlpha(float alpha)
    {
        Color color = SR.color;
        color.a = alpha;
        SR.color = color;
    }
}