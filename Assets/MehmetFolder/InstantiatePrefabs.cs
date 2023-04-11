using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using TMPro;

public class InstantiatePrefabs : MonoBehaviour
{
    public List<GameObject> objectList;
    public List<GameObject> variantList;
    public List<GameObject> prefabList;
    private GameObject[] prefabsArray;

    private List<int> categoryPrefabCount;
    private List<int> categoryPrefabIndex;

    public int prefabCount;
    public int variantCount;
    public int countOfCategories;
    [SerializeField] private int categoryOffset;
    [SerializeField] private int prefabsOffset;
    [SerializeField] private TMP_Text tmpText;

    public List<string> categoryList;

    private string[] tagParts;
    private string tagg;

    private void Awake()
    {
        //objectList = Resources.LoadAll<GameObject>("Prefabs").ToList();
        string[] IGuids = AssetDatabase.FindAssets("t:GameObject", new string[] { "Assets/Prefabs" });
        for (int i = 0; i < IGuids.Length; i++)
        {
            string IAssetPath=AssetDatabase.GUIDToAssetPath(IGuids[i]);
            objectList.Add(AssetDatabase.LoadAssetAtPath<GameObject>(IAssetPath));
        }

        for (int i = 0; i < objectList.Count; i++)
        {
            tagg = objectList[i].tag.ToString();
            tagParts = objectList[i].tag.Split('/');
            if (tagParts[0] == "Prefabs" && !categoryList.Contains(tagParts[1]))
            {
                categoryList.Add(tagParts[1]);
            }
        }
        countOfCategories = categoryList.Count;
        categoryPrefabCount = new List<int>(new int[countOfCategories]);
        categoryPrefabIndex = new List<int>(new int[countOfCategories]);
        SpawnPrefabs();
    }

    public void SpawnPrefabs()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            GameObject instance=null;
            if (objectList[i] == PrefabUtility.GetCorrespondingObjectFromOriginalSource(objectList[i]))
            {

                if (objectList[i].tag.Split('/')[0] == "Prefabs")
                {
                    instance= Instantiate(objectList[i].gameObject);
                    prefabList.Add(instance);
                    prefabCount++;
                }
                else
                {
                    Debug.LogError("Check Tag: " + objectList[i].name);//Please set prefab's tag like Prefabs/Category/Tag
                }
            }
            else
            {
                instance = Instantiate(objectList[i].gameObject);
                variantList.Add(instance);


            }
            AddCameraMoveScript(instance);
        }
        for (int i = 0; i < countOfCategories; i++)
        {
            for (int j = 0; j < i; j++)
            {
                categoryPrefabIndex[i] += categoryPrefabCount[j];
            }
            categoryPrefabIndex[i] += categoryOffset*i;
            tmpText.text = categoryList[i].ToString();
            Instantiate(tmpText, new Vector3(categoryPrefabIndex[i], 0.01f, -5f), Quaternion.Euler(90f,0,0));
        }

        for (int i = 0; i < prefabList.Count; i++)
        {
            tagg = prefabList[i].tag.ToString();
            tagParts = prefabList[i].tag.Split('/');
            if (categoryList.Contains(tagParts[1]))
            {
                int c = categoryList.IndexOf(tagParts[1]);
                prefabList[i].transform.position = new Vector3(categoryPrefabIndex[c], 1f, 0f);
                categoryPrefabIndex[c]+=prefabsOffset;
            }
        }
        for (int j = 0; j < prefabList.Count; j++)
        {
            int x = 0;
            for (int i = 0; i < variantList.Count; i++)
            {
                tagg = variantList[i].tag.ToString();
                tagParts = variantList[i].tag.Split('/');
                if (variantList[i].CompareTag(prefabList[j].tag))
                {
                    if (categoryList.Contains(tagParts[1]))
                    {
                        variantList[i].transform.position = new Vector3(prefabList[j].transform.position.x, prefabList[j].transform.position.y, prefabList[j].transform.position.z + ((x + 1) * 2));
                        x++;

                    }
                }
            }
        }
    }
    public void AddCameraMoveScript(GameObject prefab)
    {
        if (prefab==null)
        {
            return;
        }
        prefab.AddComponent<CameraMoveToClick>();
    }
}
