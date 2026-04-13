using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//呱：写在最前面 ——> 这个脚本挂在玩家身上
public class PlayerMove : MonoBehaviour
{
    #region 玩家

    [Header("音效")]
    public AudioClip MovingSound;
    
    [Header("玩家の步长设置")]
    [SerializeField] float PlayerMoveLength_X = 0;
    [SerializeField] float PlayerMoveLength_Y = 0;
    private Transform PlayerTransform;
    public static bool IsMove = false;
    private bool lastIsMove = false;
    private Animation PlayerAnimation;
 
    
    
    private Animator playerAnimator;
    
    
    public static bool canMove = true;//只有在true玩家才能进行移动，避免在对话时玩家还能移动角色
    #endregion

    enum E_PlayerState
    {
        Front,
        Back,
        TowardsLeft,
        TowardsRight,
    }
    E_PlayerState playerState = E_PlayerState.TowardsLeft;
    private Vector3 originalScale;
    

    void Start()
    {
        #region 初始化玩家组件 数据
        PlayerTransform = transform;
        PlayerAnimation = PlayerTransform.GetComponent<Animation>();
        originalScale = PlayerTransform.localScale;


        playerAnimator = GetComponent<Animator>();
        #endregion
    }


    void Update()
    { 
        PosUpdate();
        Move();
        MovingSfx();
    }

    void MovingSfx()
    {
        if (IsMove != lastIsMove)
        {
            if (IsMove)
                MovingSfxStart();
            else
                MovingSfxStop();
            lastIsMove = IsMove;
        }
    }
    
    void MovingSfxStart()
    {
        AudioMgr.Instance.LoopSFX(MovingSound);
    }

    void MovingSfxStop()
    {
        AudioMgr.Instance.StopSFX(MovingSound);
    }
    
    
    bool Move()
    {
        float newX = PlayerTransform.position.x;
        float newY = PlayerTransform.position.y;
       
        
        
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))&&canMove)
        {
            
            newY = PlayerTransform.position.y + PlayerMoveLength_Y * Time.deltaTime;
            IsMove = true;
            playerState = E_PlayerState.Back;
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))&&canMove)
        {
            newY = PlayerTransform.position.y - PlayerMoveLength_Y * Time.deltaTime;
            IsMove = true;
            playerState = E_PlayerState.Front;
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))&&canMove)
        {
            newX = PlayerTransform.position.x - PlayerMoveLength_X * Time.deltaTime;
            IsMove = true;
            playerState = E_PlayerState.TowardsLeft;
            SetSideTowards(E_PlayerState.TowardsLeft);
        }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))&&canMove)
        {
            newX = PlayerTransform.position.x + PlayerMoveLength_X * Time.deltaTime;
            IsMove = true;
            playerState = E_PlayerState.TowardsRight;
            SetSideTowards(E_PlayerState.TowardsRight);
        }
        else
        {
            IsMove = false;
        }

        PlayerTransform.position = new Vector3(newX, newY, PlayerTransform.position.z);

       
        
            playerAnimator.SetBool("IsMoving", IsMove);
    
         
            bool isSide = (playerState == E_PlayerState.TowardsLeft || playerState == E_PlayerState.TowardsRight);
            playerAnimator.SetBool("IsSide", isSide);
    
           
            float moveY = 0f;
            if (playerState == E_PlayerState.Back)
                moveY = 1f;
            else if (playerState == E_PlayerState.Front)
                moveY = -1f;
       
            playerAnimator.SetFloat("MoveY", moveY);

        return IsMove;
    }

    void SetSideTowards(E_PlayerState targetState)
    {
        if (targetState == E_PlayerState.TowardsRight)
        {
            PlayerTransform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }
        else if (targetState == E_PlayerState.TowardsLeft)
        {
            PlayerTransform.localScale = originalScale;
        }
    }
    
    void PosUpdate()
    {
        PlayerTransform = transform;
    }
}