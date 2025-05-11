using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class CutsceneManager: MonoBehaviour
{
    public static CutsceneManager Instance;
    [SerializeField] private List<CutsceneStruct> cutscenes = new ();
    private static readonly Dictionary<string, GameObject> CutsceneDataBase = new ();
    public static GameObject ActiveCutscene;

    private void Awake()
    {
        Instance = this;
        InitializeCutsceneDataBase();

        foreach (var cutscene in CutsceneDataBase)
        {
            cutscene.Value.SetActive(false);
        }
    }

    private void InitializeCutsceneDataBase()
    {
        CutsceneDataBase.Clear();

        for (int i = 0; i < cutscenes.Count; i++)
        {
            CutsceneDataBase.Add(cutscenes[i].cutSceneKey, cutscenes[i].cutSceneObject);
        }
    }
    
    private void Start()
    {
        StartCutscene("Cutscene_1");
    }
    
    private void Update()
    {
        if (CutsceneDataBase.All(c => c.Value == false))
            EndCutscene("Main menu");
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndCutscene("Main menu");
        }
    }
    
    public void StartCutscene(string cutsceneKey)
    {
        if (!CutsceneDataBase.ContainsKey(cutsceneKey))
        {
            return;
        }

        if (ActiveCutscene != null && ActiveCutscene == CutsceneDataBase[cutsceneKey])
        {
            return;
        }

        ActiveCutscene = CutsceneDataBase[cutsceneKey];
        foreach (var cutscene in CutsceneDataBase)
        {
            if (cutscene.Value != ActiveCutscene)
                cutscene.Value.SetActive(false);
        }
        CutsceneDataBase[cutsceneKey].SetActive(true);
    }
    
    public void EndCutscene(string sceneName)
    {
        if (ActiveCutscene != null)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            ActiveCutscene.SetActive(false);
            ActiveCutscene = null;
        }
    }
}

[System.Serializable]
public struct CutsceneStruct
{
    public string cutSceneKey;
    public GameObject cutSceneObject;
}