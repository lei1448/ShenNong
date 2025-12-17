using UnityEngine;


public struct OnCropUpdated // 农作物状态更新（种植、生长、成熟）
{
    public Vector3Int Position;
}

public struct OnTermScoreSettled // 节气结算分数
{
    public int Score;
    public string TermName;
}

public struct OnCropDead // 农作物死亡
{
    public Vector3Int Position;
}