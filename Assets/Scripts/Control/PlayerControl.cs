using System;
using System.Collections;
using System.Collections.Generic;
using MyUtils;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PlayerControl : BaseNotificationBehaviour
{

    private static readonly Color[] InvinsibleColors = ArrayUtils.As<Color>(Color.cyan, Color.blue, Color.red, Color.yellow, Color.white);

    /**
     * <summary>无敌维持时间，默认3s</summary>
     */
    public float InvinsibleDuration = 3;

    private AnimTrigger isJumpTrigger;

    private AnimSetter<bool> isGroundSetter;

    private AnimSetter<float> speedSetter;

    private AnimTrigger isSlowTrigger;

    private AnimTrigger isFireTrigger;

    private AnimSetter<bool> isPushSetter;

    private AnimSetter<bool> isSpeedUpSetter;

    private AnimTrigger isDeadTrigger;

    private Rigidbody2D rbody;

    private bool isGround = true;

    private Transform footPosition;

    private Transform pushPosition;

    private bool isMoving = false;

    private GameObject pushBall;

    private bool isDied = false;
    // 剩余无敌时间，如果大于零则表示在无敌状态
    private float invinsibleTime = 0;

    private bool isStarted = false;


    private SpriteRenderer spriteRenderer;

    public override void Awake()
    {
        base.Awake();
        Animator animator = GetComponent<Animator>();
        isJumpTrigger = new AnimTrigger(animator, "Jump");
        isGroundSetter = new AnimSetter<bool>(animator, "IsGround");
        speedSetter = new AnimSetter<float>(animator, "Speed");
        isSlowTrigger = new AnimTrigger(animator, "Slow");
        isFireTrigger = new AnimTrigger(animator, "Fire");
        isPushSetter = new AnimSetter<bool>(animator, "Push");
        isSpeedUpSetter = new AnimSetter<bool>(animator, "SpeedUp");
        isDeadTrigger = new AnimTrigger(animator, "Dead");
        spriteRenderer = GetComponent<SpriteRenderer>();
        isGroundSetter.Set(true);
        rbody = GetComponent<Rigidbody2D>();
        footPosition = transform.Find("Foot");
        pushPosition = transform.Find("PushPoint");
        invinsibleTime = InvinsibleDuration;
    }



    public void DoShoot()
    {
        isFireTrigger.Trigger();
    }



    private void Update()
    {
        if (!isStarted)
        {
            return;
        }

        SetGround();
        // to jump
        toJump();
        // 计算走路
        toWalk();
        //无敌
        toInvinsible();
    }


    public void DoStart()
    {
        isStarted = true;
        isGround = true;
    }

    // 处理无敌状态
    private void toInvinsible()
    {
        // 如果不是无敌状态，则忽略
        if (invinsibleTime == 0)
        {
            return;
        }
        // 如果是无敌状态则处理
        invinsibleTime = invinsibleTime >= Time.deltaTime ? invinsibleTime - Time.deltaTime : 0;
        //如果结束无敌状态则设置成白色
        if (invinsibleTime == 0)
        {
            spriteRenderer.color = Color.white;
            gameObject.layer = LayerMask.NameToLayer(Const.Layer.Player);
        }
        else
        {
            //根据时间来循环显示每0.1秒一个
            int rd = (int)(invinsibleTime * 10) % InvinsibleColors.Length;
            spriteRenderer.color = InvinsibleColors[rd];
            gameObject.layer = LayerMask.NameToLayer(Const.Layer.Invincibility);
        }
    }

    /// <summary>
    /// 跳跃处理
    /// </summary>
    /// <returns></returns>
    private bool toJump()
    {
        if (!Input.GetKeyDown(KeyCode.J) || !isGround)
        {
            return false;
        }
        Debug.Log($"IsGround is {isGround}");
        rbody.AddForce(Vector2.up * 300);
        isJumpTrigger.Trigger();
        isGround = false;
        return true;
    }

    private void FixedUpdate()
    {
        SetGround();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Coll trigger is {collision.tag}");
    }

    private void toWalk()
    {
        float hor = Input.GetAxis(MyUtils.Const.Axis.Horizontal);
        speedSetter.Set(Math.Abs(hor));
        // 如果小于0.1 则忽略
        if (Math.Abs(hor) <= 0.1)
        {
            isMoving = false;
            isSpeedUpSetter.Set(false);
            return;
        }
        isMoving = true;
        isSpeedUpSetter.Set(true);
        // 来个旋转
        Vector3 pos = transform.position;
        // 如果在空中，则移动速度降低为0.5
        pos.x += hor * (isGround ? 2 : 0.5f) * Time.deltaTime;
        transform.position = pos;
        if (hor < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }


    void SetGround()
    {
        isGround = SnowUtils.IsCollision(footPosition, 0.1f, Const.Layer.Floor, Const.Layer.Stair);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 如果是雪球，则改变状态为push
        if (isMoving && collision.gameObject.tag == Const.Tag.Ball)
        {
            Physics2D.Raycast(pushPosition.position, Vector2.right);
            // 再检查是否在范围内
            if (SnowUtils.IsCollision(pushPosition.position, collision.gameObject.transform.position, 0.45f))
            {
                pushBall = collision.gameObject;
                pushBall.GetComponent<BallObject>().DoStartPush();
                isPushSetter.Set(true);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == Const.Tag.Ball)
        {
            isPushSetter.Set(false);
            isMoving = false;
            if (pushBall == collision.gameObject)
            {
                pushBall.GetComponent<BallObject>().DoEndPush();
                pushBall = null;
            }
            return;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 如果是无敌状态，则忽略
        if (invinsibleTime > 0)
        {
            return;
        }

        // 如果是enemy
        if (collision.gameObject.tag == Const.Tag.Enemy)
        {
            if (!isDied)
            {
                isDied = true;
                isDeadTrigger.Trigger();
                //关闭 rbody 和 碰撞
                Destroy(rbody);
                Destroy(GetComponent<CapsuleCollider2D>());
                PostNotification.Post(Const.Event.PlayerDied, this);
            }
            return;
        }
    }

    [Subscribe(Const.Event.Invinsible)]
    public void OnToInvisible(MessagePayload<Vector3> ballPos)
    {
        if (SnowUtils.IsCollision(ballPos.data, 0.1f, Const.Layer.Player))
        {
            invinsibleTime = InvinsibleDuration;
        }
        else
        {

            Debug.Log("我不是无敌状态");
        }
    }

    public void DelayDestroy()
    {
        Destroy(gameObject);
    }

}
