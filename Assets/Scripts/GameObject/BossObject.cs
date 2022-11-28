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
    /// 最小重力加速度
    /// </summary>
    public int MinForce = 300;

    /// <summary>
    /// 最大重力加速度
    /// </summary>
    public int MaxForce = 600;

    /// <summary>
    /// 最小怪兽速度
    /// </summary>
    public int MinSpeed = 1;

    /// <summary>
    /// 最大怪兽速度
    /// </summary>
    public int MaxSpeed = 3;

    /// <summary>
    /// 大跳的比率
    /// </summary>
    public int BigJumpPre = 30;

    private float stairPosY = 1;

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





    // Use this for initialization
    //void Start()
    //{
    //    nextCreateTime = FireDuration;
    //}
    public override void Awake()
    {
        base.Awake();

        firePosition = transform.Find("FirePoint");
        footPosition = transform.Find("Foot");

        Animator animator = GetComponent<Animator>();
        isDeadSetter = new AnimSetter<bool>(animator, "Dead");
        isGroundSetter = new AnimSetter<bool>(animator, "IsGround");
        isJumpTrigger = new AnimTrigger(animator, "Jump");
        isBigJumpTrigger = new AnimTrigger(animator, "BigJump");
    }

    // Update is called once per frame
    void Update()
    {
        setGround();
        createEnemy();
    }

    private void setGround()
    {
        isGround = SnowUtils.IsCollision(footPosition, 0.2f, "Boss");
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
        if (isGround)
        {
            isGroundSetter.Set(true);
        }
        else
        {
            isGroundSetter.Set(false);
        }
    }

    private void toJump()
    {
        // 随机
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
        nextCreateTime = Random.Range(1, 8);
    }
}
