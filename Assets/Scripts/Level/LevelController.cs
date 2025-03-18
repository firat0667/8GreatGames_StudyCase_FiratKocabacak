using UnityEngine;

public class LevelController : MonoBehaviour
{
    public void Initialize()
    {
        Debug.Log("LevelController Initialized!");
    }

    public void DestroyLevel()
    {
        Debug.Log("Destroying LevelController...");
        Destroy(gameObject);
    }
}
