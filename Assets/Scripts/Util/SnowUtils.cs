using System;
using UnityEngine;

public class SnowUtils
{


    /**
     * <summary>检查是否碰撞</summary>
     * <param name="position">检测的物体位置</param>
     * <param name="radius">中心点的检测范围，圆形半径</param>
     * <param name="layerNames">显示层</param>
     * <returns>是否有碰撞，如果有则返回true，反之亦然</returns>
     */
    public static bool IsCollision(Transform position, float radius, params string[] layerNames)
    {
        return Physics2D.OverlapCircle(position.position, radius, LayerMask.GetMask(layerNames));
    }

}

