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
    public bool Direct { private set; get; } = false;

    // 是否在移动中
    public bool IsMoving { private set; get; } = false;

    /**
     * <summary>开始移动</summary>
     */
    public void DoStart(float speed, bool direct)
    {
        this.speed = speed;
        this.Direct = direct;
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
        if (this.Direct)
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
            this.Direct = !this.Direct;
            // 左侧
            if (this.Direct)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

}

