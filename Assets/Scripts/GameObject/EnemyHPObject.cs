using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 敌人血量显示 text
/// </summary>
public class EnemyHPObject : MonoBehaviour
{
    private GameObject enemy;

    private TMP_Text text;

    private int hp;

    private Camera currentCamera;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        currentCamera = Camera.main;
        // 重新计算宽高
        GetComponent<RectTransform>().sizeDelta = SnowUtils.ScaleWithScreen(GetComponent<RectTransform>().sizeDelta);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="_hp">默认血量</param>
    /// <param name="_enemy">关联的敌人</param>
    public void Init(int _hp, GameObject _enemy)
    {
        this.enemy = _enemy;
        UpdateHp(_hp);
    }

    /// <summary>
    /// 更新 血量
    /// </summary>
    /// <param name="_hp"></param>
    public void UpdateHp(int _hp)
    {
        this.hp = _hp;
        text.text = $"HP: {this.hp}";
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        updatePosition();
    }

    private void updatePosition()
    {
        Vector3 epos = enemy.transform.position;
        epos.y += 0.6f;
        // 更新 位置
        transform.position = currentCamera.WorldToScreenPoint(epos);
    }
}
