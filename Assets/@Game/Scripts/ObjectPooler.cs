using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPooler : Singleton<ObjectPooler>
{
    // ������Ʈ Ǯ�� �����ϱ� ���� ��ųʸ�
    private readonly Dictionary<string, Queue<GameObject>> _objectPool = new();

    /// <summary>
    /// �������� Ǯ���ؼ� ������Ʈ�� ��ȯ��
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject Spawn(GameObject prefab, Transform parent = null)
    {
        string key = prefab.name;

        // ���� Ǯ�� �ش� Ű�� ���ٸ�, ���� ����
        if (!_objectPool.ContainsKey(key))
        {
            _objectPool[key] = new Queue<GameObject>();
        }

        // Ǯ�� ������Ʈ�� �����ϴ��� Ȯ��
        if (_objectPool[key].Count > 0)
        {
            // ť���� ������Ʈ�� ������
            GameObject pooledObject = _objectPool[key].Dequeue();

            pooledObject.SetActive(true);
            if (parent != null) pooledObject.transform.SetParent(parent);
            ResetObject(pooledObject);
            return pooledObject;
        }
        else
        {
            // Ǯ�� ��� ������ ������Ʈ�� ���ٸ� ���� ����
            GameObject newObject = Instantiate(prefab, parent);
            newObject.name = newObject.GetInstanceID().ToString();
            return newObject;
        }
    }

    /// <summary>
    /// ��ο��� ���ҽ��� �ҷ��ͼ� Ǯ����Ű�� ������Ʈ�� ��ȯ��
    /// </summary>
    /// <param name="path"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject SpawnFromPath(string path, Transform parent = null)
    {
        // ���ҽ��� ��ο��� �ε�
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Logger.LogError($"���ҽ� ��� '{path}'���� �������� ã�� �� �����ϴ�.");
            return null;
        }

        // ���� Spawn �޼��带 ����� �������� Ǯ���ؼ� ��������
        return Spawn(prefab, parent);
    }

    /// <summary>
    /// ������Ʈ�� Ǯ �ȿ� ��ȯ��Ŵ
    /// </summary>
    /// <param name="obj"></param>
    public void ReturnToPool(GameObject obj)
    {
        if (!obj.activeInHierarchy)
        {
            Logger.LogWarning("�̹� Ǯ�� ��ȯ�� ������Ʈ�Դϴ�.");
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);  // Ǯ ������ �θ�� ����

        if (!_objectPool.ContainsKey(obj.name))
        {
            _objectPool[obj.name] = new Queue<GameObject>();
        }

        // ť�� �߰�
        _objectPool[obj.name].Enqueue(obj);
    }

    /// <summary>
    /// UI ������Ʈ�� Ǯ�� ��ȯ��Ű��, �ڽĿ� ���ο� Canvas�� ������ �� �θ�� �Ҵ���
    /// </summary>
    /// <param name="uiObject"></param>
    public void ReturnToPoolUI(GameObject uiObject)
    {
        if (!uiObject.activeInHierarchy)
        {
            //Debug.LogWarning("�̹� Ǯ�� ��ȯ�� UI ������Ʈ�Դϴ�.");
            return;
        }

        var canvasChild = transform.Find("UI_Canvas");
        GameObject canvasObject = canvasChild?.gameObject;

        if (canvasObject == null)
        {
            canvasObject = new GameObject("UI_Canvas");

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.AddComponent<CanvasScaler>(); // UI ������ ����
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            canvasObject.AddComponent<GraphicRaycaster>(); // UI Ŭ�� �̺�Ʈ ó��

            canvasObject.transform.SetParent(transform, false);
        }
        
        // UI �θ� ResourceManager�� ����
        uiObject.transform.SetParent(canvasObject.transform, false);

        // UI ������Ʈ�� ��Ȱ��ȭ�ϰ� Ǯ�� �߰�
        uiObject.SetActive(false);

        if (!_objectPool.ContainsKey(uiObject.name))
        {
            _objectPool[uiObject.name] = new Queue<GameObject>();
        }

        _objectPool[uiObject.name].Enqueue(uiObject);
    }


    public Queue<GameObject> GetPoolObjects(string key)
    {
        return _objectPool[key];
    }

    /// <summary>
    /// ������Ʈ ���� �ʱ�ȭ
    /// </summary>
    /// <param name="obj"></param>
    private void ResetObject(GameObject obj)
    {
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }
}
