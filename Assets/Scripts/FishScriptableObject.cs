using UnityEngine;

[CreateAssetMenu(fileName = "Fish", menuName = "Fish Data", order = 51)]
public class FishScriptableObject : ScriptableObject
{
    [Header("Boxing")]
    public int fishHealth;
    public float fishTimer;
    public float minTimeBetweenPunches;
    public float maxTimeBetweenPunches;

    [Header("Other")]
    public int fishReward;
    [Tooltip("Leave empty if basic fish")]
    public QuestScriptableObject quest;
}
