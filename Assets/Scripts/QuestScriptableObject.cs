using UnityEngine;

public class GameEnums
{
    public enum QuestType
    {
        Kills,
        Purchase,
        NoHit,
        Appear
    }
}

[CreateAssetMenu(fileName = "Quest", menuName = "Quest Data")]
public class QuestScriptableObject : ScriptableObject
{
    public GameEnums.QuestType type;
    public int amount;
}
