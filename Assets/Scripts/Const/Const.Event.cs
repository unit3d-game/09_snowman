using System;
public partial class Const
{
    public class Event
    {

        /**
         * <summary>玩家死亡</summary>
         */
        public const string PlayerDied = "PlayerDied";

        /**
         * <summary>Boss 被击中</summary>
         */
        public const string BossAttacked = "BossAttacked";

        /**
         * <summary>Boss 死亡</summary>
         */
        public const string BossDied = "BossDied";

        // 开启无敌状态
        public const string Invinsible = "Invinsible";

        // 增加积分
        public const string IncrementScore = "IncrementScore";

        //重新复活
        public const string PlayerRestart = "PlayerRestart";
    }
}

