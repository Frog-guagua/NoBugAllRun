using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    #region 玩家

    [Header("音效")]
    public AudioClip MovingSound;
    
    [Header("玩家の步长设置（速度）")]
    [SerializeField] float PlayerMoveLength_X = 0;
    [SerializeField] float PlayerMoveLength_Y = 0;
    
    private Transform PlayerTransform;
    public static bool IsMove = false;
    private bool lastIsMove = false;
    private Animation PlayerAnimation;
    private Animator playerAnimator;
    public static bool canMove = true;

    [Header("主角初始面朝向")] public E_FirstPlayerState firststate;
    public enum E_FirstPlayerState
    {
        front,
        back,
        towardsLeft,
        towardsRight
    }
    
    // 2D 刚体组件
    private Rigidbody2D rb;

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
    
    // 用于在 Update 和 FixedUpdate 之间传递输入
    private Vector2 moveInput;

    void Start()
    {
        #region 初始化玩家组件 数据
        PlayerTransform = transform;
        PlayerAnimation = PlayerTransform.GetComponent<Animation>();
        originalScale = PlayerTransform.localScale;
        playerAnimator = GetComponent<Animator>();
        if (playerAnimator != null)
            playerAnimator.applyRootMotion = false;

        // 获取或添加 Rigidbody2D 组件
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        NormalizeMoveSpeedSettings();

     
        rb.simulated = true;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        #endregion

        switch (firststate)
        {
            case E_FirstPlayerState.front:
                playerAnimator.SetBool("IsMoving", false);
                playerAnimator.SetBool("IsSide", false);
                playerAnimator.SetFloat("MoveY", -1f);
                playerAnimator.Play("主角正面待机");
                playerAnimator.Update(0);
                break;
            case E_FirstPlayerState.back:
                playerAnimator.SetBool("IsMoving", false);
                playerAnimator.SetBool("IsSide", false);
                playerAnimator.SetFloat("MoveY", 1f);
                playerAnimator.Play("主角背面待机");
                playerAnimator.Update(0);
                break;
            case E_FirstPlayerState.towardsLeft:
                playerAnimator.SetBool("IsMoving", false);
                playerAnimator.SetBool("IsSide", true);
                playerAnimator.SetFloat("MoveY", 0f);
                playerAnimator.Play("主角侧面待机");
                playerAnimator.Update(0);
                break;
            case E_FirstPlayerState.towardsRight:
                playerAnimator.SetBool("IsMoving", false);
                playerAnimator.SetBool("IsSide", true);
                playerAnimator.SetFloat("MoveY", 0f);
                playerAnimator.Play("主角侧面待机");
                playerAnimator.Update(0);
                this.gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
                break;
        }
    }

    void Update()
    { 
        PosUpdate();
        HandleInput();          // 检测输入并更新朝向、动画状态
        MovingSfx();
    }

    void FixedUpdate()
    {
        // 物理移动在固定时间步长中执行
        PerformMove();
    }

    void LateUpdate()
    {
        SyncTransformToRigidbody();
    }

    /// <summary>
    /// 检测玩家输入，记录移动方向，同时更新动画和朝向（这些不依赖物理时间步）
    /// </summary>
    void HandleInput()
    {
        if (!canMove)
        {
            moveInput = Vector2.zero;
            IsMove = false;
            playerAnimator.SetBool("IsMoving", false);
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            vertical = 1f;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            vertical = -1f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontal = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontal = 1f;

        moveInput = new Vector2(horizontal, vertical);

        // 判断是否在移动
        IsMove = moveInput != Vector2.zero;

        // 更新动画状态
        playerAnimator.SetBool("IsMoving", IsMove);

        // 根据移动方向更新玩家状态和朝向
        if (IsMove)
        {
            // 左右优先，还是上下优先？这里保持原逻辑：只要有左右输入就覆盖上下输入的状态
            if (horizontal != 0)
            {
                playerState = horizontal > 0 ? E_PlayerState.TowardsRight : E_PlayerState.TowardsLeft;
                SetSideTowards(playerState);
            }
            else if (vertical != 0)
            {
                playerState = vertical > 0 ? E_PlayerState.Back : E_PlayerState.Front;
            }

            // 设置动画参数
            bool isSide = (playerState == E_PlayerState.TowardsLeft || playerState == E_PlayerState.TowardsRight);
            playerAnimator.SetBool("IsSide", isSide);

            float moveY = 0f;
            if (playerState == E_PlayerState.Back)
                moveY = 1f;
            else if (playerState == E_PlayerState.Front)
                moveY = -1f;
            playerAnimator.SetFloat("MoveY", moveY);
        }
    }

    /// <summary>
    /// 执行基于 Rigidbody2D.MovePosition 的物理移动
    /// </summary>
    void PerformMove()
    {
        if (moveInput == Vector2.zero)
            return;

        // 使用 fixedDeltaTime 保证物理步长下的恒定速度
        float deltaX = moveInput.x * PlayerMoveLength_X * Time.fixedDeltaTime;
        float deltaY = moveInput.y * PlayerMoveLength_Y * Time.fixedDeltaTime;

        Vector2 currentPos = rb.position;
        Vector2 targetPos = currentPos + new Vector2(deltaX, deltaY);

        rb.MovePosition(targetPos);
    }

    void SyncTransformToRigidbody()
    {
        if (rb == null || !rb.simulated)
            return;

        Vector3 p = PlayerTransform.position;
        Vector2 rp = rb.position;
        if (!Mathf.Approximately(p.x, rp.x) || !Mathf.Approximately(p.y, rp.y))
            PlayerTransform.position = new Vector3(rp.x, rp.y, p.z);
    }

    void NormalizeMoveSpeedSettings()
    {
        const float fallbackSpeed = 3f;

        bool xInvalid = PlayerMoveLength_X <= 0f;
        bool yInvalid = PlayerMoveLength_Y <= 0f;

        if (xInvalid && yInvalid)
        {
            PlayerMoveLength_X = fallbackSpeed;
            PlayerMoveLength_Y = fallbackSpeed;
            return;
        }

        if (xInvalid)
            PlayerMoveLength_X = Mathf.Max(PlayerMoveLength_Y, fallbackSpeed);
        if (yInvalid)
            PlayerMoveLength_Y = Mathf.Max(PlayerMoveLength_X, fallbackSpeed);
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
