using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    private string uiPrfabPath = "Prefabs/UI/";

    public static CanvasManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }


    public T LoadPrefabs<T>(string name) where T : MonoBehaviour
    {
        string fullPath = uiPrfabPath + name;
        GameObject prefab = Resources.Load<GameObject>(fullPath);
        if (!prefab)
        {
            Debug.LogError("Lỗi: Không tìm thấy Prefab tại đường dẫn Resources/" + fullPath + " ! Hãy chắc chắn file nằm trong thư mục Resources.");
            return null;
        }

        T uiInstance = Instantiate(prefab).GetComponent<T>();

        if (!uiInstance)
        {
            Debug.LogError("Lỗi: Prefab đã được tạo nhưng KHÔNG TÌM THẤY script " + typeof(T).Name + " gắn trên nó!");
            return null;
        }

        return uiInstance;
    }

    public void AddUI(MonoBehaviour ui)
    {
        if (!ui)
        {
            Debug.Log("UI null");
            return;
        }
        ui.transform.SetParent(Instance.transform, false);

    }
        
    public void removeUI(MonoBehaviour ui)
    {
        if (ui)
        {
            Destroy(ui.gameObject);
        }
    }
}
