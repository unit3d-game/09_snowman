using System.Collections;
using System.Collections.Generic;
using MyUtils;
using UnityEngine;

public class BulletControl : BaseNotificationBehaviour
{

    /**
     * <summary>子弹速度</summary>
     */
    public float Speed = 4;
    /// <summary>
    /// 最大距离
    /// </summary>
    public float MaxDistance = 3;

    public float EffectDuration = 20;

    /**
     * <summary>子弹预制体</summary>
     */
    public GameObject BulletPrefab;

    /// <summary>
    /// 大子弹的预制体
    /// </summary>
    public GameObject BigBulletPrefab;

    /**
     * <summary>射击间隔</summary>
     * 
     */
    public float ShootInterval;

    private Transform firePosition;


    private float lastShootTime;

    private PlayerControl player;

    // 增强子弹的有效时间
    private float bigBulletDuration;


    private float delayBulletDuration;

    public override void Awake()
    {
        base.Awake();
        firePosition = transform.Find("FirePoint");
        player = GetComponent<PlayerControl>();

    }
    /// <summary>
    /// 升级子弹
    /// </summary>
    [Subscribe(Const.Event.DrinkWater)]
    public void OnDrinkWater(MessagePayload<WaterType> payload)
    {
        // 子弹升级
        if (payload.data == WaterType.Blue)
        {
            bigBulletDuration = EffectDuration;
        }
        //子弹消失延迟
        else if (payload.data == WaterType.Yellow)
        {
            delayBulletDuration = EffectDuration;
            MaxDistance = 5;
        }
    }

    /// <summary>
    /// 清空子弹效果
    /// </summary>
    public void ClearWatterEffect()
    {

        bigBulletDuration = 0;
        delayBulletDuration = 0;
        MaxDistance = 3;
    }


    void Start()
    {
        lastShootTime = 0;
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && lastShootTime >= ShootInterval)
        {
            player.DoShoot();
            Instantiate(bigBulletDuration > 0 ? BigBulletPrefab : BulletPrefab, firePosition.position, firePosition.rotation);
            lastShootTime = 0;
        }
        else
        {
            lastShootTime += Time.deltaTime;
        }
        // 处理增强子弹的时间
        if (bigBulletDuration > 0)
        {
            bigBulletDuration -= Time.deltaTime;
            if (bigBulletDuration <= 0)
            {
                PostNotification.Post(Const.Event.ClearPlayerEffect, this);
                bigBulletDuration = 0;
            }
        }
        if (delayBulletDuration > 0)
        {
            delayBulletDuration -= Time.deltaTime;
            if (delayBulletDuration <= 0)
            {
                MaxDistance = 3;
                delayBulletDuration = 0;
                PostNotification.Post(Const.Event.ClearPlayerEffect, this);
            }
        }
    }
}
