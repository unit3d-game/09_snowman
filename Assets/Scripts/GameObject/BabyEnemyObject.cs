using System;
using MyUtils;
using UnityEngine;

public class BabyEnemyObject : BaseNotificationBehaviour
{

    /**
     * <summary>雪球状态</summary>
     * 
     */
    public GameObject BallPrefab;

    /**
     * <summary>速度</summary>
     */
    public float Speed = 3;

    /// <summary>
    /// 糖果预制体
    /// </summary>
    public GameObject SweetPrefab;

    // 移动动画设置
    private AnimSetter<bool> isMoveSetter;
    // 是否脚踏实地
    private AnimSetter<bool> isGroundSetter;
    // 速度动画设置
    private AnimSetter<float> speedSetter;

    private Transform footPosition;

    private bool isGround = false;


    private Rigidbody2D rbody;

    private AiWalkObject aiWalkObject;


    public override void Awake()
    {
        base.Awake();
        Animator animator = GetComponent<Animator>();
        aiWalkObject = GetComponent<AiWalkObject>();
        rbody = GetComponent<Rigidbody2D>();
        isMoveSetter = new AnimSetter<bool>(animator, "Move");
        isGroundSetter = new AnimSetter<bool>(animator, "IsGround");
        speedSetter = new AnimSetter<float>(animator, "Speed");
        footPosition = transform.Find("Foot");
    }

    public void DoStart(float mspeed)
    {
        Speed = mspeed;
    }

    public void DoStart(int force, float mspeed)
    {
        Speed = mspeed;
        rbody.AddForce(new Vector2(-1, UnityEngine.Random.Range(0, 1f)) * force);
    }

    private void Update()
    {
        setGround();
        toWalk();
    }

    private void setGround()
    {
        isGround = SnowUtils.IsCollision(footPosition, 0.1f, Const.Layer.Stair, Const.Layer.Floor);
    }

    private void toWalk()
    {
        // 如果不在地上
        if (!isGround)
        {
            if (isGroundSetter.Get())
            {
                //speedSetter.Set(0)
                isGroundSetter.Set(false);
            }
            return;
        }
        // 是否需要启动
        if (!aiWalkObject.IsMoving)
        {
            aiWalkObject.DoStart(Speed, transform.localScale.x > 0);
        }
        if (!isGroundSetter.Get())
        {
            //speedSetter.Set(0)
            isGroundSetter.Set(true);
        }
        // 如果是移动
        if (aiWalkObject.IsMoving)
        {
            // 检查动画是否设置
            if (!isMoveSetter.Get())
            {
                speedSetter.Set(1);
                isMoveSetter.Set(true);
            }
        }
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public void DoDeath()
    {
        // 开始
        Instantiate(SweetPrefab, transform.position, transform.rotation, transform.parent);
        Destroy(gameObject);
    }

    private void toBall()
    {
        Instantiate(BallPrefab, transform.position, transform.rotation, transform.parent);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果碰到子弹，则变成雪球
        if (collision.tag == Const.Tag.Bullet)
        {
            toBall();
        }
    }



    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == Const.Tag.Ball)
    //    {
    //        // 检查是否是 rolling 状态
    //        BallObject ball = collision.gameObject.GetComponent<BallObject>();
    //        if (!ball.isRolling)
    //        {
    //            return;
    //        }
    //        // 开始
    //        Instantiate(SweetPrefab, transform.position, transform.rotation, transform.parent);
    //        Destroy(gameObject);
    //    }
    //}

}

