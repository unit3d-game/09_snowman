using System;
using MyUtils;
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

    public static bool IsCollision(Transform position, float radius, int layer)
    {
        return Physics2D.OverlapCircle(position.position, radius, layer);
    }

    /**
     * <summary>检查是否碰撞</summary>
     * <param name="position">检测的物体位置</param>
     * <param name="radius">中心点的检测范围，圆形半径</param>
     * <param name="layerNames">显示层</param>
     * <returns>是否有碰撞，如果有则返回true，反之亦然</returns>
     */
    public static bool IsCollision(Vector3 position, float radius, params string[] layerNames)
    {

        return Physics2D.OverlapCircle(position, radius, LayerMask.GetMask(layerNames));
    }


    /// <summary>
    /// 检测两个点的距离或者说是碰撞
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="minDis"></param>
    /// <returns></returns>
    public static bool IsCollision(Vector3 pos1, Vector3 pos2, float minDis)
    {
        return Vector3.Distance(pos1, pos2) <= minDis;
    }

    /// <summary>
    /// 获取碰撞体
    /// </summary>
    /// <param name="pos">检测开始</param>
    /// <param name="direct">检测的方向</param>
    /// <param name="radius">检测的范围</param>
    /// <param name="layerNames">图层</param>
    /// <returns>碰撞体</returns>
    public static Optional<Collider2D> GetCollider(Vector2 pos, Vector2 direct, float radius, params string[] layerNames)
    {
        RaycastHit2D raycast = Physics2D.Raycast(pos, direct, radius, LayerMask.GetMask(layerNames));
        if (raycast.collider == null)
        {
            return Optional<Collider2D>.OfNullable();
        }

        return Optional<Collider2D>.Of(raycast.collider);
    }

    /// <summary>
    /// 是否
    /// </summary>
    /// <param name="pos">起始坐标</param>
    /// <param name="direct">检测方向</param>
    /// <param name="radius">范围</param>
    /// <param name="doAction">执行的动作</param>
    /// <param name="layerNames">碰撞层</param>
    public static void DoCollision(Vector2 pos, Vector2 direct, float radius, Action<Collider2D> doAction, params string[] layerNames)
    {
        Optional<Collider2D> optional = GetCollider(pos, direct, radius, layerNames);
        if (optional.IsPresent())
        {
            doAction(optional.Get());
        }
    }

    /// <summary>
    ///  根据频幕大小对物体宽高进行缩放
    /// </summary>
    /// <param name="origin">原始宽高,x 宽, y 高</param>
    /// <returns>缩放后的宽高</returns>
    public static Vector2 ScaleWithScreen(Vector2 origin)
    {
        float ratio = UnityEngine.Screen.width / Const.Screen.Width;
        // 开始缩放
        return new Vector2(origin.x * ratio, origin.y * ratio);
    }
}

