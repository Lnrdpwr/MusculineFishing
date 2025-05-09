using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fish", menuName = "Fish Data", order = 51)]
public class FishScriptableObject : ScriptableObject
{
    [SerializeField] private float _fishAgility;
    [SerializeField] private float _fishTimer;
    [SerializeField] private int _fishReward;
}
