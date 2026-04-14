using UnityEngine;

public class ArrowHint : MonoBehaviour
{
    [SerializeField] Vector2 pos1;
    [SerializeField] Vector2 pos2;
    [SerializeField] GameObject bugA;
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
        
        if (!bugA.activeSelf && !bugB.activeSelf)
            bugsPlaced = true;

        
        if (Draggable.IsDragging && !bugsPlaced)
        {
            
            if (Draggable.nowBugType == E_BugType.A)
                transform.position = pos1;
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