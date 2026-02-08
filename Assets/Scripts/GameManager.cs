using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Vector3 lastSavePosition;
    private bool hasSavePoint = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePosition(Vector3 pos)
    {
        lastSavePosition = pos;
        hasSavePoint = true;
    }

    public Vector3 GetRespawnPosition(Vector3 defaultPos)
    {
        return hasSavePoint ? lastSavePosition : defaultPos;
    }
}
