using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointObject : MonoBehaviour
{
    /// <summary>
    /// 显示时长
    /// </summary>
    public float ShowDuration = 1.5f;

    /// <summary>
    /// 开始时间
    /// </summary>
    private float startTime;


    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime >= ShowDuration)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position += Vector3.up * Time.deltaTime;
            Color color = spriteRenderer.color;
            color.a = (ShowDuration - Time.time + startTime) / ShowDuration;
            spriteRenderer.color = color;
        }
    }

}
