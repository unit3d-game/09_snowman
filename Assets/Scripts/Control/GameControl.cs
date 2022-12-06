using UnityEngine;
using System.Collections;
using MyUtils;

public class GameControl : BaseNotificationBehaviour
{

    private Transform playerPoint;

    private PlayerControl player;


    public override void Awake()
    {
        base.Awake();
        player = GameObject.Find(Const.ObjectName.Player).GetComponent<PlayerControl>();
        playerPoint = player.transform.Find("Point");
    }

    [Subscribe(Const.Event.IncrementScore)]
    public void OnIncrementScore(MessagePayload<int> data)
    {
        GameObject pointPrefab = Resources.Load<GameObject>($"Prefabs/Point{data.data}");
        GameObject obj = Instantiate(pointPrefab, playerPoint.position, playerPoint.rotation, transform.parent);
        obj.AddComponent<PointObject>();
    }

    [Subscribe(Const.Event.PlayerRestart)]
    public void OnPlayerRestart()
    {
        player.Restart();
    }
}

