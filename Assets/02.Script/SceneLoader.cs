using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public PlayableDirector director;

    public void SetTimelineSpeed(float speed)
    {
        if (director.playableGraph.IsValid())
        {
            // 타임라인의 재생 속도를 직접 조절
            director.playableGraph.GetRootPlayable(0).SetSpeed(speed);
        }
    }
    private void Start()
    {
        Time.timeScale = 1.0f;
    }
    public string testSceneName;    
    private bool flag = false;
    public void STR_BTN()
    {
        if (flag)
        {
            Debug.Log("화면 이동");
        }
        else
        {
            Time.timeScale = 16.0f;
            flag = true;
        }

    }
    public void FlagSceneTest()
    {
        if (flag)
        {
            Time.timeScale = 1.0f;
            Debug.Log("화면 이동");
        }
        else
        {
            flag = true;
        }
    }
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    private void OnDisable()
    {
        Time.timeScale = 1.0f;
    }
}
