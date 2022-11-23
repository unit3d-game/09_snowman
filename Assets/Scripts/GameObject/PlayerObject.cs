using System;
using System.Collections;
using System.Collections.Generic;
using MyUtils;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{

    private readonly string[] GroundTags = ArrayUtils.As<string>(Const.Tag.Ground, Const.Tag.Stair);

    private AnimTrigger isJumpTrigger;

    private AnimSetter<bool> isGroundSetter;

    private AnimSetter<float> speedSetter;

    private AnimTrigger isSlowTrigger;

    private AnimTrigger isFireTrigger;

    private AnimSetter<bool> isPushSetter;

    private AnimSetter<bool> isSpeedUpSetter;

    private AnimTrigger isDeadTrigger;

    private Rigidbody2D rbody;

    private bool isGround = true;

    private float JumpedSpeed = 3;


    private void Awake()
    {
        Animator animator = GetComponent<Animator>();
        isJumpTrigger = new AnimTrigger(animator, "Jump");
        isGroundSetter = new AnimSetter<bool>(animator, "IsGround");
        speedSetter = new AnimSetter<float>(animator, "Speed");
        isSlowTrigger = new AnimTrigger(animator, "Slow");
        isFireTrigger = new AnimTrigger(animator, "Fire");
        isPushSetter = new AnimSetter<bool>(animator, "Push");
        isSpeedUpSetter = new AnimSetter<bool>(animator, "SpeedUp");
        isDeadTrigger = new AnimTrigger(animator, "Dead");
        isGroundSetter.Set(true);
        rbody = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        // to jump
        toJump();
        // 计算走路
        toWalk();
    }


    private bool toJump()
    {
        if (!Input.GetKeyDown(KeyCode.J) || !isGround)
        {
            return false;
        }

        rbody.AddForce(Vector2.up * 300);
        isJumpTrigger.Trigger();
        isGround = false;
        return true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Coll trigger is {collision.tag}");
    }

    private void toWalk()
    {
        float hor = Input.GetAxis(MyUtils.Const.Axis.Horizontal);
        speedSetter.Set(Math.Abs(hor));
        // 如果小于0.1 则忽略
        if (Math.Abs(hor) <= 0.1)
        {
            isSpeedUpSetter.Set(false);
            return;
        }
        isSpeedUpSetter.Set(true);
        // 来个旋转
        Vector3 pos = transform.position;
        pos.x += hor * 2 * Time.deltaTime;
        transform.position = pos;
        if (hor < 0)
        {
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, -180f, 0));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 放地上
        if (Array.IndexOf(GroundTags, collision.gameObject.tag) >= 0)
        {
            isGroundSetter.Set(true);
            isGround = true;
        }
    }
}
