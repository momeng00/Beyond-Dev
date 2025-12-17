using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "CostumeData", menuName = "Scriptable Objects/CostumeData")]
public class CostumeData : ScriptableObject
{

    public List<CostumeEntry> costumeList;
    public SpriteLibraryAsset GetLibrary(GameState state)
    {
        foreach (var pair in costumeList)
        {
            if (pair.state == state) return pair.library;
        }
        Debug.Log("설정된 Library가 없음");
        return null;
    }
    [System.Serializable]
    public struct CostumeEntry
    {
        public GameState state;       // 시점 (Enum)
        public SpriteLibraryAsset library; // 적용할 라이브러리
    }
}

public enum GameState
{
    Openning,
    Playing,
    Ending
}
