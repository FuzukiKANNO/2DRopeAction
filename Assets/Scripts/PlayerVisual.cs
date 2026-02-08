using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    PlayerGoal player;

    void Awake()
    {
        player = GetComponentInParent<PlayerGoal>();
    }

    public void OnGoalAnimationEnd()
    {
        player.OnGoalAnimationEnd();
    }
}
