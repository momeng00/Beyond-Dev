using System.Collections.Generic;
using UnityEngine;

public class PopUpDataManager : MonoBehaviour
{
    private static PopUpDataManager _instance;
    public static PopUpDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<PopUpDataManager>();
                if (_instance == null)
                {
                    _instance = new GameObject("PopUpDataManager").AddComponent<PopUpDataManager>();
                }

            }
            return _instance;
        }
    }
    private Dictionary<string, PopUpData> dataDictionary = new Dictionary<string, PopUpData>();
}