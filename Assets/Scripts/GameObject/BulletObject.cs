using UnityEngine;
using System.Collections;
using MyUtils;

public class BulletObject : MonoBehaviour
{


    /**
     * <summary>最大射程</summary>
     * 
     */
    public float MaxDistance = 1;

    private Transform firePoint;

    private bool isLeft;

    private float speed;


    private Rigidbody2D rbody;

    // 当前运动距离
    private float currentDistance = 0;




    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.Find(Const.ObjectName.Player);
        speed = player.GetComponent<BulletControl>().Speed;
        firePoint = player.transform.Find("FirePoint");
        isLeft = player.transform.localScale.x > 0;
        Debug.Log($"bullet dir is {isLeft}");
        transform.position = firePoint.position;
        if (!isLeft)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void Update()
    {
        float dis = checkDistance();
        Vector3 pos = transform.position;

        if (isLeft)
        {
            pos.x -= dis;
        }
        else
        { pos.x += dis; }
        transform.position = pos;
    }

    /**
     * <summary>检查距离，超过距离则开始使用重力</summary>
     * 
     */
    private float checkDistance()
    {
        float dis = speed * Time.deltaTime;
        currentDistance += dis;
        if (currentDistance >= MaxDistance && rbody.isKinematic)
        {
            // 当大于最大距离时，开始受重力影响
            rbody.isKinematic = false;
        }
        return dis;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果是碰到 boss
        if (collision.gameObject.layer == LayerMask.NameToLayer(Const.Layer.Boss) || collision.gameObject.layer == LayerMask.NameToLayer(Const.Layer.BossDown))
        {
            PostNotification.Post<int>(Const.Event.BossAttacked, this, 10);
            PostNotification.Post<int>(Const.Event.IncrementScore, this, 10);
        }
        // 无论碰到谁，都销毁
        Destroy(gameObject);
    }
}

