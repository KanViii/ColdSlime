using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    [Tooltip("Check if you want to prevent duplicates when reloading the original scene.")]
    public bool preventDuplicates = true;

    private void Awake()
    {
        if (preventDuplicates)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(gameObject.tag);
            
            foreach (GameObject obj in objs)
            {
                if (obj != this.gameObject && obj.name == this.gameObject.name)
                {
                    Destroy(this.gameObject);
                    return;
                }
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
