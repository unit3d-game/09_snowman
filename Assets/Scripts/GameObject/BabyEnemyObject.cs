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

    /// <summary>
    /// 显示血量
    /// </summary>
    public GameObject ShowHpPrefab;


    private int hp = 5;

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

    private EnemyHPObject showHpO;

    private BossObject boss;


    public override void Awake()
    {
        base.Awake();
        Animator animator = GetComponent<Animator>();
        aiWalkObject = GetComponent<AiWalkObject>();
        rbody = GetComponent<Rigidbody2D>();
        boss = GameObject.Find(Const.ObjectName.Boss).GetComponent<BossObject>();
        isMoveSetter = new AnimSetter<bool>(animator, "Move");
        isGroundSetter = new AnimSetter<bool>(animator, "IsGround");
        speedSetter = new AnimSetter<float>(animator, "Speed");
        footPosition = transform.Find("Foot");
        showHpO = Instantiate(ShowHpPrefab).GetComponent<EnemyHPObject>();
        showHpO.gameObject.transform.SetParent(GameObject.Find("UIMananger").transform.Find("Canvas"));
        showHpO.Init(this.hp, gameObject);
    }

    public void DoStart(float mspeed)
    {
        Speed = mspeed;
        restHP();
    }

    public void DoStart(int force, float mspeed)
    {
        Speed = mspeed;
        rbody.AddForce(new Vector2(-1, UnityEngine.Random.Range(0, 1f)) * force);
        restHP();
    }

    private void restHP()
    {
        // 根据 speed 生成 血条
        this.hp -= (int)((this.Speed - boss.MinSpeed) / (boss.MaxSpeed - boss.MinSpeed) * this.hp);
        this.showHpO.UpdateHp(this.hp);

    }

    private void Update()
    {
        setGround();
        toWalk();
    }

    private void setGround()
    {
        isGround = SnowUtils.IsCollision(footPosition, 0.25f, Const.Layer.Stair, Const.Layer.Floor);
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
        DestroySelf();
    }

    private void toBall()
    {
        Instantiate(BallPrefab, transform.position, transform.rotation, transform.parent);
        DestroySelf();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果碰到子弹，则变成雪球
        if (collision.tag == Const.Tag.Bullet)
        {
            BulletObject bullet = collision.gameObject.GetComponent<BulletObject>();
            hp -= bullet.AttackPower;
            showHpO.UpdateHp(hp);
            PostNotification.Post<int>(Const.Event.IncrementScore, this, bullet.AttackPower * 10);
            if (hp <= 0)
            {
                toBall();
            }
        }
    }

    private void DestroySelf()
    {
        showHpO.DestroySelf();
        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 如果是最下面的左右侧，则消失
        if (collision.gameObject.tag == Const.Tag.Ground && collision.gameObject.name.Contains("Dead"))
        {
            DestroySelf();
        }
    }

}

