using UnityEngine;
using System.Collections;

/**
 * <summary>游戏 AI 物体，可以根据碰撞物体来自行挪动，如果遇到 wall 就需要反向</summary>
 * 
 * 
 */
public class AiWalkObject : MonoBehaviour
{

    // 速度
    private float speed;

    // 方向, true 左边挪动，false 右侧挪动 
    private bool direct;

    // 是否在移动中
    public bool IsMoving { private set; get; } = false;

    /**
     * <summary>开始移动</summary>
     */
    public void DoStart(float speed, bool direct)
    {
        this.speed = speed;
        this.direct = direct;
        this.IsMoving = true;
    }



    void Update()
    {
        if (!IsMoving)
        {
            return;
        }
        doMove();
    }

    private void doMove()
    {
        Vector3 pos = transform.position;
        if (this.direct)
        {
            pos.x -= this.speed * Time.deltaTime;
        }
        else
        {
            pos.x += this.speed * Time.deltaTime;
        }

        transform.position = pos;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!this.IsMoving)
        {
            return;
        }
        // 如果是碰到了墙，则反向
        if (collision.gameObject.name == "LeftWall" || collision.gameObject.name == "RightWall")
        {
            // 切换方向
            this.direct = !this.direct;
            // 左侧
            if (this.direct)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        // 如果是最下面的左右侧，则消失
        if (collision.gameObject.tag == Const.Tag.Ground && collision.gameObject.name.Contains("Dead"))
        {
            Destroy(gameObject);
        }
    }

}

