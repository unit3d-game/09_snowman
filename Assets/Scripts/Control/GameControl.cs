using UnityEngine;
using System.Collections;
using MyUtils;

public class GameControl : BaseNotificationBehaviour
{

    /// <summary>
    /// 主角预制体
    /// </summary>
    public GameObject PlayerPrefab;


    private Transform playerPoint;



    void Start()
    {
        createPlayer();
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
        createPlayer();
    }

    private void createPlayer()
    {
        GameObject p = Instantiate(PlayerPrefab, new Vector3(1.63f, -3.64f, 0), Quaternion.identity, transform.parent);
        p.name = Const.ObjectName.Player;
        playerPoint = p.transform.Find("Point");
    }
}

