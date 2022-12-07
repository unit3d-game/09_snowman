using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using MyUtils;

public class BossObject : BaseNotificationBehaviour
{

    /// <summary>
    /// 小怪兽预制体
    /// </summary>
    public GameObject EnemyPrefab;

    /// <summary>
    /// 显示boss的扣血动画
    /// </summary>
    public GameObject ShowHitBossHPPrefab;

    /// <summary>
    /// 最小重力加速度
    /// </summary>
    public int MinForce = 100;

    /// <summary>
    /// 最大重力加速度
    /// </summary>
    public int MaxForce = 200;

    /// <summary>
    /// 最小怪兽速度
    /// </summary>
    public float MinSpeed = 0.5f;

    /// <summary>
    /// 最大怪兽速度
    /// </summary>
    public float MaxSpeed = 1f;

    /// <summary>
    /// 大跳的比率
    /// </summary>
    public int BigJumpPrecent = 30;

    /// <summary>
    /// 延迟初始化时间
    /// </summary>
    public float DelayedInitTime = 5;

    private const float stairPosY = 1;



    private float nextCreateTime = 0;

    private Transform firePosition;

    private Transform footPosition;

    private bool isGround = false;


    // 是否脚踏实地
    private AnimSetter<bool> isGroundSetter;

    private AnimSetter<bool> isDeadSetter;

    private AnimTrigger isJumpTrigger;

    private AnimTrigger isBigJumpTrigger;

    /**
     * 下一次跳跃时间
     */
    private float nextJumpTime;

    // 显示击中时间
    private float showHitedTime;


    private Rigidbody2D rbody;


    private SpriteRenderer spriteRenderer;

    public override void Awake()
    {
        base.Awake();

        firePosition = transform.Find("FirePoint");
        footPosition = transform.Find("Foot");
        rbody = GetComponent<Rigidbody2D>();
        Animator animator = GetComponent<Animator>();
        isDeadSetter = new AnimSetter<bool>(animator, "Dead");
        isGroundSetter = new AnimSetter<bool>(animator, "IsGround");
        isJumpTrigger = new AnimTrigger(animator, "Jump");
        isBigJumpTrigger = new AnimTrigger(animator, "BigJump");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DelayedInitTime > 0)
        {
            DelayedInitTime -= Time.deltaTime;
            return;
        }
        setGround();
        createEnemy();
        showHitedAnimation();
    }


    private void showHitedAnimation()
    {
        if (showHitedTime <= 0)
        {
            return;
        }
        // 动画闪耀
        showHitedTime -= Time.deltaTime;
        if (showHitedTime <= 0)
        {
            showHitedTime = 0;
            // 重置
            spriteRenderer.color = Color.white;
        }
        else
        {
            int rin = (int)(showHitedTime * 10) % Const.Common.RandomColors.Length;
            spriteRenderer.color = Const.Common.RandomColors[rin];
            Debug.Log($"Color is {rin}");
        }

    }

    private void setGround()
    {

        bool isSync = !(isGround ^ isGroundSetter.Get());
        // 以同步状态，则开始倒计时
        if (isSync)
        {
            nextJumpTime -= Time.deltaTime;
            if (nextJumpTime <= 0)
            {
                nextCreateTime = 0;
                toJump();
            }
            return;
        }
    }

    private void toJump()
    {
        // 随机大跳和小跳
        // 如果是随机大跳
        if (RandomUtils.isWithinRatioOfPrecent(BigJumpPrecent))
        {
            bool isUp = transform.position.y < stairPosY;
            if (isUp)
            {
                rbody.AddForce(Vector2.up * 500);
            }
            else
            {
                // 掉下来
                gameObject.layer = LayerMask.NameToLayer(Const.Layer.BossDown);
            }
            isGroundSetter.Set(false);
            isBigJumpTrigger.Trigger();
        }
        else
        {
            rbody.AddForce(Vector2.up * 200);
            isGroundSetter.Set(false);
            isJumpTrigger.Trigger();
        }
        // 重新充值下一次跳跃时间
        nextJumpTime = Random.Range(2f, 6f);
    }

    private void createEnemy()
    {
        if (nextCreateTime > 0)
        {
            nextCreateTime -= Time.deltaTime;
            return;
        }
        GameObject enemy = Instantiate(EnemyPrefab, firePosition.transform.position, transform.rotation);
        enemy.GetComponent<BabyEnemyObject>().DoStart(Random.Range(MinForce, MaxForce), Random.Range(MinSpeed, MaxSpeed));
        nextCreateTime = Random.Range(5f, 15f);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == Const.Tag.Ground || collision.gameObject.tag == Const.Tag.Stair)
        {
            gameObject.layer = LayerMask.NameToLayer(Const.Layer.Boss);
            isGround = true;
            isGroundSetter.Set(true);
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == Const.Tag.Ground)
        {
            isGround = false;
            isGroundSetter.Set(false);
        }
    }

    [Subscribe(Const.Event.BossAttacked)]
    public void OnHitBoss(MessagePayload<int> payload)
    {
        showHitedTime = 0.6f;
        GameObject showHitBossHp = Instantiate(ShowHitBossHPPrefab);
        showHitBossHp.transform.SetParent(GameObject.Find("UIMananger").transform.Find("Canvas"));
        showHitBossHp.GetComponent<HitBossHP>().Init(payload.data);
    }

}
