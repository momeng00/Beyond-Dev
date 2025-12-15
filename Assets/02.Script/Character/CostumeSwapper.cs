using UnityEngine;
using UnityEngine.U2D.Animation;

public class CostumeSwapper : MonoBehaviour
{
    public CostumeData costumeData;
    private SpriteLibrary mySpriteLibrary;

    private void Awake()
    {
        mySpriteLibrary = GetComponent<SpriteLibrary>();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += SwapCostume;
        }
    }


    private void SwapCostume(GameState newState)
    {
        if (costumeData == null) return;

        SpriteLibraryAsset newAsset = costumeData.GetLibrary(newState);
        if (newAsset != null)
        {
            mySpriteLibrary.spriteLibraryAsset = newAsset;
        }
    }
}