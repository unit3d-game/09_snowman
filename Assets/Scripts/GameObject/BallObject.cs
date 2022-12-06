using UnityEngine;
using MyUtils;

/**
 * 雪球逻辑：
 * 1、HP = 3 之前是不能被碰撞和移动的（除了子弹可以），而且hp * 5秒内会复活
 * 2、人物站在雪球上时，是可以推动和攻击的，攻击（按K）的话会使其拥有 6 的速度向前方快速滚动
 * 3、雪球可以推动人物运动（人物进入无敌状态），如果此时跳跃（按J）就会脱离跟随状态，人物同时进入无敌状态
 * 4、碰到Wall和BOSS则消失
 * 5、HP是1、2的时候，人物是碰触不到的
 * 
 * 药水的逻辑：
 * 1、蓝色: 增加子弹效果，效果增加一倍
 * 2、绿色：玩家进入无敌状态
 * 3、红色：玩家速度提升一倍
 * 4、黄色:延长子弹飞行时间，增加50%
 * 
 */
public class BallObject : MonoBehaviour
{

    /**
     * <summary>每个hp衰减的时长，默认5秒</summary>
     * 
     */
    public float HpDuration = 10;

    /// <summary>
    /// 药水
    /// </summary>
    public GameObject[] Waters;

    /**
     * <summary></summary>
     */
    public float RollSpeed = 6;

    public const int MAX_HP = 3;

    private int hp = 1;

    private AnimSetter<int> hpSetter;

    private AnimSetter<bool> isRollingSetter;


    private float currentCountdown = 0;

    // 是否是推动状态
    private bool isPushing = false;

    // 是否是滚动状态
    public bool isRolling { private set; get; } = false;


    private GameObject babyEnemyPrefab;

    private AiWalkObject aiWalkObject;

    private GameObject player;

    private Transform playerFoot;

    private PlatformEffector2D platformEffector2D;




    private bool isDestroyed = false;

    private void Awake()
    {
        Animator animator = GetComponent<Animator>();
        platformEffector2D = GetComponent<PlatformEffector2D>();
        hpSetter = new AnimSetter<int>(animator, "hp");
        isRollingSetter = new AnimSetter<bool>(animator, "Rolling");
        babyEnemyPrefab = GameObject.Find(Const.ObjectName.Boss).GetComponent<BossObject>().EnemyPrefab;
        aiWalkObject = GetComponent<AiWalkObject>();
        player = GameObject.Find("Player");
        playerFoot = player.transform.Find("Foot");

    }

    void Update()
    {
        // 是否需要复活成小怪兽
        toBabyEnemy();
        DoAttack();
    }

    private void FixedUpdate()
    {
        checkCollisionBoss();
        checkCollisionEnemy();
    }

    // 复活小怪兽
    private void toBabyEnemy()
    {
        // 如果是在推动或者滚动状态下，则倒计时关闭
        if (isPushing || isRolling || isDestroyed)
        {
            return;
        }

        currentCountdown += Time.deltaTime;
        // 如果到达最大时长，则hp --
        if (currentCountdown >= HpDuration)
        {
            setHp(hp - 1);
        }
        // 如果hp =0 表示可以复活小怪兽了
        if (hp == 0)
        {
            // 产生一个新的小怪兽
            GameObject enemy = Instantiate(babyEnemyPrefab, transform.position, transform.rotation, transform.parent);

            isDestroyed = true;
            Debug.Log($"Clear ball game object from birth enemy");
            // 销毁当前雪球
            Destroy(gameObject);
        }
    }


    // 是否是激活状态，即可以推动
    public bool IsActive()
    {
        return hp == MAX_HP;
    }

    /**
     * <summary>开始推着走</summary>
     * <returns>是否设置成功</returns>
     */
    public void DoStartPush()
    {
        if (!IsActive())
        {
            return;
        }
        isPushing = true;
        isRollingSetter.Set(true);
    }

    /**
     * <summary>结束 push</summary>
     */
    public void DoEndPush()
    {
        isPushing = false;
        isRollingSetter.Set(false);
    }

    /**
     * <summary>攻击雪球</summary>
     * <returns>是否设置成功</returns>
     */
    private void DoAttack()
    {
        if (!IsActive() || isRolling)
        {
            return;
        }
        // 是否是攻击
        if (!Input.GetKeyDown(KeyCode.K))
        {
            return;
        }
        if (!isPushing && !SnowUtils.IsCollision(playerFoot, 0.1f, Const.Layer.Default))
        {
            return;
        }
        isRolling = true;
        aiWalkObject.DoStart(RollSpeed, player.transform.localScale.x > 0);
        gameObject.layer = LayerMask.NameToLayer(Const.Layer.RollingBall);
        PostNotification.Post<Vector3>(Const.Event.Invinsible, this, transform.position);
    }

    /**
     * 设置 hp
     */
    private void setHp(int mhp)
    {
        this.hp = mhp;
        hpSetter.Set(hp - 1);
        // 重置时长
        currentCountdown = 0;
        // 如果是1、2 则让其不可触碰
        if (hp < MAX_HP)
        {
            // collider2D 设置为触发器
            gameObject.layer = LayerMask.NameToLayer(Const.Layer.UnformedBall);
        }
        else
        {
            // 可以触碰
            gameObject.layer = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果是子弹攻击到，同时hp < MAX_HP，则hp ++
        if (collision.tag == Const.Tag.Bullet && !IsActive())
        {
            setHp(hp + 1);
        }
    }

    private void checkCollisionBoss()
    {
        if (isDestroyed)
        {
            return;
        }
        // 如果是碰到boss
        if (SnowUtils.IsCollision(transform, 0.1f, Const.Layer.Boss, Const.Layer.BossDown))
        {
            isDestroyed = true;
            Destroy(gameObject);
            // 碰到到boss 分
            PostNotification.Post<int>(Const.Event.IncrementScore, this, 200);
            PostNotification.Post<int>(Const.Event.BossAttacked, this, 200);
        }
    }
    /// <summary>
    /// 检查有没有敌人
    /// </summary>
    private void checkCollisionEnemy()
    {
        if (isDestroyed || !isRolling)
        {
            return;
        }
        SnowUtils.DoCollision(transform.position, -transform.localScale, 0.2f, collider =>
        {
            Debug.Log($"Clear ball game object from enemy");
            // 如果碰撞到了，则直接销毁
            BabyEnemyObject baby = collider.gameObject.GetComponent<BabyEnemyObject>();
            baby.DoDeath();
        }, Const.Layer.Enemy);

    }
    /// <summary>
    /// 滚动球碰撞检测
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == Const.Tag.Ball)
        {
            BallObject ballObject = collision.gameObject.GetComponent<BallObject>();
            if (!ballObject.isRolling)
            {
                return;
            }
            // 碰到其他雪球来点装备, 30%几率出状态
            if (RandomUtils.isWithinRatioOfPrecent(30))
            {
                Optional<GameObject> water = RandomUtils.GetOneUnityObject(Waters);
                if (water.IsPresent())
                {
                    Instantiate(water.Get(), transform.position, transform.rotation);
                }
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 如果是最下面的左右侧，则消失
        if (collision.gameObject.tag == Const.Tag.Ground && collision.gameObject.name.Contains("Dead"))
        {
            Destroy(gameObject);
        }
    }
}
