using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private FishScriptableObject _fishData;

    public FishScriptableObject GetData()
    {
        return _fishData;
    }
}
