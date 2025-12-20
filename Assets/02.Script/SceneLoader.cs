using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject UI;
    public PlayableDirector director;

    public void STR_BTN()
    {
        if ((float)director.time <= 1f)
        {
            director.playableGraph.GetRootPlayable(0).SetSpeed(1f);
            UI.SetActive(false);
            Debug.Log("타임 1 이하");
        }
    }
    
}
