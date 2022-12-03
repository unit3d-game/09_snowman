using UnityEngine;
using System.Collections;
using MyUtils;

public class SweetObject : MonoBehaviour
{
    /// <summary>
    /// 分数
    /// </summary>
    public int[] sweetPoints;

    /// <summary>
    /// 糖果图片
    /// </summary>
    public Sprite[] sweetImages;

    private int point;

    private void Awake()
    {
        // 随机一糖果
        int index = RandomUtils.GetIndex(sweetPoints);
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = sweetImages[index];
        point = sweetPoints[index];
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Const.Tag.Player)
        {
            PostNotification.Post<int>(Const.Event.IncrementScore, this, point);
            Destroy(gameObject);
        }
    }

}

