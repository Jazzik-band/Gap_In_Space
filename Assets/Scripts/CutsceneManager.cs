using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneManager: MonoBehaviour
{
    public static CutsceneManager Instance;
    [SerializeField] private List<CutsceneStruct> cutscenes = new List<CutsceneStruct>();
    public static Dictionary<string, GameObject> cutsceneDataBase = new Dictionary<string, GameObject>();
    public static GameObject activeCutscene;

    private void Awake()
    {
        Instance = this;
        InitializeCutsceneDataBase();

        foreach (var cutscene in cutsceneDataBase)
        {
            cutscene.Value.SetActive(false);
        }
    }

    private void Start()
    {
        StartCutscene("Cutscene_1");
    }
    
    private void InitializeCutsceneDataBase()
    {
        cutsceneDataBase.Clear();

        for (int i = 0; i < cutscenes.Count; i++)
        {
            cutsceneDataBase.Add(cutscenes[i].cutSceneKey, cutscenes[i].cutSceneObject);
        }
    }
    
    public void EndCutscene(string sceneName = "")
    {
        if (activeCutscene != null)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            activeCutscene.SetActive(false);
            activeCutscene = null;
        }
    }
    
    public void StartCutscene(string cutsceneKey)
    {
        if (!cutsceneDataBase.ContainsKey(cutsceneKey))
        {
            Debug.LogError($"Катсцены с ключом \"{cutsceneKey}\" нету в cutsceneDataBase");
            return;
        }

        if (activeCutscene != null)
        {
            if (activeCutscene == cutsceneDataBase[cutsceneKey])
            {
                return;
            }
        }

        activeCutscene = cutsceneDataBase[cutsceneKey];

        foreach (var cutscenes in cutsceneDataBase)
        {
            cutscenes.Value.SetActive(false);
        }
        
        cutsceneDataBase[cutsceneKey].SetActive(true);
    }
    

    
}

[System.Serializable]
public struct CutsceneStruct
{
    public string cutSceneKey;
    public GameObject cutSceneObject;
}