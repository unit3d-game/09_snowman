using UnityEngine;
using MyUtils;

/**
 * 雪球逻辑：
 * 1、HP = 3 之前是不能被碰撞和移动的（除了子弹可以），而且hp * 5秒内会复活
 * 2、人物站在雪球上时，是可以推动和攻击的，攻击（按K）的话会使其拥有 6 的速度向前方快速滚动
 * 3、雪球可以推动人物运动（人物进入无敌状态），如果此时跳跃（按J）就会脱离跟随状态，人物同时进入无敌状态
 * 4、碰到Wall和BOSS则消失
 * 5、HP是1、2的时候，人物是碰触不道德
 */
public class BallObject : MonoBehaviour
{

    /**
     * <summary>每个hp衰减的时长，默认5秒</summary>
     * 
     */
    public float HpDuration = 5;

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
    private bool isRolling = false;


    private GameObject babyEnemyPrefab;

    private AiWalkObject aiWalkObject;

    private GameObject player;

    private PlatformEffector2D platformEffector2D;


    private void Awake()
    {
        Animator animator = GetComponent<Animator>();
        platformEffector2D = GetComponent<PlatformEffector2D>();
        hpSetter = new AnimSetter<int>(animator, "hp");
        isRollingSetter = new AnimSetter<bool>(animator, "Rolling");
        babyEnemyPrefab = GameObject.Find(Const.ObjectName.Boss).GetComponent<BossObject>().EnemyPrefab;
        aiWalkObject = GetComponent<AiWalkObject>();
        player = GameObject.Find("Player");
    }

    void Update()
    {
        // 是否需要复活成小怪兽
        toBabyEnemy();
        DoAttack();
    }

    // 复活小怪兽
    private void toBabyEnemy()
    {
        // 如果是在推动或者滚动状态下，则倒计时关闭
        if (isPushing || isRolling)
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
        if (!isPushing || isRolling)
        {
            return;
        }
        // 是否是攻击
        if (!Input.GetKeyDown(KeyCode.K))
        {
            return;
        }
        isRolling = true;
        aiWalkObject.DoStart(RollSpeed, player.transform.localScale.x > 0);
        gameObject.layer = LayerMask.NameToLayer(Const.Layer.RollingBall);
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
        Debug.Log($"set hp is {mhp}");
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
}
