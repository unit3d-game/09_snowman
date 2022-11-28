using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{

    /**
     * <summary>子弹速度</summary>
     */
    public float Speed = 4;

    /**
     * <summary>子弹预制体</summary>
     */
    public GameObject BulletPrefab;

    /**
     * <summary>射击间隔</summary>
     * 
     */
    public float ShootInterval;

    private Transform firePosition;


    private float lastShootTime;

    private PlayerControl player;

    private void Awake()
    {
        firePosition = transform.Find("FirePoint");
        player = GetComponent<PlayerControl>();
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
            Instantiate(BulletPrefab, firePosition.position, firePosition.rotation);
            lastShootTime = 0;
        }
        else
        {
            lastShootTime += Time.deltaTime;
        }
    }
}
