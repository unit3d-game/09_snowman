using UnityEngine;
using System.Collections;


/// <summary>
///   药水的逻辑：
///   1、蓝色: 增加子弹效果，效果增加一倍
///   2、绿色：玩家进入无敌状态
///   3、红色：玩家速度提升一倍
///   4、黄色: 延长子弹飞行时间，增加50 %
/// </summary>
public enum WaterType
{
    Blue,
    Yellow,
    Red,
    Green
}

public class WaterObject : MonoBehaviour
{

    /// <summary>
    /// 药水类型
    /// </summary>
    public WaterType WaterType;


}

