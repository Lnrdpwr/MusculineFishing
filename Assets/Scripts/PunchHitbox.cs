using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    public delegate void PunchDelegate();
    public PunchDelegate onPunch;

    private BoxingMinigame _minigame;

    public void Construct(BoxingMinigame minigame)
    {
        _minigame = minigame;
    }

    private void OnMouseDown()
    {
        _minigame.Hit();
    }
}
