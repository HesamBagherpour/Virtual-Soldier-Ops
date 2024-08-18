using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject objectPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(objectPrefab, transform).SetActive(false);
    }

    public GameObject GiveObject()
    {
        GameObject firstChild = transform.GetChild(0).gameObject;
        if(firstChild.activeSelf)
        {
            GameObject newChild = Instantiate(objectPrefab, transform);
            newChild.transform.SetAsLastSibling();
            return newChild;
        }

        firstChild.SetActive(true);  
        firstChild.transform.SetAsLastSibling();
        return firstChild;
    }

    public void TakeObject(GameObject obj, float delay = 0)
    {
       StartCoroutine(TakeObject_CO(obj,delay));
    }

    private IEnumerator TakeObject_CO(GameObject obj, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        obj.transform.SetAsLastSibling();
    }
}
