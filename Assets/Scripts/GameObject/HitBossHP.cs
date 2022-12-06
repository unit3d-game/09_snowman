using UnityEngine;
using System.Collections;
using TMPro;

/// <summary>
/// 命中 boss 的血量
/// </summary>
public class HitBossHP : MonoBehaviour
{


    /// <summary>
    /// 显示时长
    /// </summary>
    public float ShowDuration = 1.5f;


    private TMP_Text hpText;


    /// <summary>
    /// 本次扣除的血量，在 update 之前调用
    /// </summary>
    /// <param name="_hp">血量</param>
    public void Init(int _hp)
    {
        hpText.text = $"- {_hp}";
    }

    private void Awake()
    {
        // 初始化起始坐标
        transform.position = Camera.main.WorldToScreenPoint(GameObject.Find("Boss").transform.Find("HitHP").position);
        hpText = GetComponent<TMP_Text>();
        GetComponent<RectTransform>().sizeDelta = SnowUtils.ScaleWithScreen(GetComponent<RectTransform>().sizeDelta);
    }


    void FixedUpdate()
    {
        // 更新坐标位置
        transform.position += Vector3.up * (1 / Time.fixedDeltaTime) * Time.deltaTime;
        ShowDuration -= Time.deltaTime;
        Color now = hpText.color;
        now.a = ShowDuration / 1.5f;

        hpText.color = now;
        if (ShowDuration <= 0)
        {
            Destroy(gameObject);
        }
    }
}

