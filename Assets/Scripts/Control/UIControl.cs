using UnityEngine;
using System.Collections;
using MyUtils;
using TMPro;

public class UIControl : BaseNotificationBehaviour
{


    private TMP_Text currentText;

    private TMP_Text highText;

    private TMP_Text lifeText;

    private TMP_Text hpText;

    private RectTransform hpSlider;

    private UserData userData;

    private int MaxHp = 0;

    private float hpBGWidth;

    private float resurrectionTime = 0;

    public override void Awake()
    {
        base.Awake();
        currentText = GameObjectUtils.RequiredGetComponent<TMP_Text>(transform, "CurrentValue");
        highText = GameObjectUtils.RequiredGetComponent<TMP_Text>(transform, "HighValue");
        lifeText = GameObjectUtils.RequiredGetComponent<TMP_Text>(transform, "LifeNum");
        hpText = GameObjectUtils.RequiredGetComponent<TMP_Text>(transform, "HPA");
        hpSlider = GameObjectUtils.RequiredGetComponent<RectTransform>(transform, "HBSlider");
        hpBGWidth = GameObjectUtils.RequiredGetComponent<RectTransform>(transform, "HPBG").rect.width - 10;
        userData = new UserData();
        UserData ud = JSONUtils.Load<UserData>();
        MaxHp = userData.BossHP;
        userData.HighScore = ud.HighScore;
    }
    void Start()
    {
        currentText.text = $"{userData.CurrentScore}";
        highText.text = $"{userData.HighScore}";
        hpText.text = $"{userData.BossHP}/{MaxHp}";
        lifeText.text = $"{userData.LifeNum}";
    }

    void Update()
    {
        if (resurrectionTime > 0)
        {
            resurrectionTime -= Time.deltaTime;
            if (resurrectionTime <= 0)
            {
                resurrectionTime = 0;
                PostNotification.Post(Const.Event.PlayerRestart, this);
            }
        }
    }

    [Subscribe(Const.Event.IncrementScore)]
    public void IncrementPoint(MessagePayload<int> data)
    {
        userData.CurrentScore += data.data;
        if (userData.CurrentScore > userData.HighScore)
        {
            userData.HighScore = userData.CurrentScore;
            JSONUtils.Save<UserData>(userData);
            highText.text = $"{userData.HighScore}";
        }
        currentText.text = $"{userData.CurrentScore}";
    }

    [Subscribe(Const.Event.PlayerDied)]
    public void PlayerDied()
    {
        if (userData.LifeNum == 0)
        {
            return;
        }
        userData.LifeNum -= 1;
        lifeText.text = $"{userData.LifeNum}";
        resurrectionTime = 3;
    }

    [Subscribe(Const.Event.BossAttacked)]
    public void HitBoss(MessagePayload<int> data)
    {
        if (userData.BossHP == 0)
        {
            return;
        }
        userData.BossHP = userData.BossHP >= data.data ? userData.BossHP - data.data : 0;
        hpText.text = $"{userData.BossHP}/{MaxHp}";
        float tt = (float)(MaxHp - userData.BossHP) / (float)MaxHp * hpBGWidth;
        hpSlider.sizeDelta = new Vector2(5 + tt, hpSlider.sizeDelta.y);
        Debug.Log($"hp is {userData.BossHP}, {tt}");
        Vector3 pos = hpSlider.localPosition;
        pos.x = (tt - hpBGWidth) / 2;
        hpSlider.localPosition = pos;
        if (userData.BossHP == 0)
        {
            // BOSS 死亡
            PostNotification.Post(Const.Event.BossDied, this);
        }
    }
}

