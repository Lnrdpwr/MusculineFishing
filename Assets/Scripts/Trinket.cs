using UnityEngine;

[CreateAssetMenu(fileName = "Trinket", menuName = "Trinket Data")]
public class Trinket : ScriptableObject
{
    public Sprite sprite;
    public string trinketName;
    public string trinketDescription;
    public int price;
}
