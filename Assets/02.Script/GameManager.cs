using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    private int stage = 1;
    private Dictionary<int,List<IClearCondition>> condition = new Dictionary<int, List<IClearCondition>>();
    private Dictionary<int, Action> clearAction = new Dictionary<int, Action>();
    public Action initAction;
    public Action OnReset;
    public Action<GameState> OnGameStateChanged;
    public GameState currentGameState;
    public string finalClearSceneName = "Ending";
    private static GameManager _instance;
    private static bool _isQuitting = false;
    public UIBase pauseMenu;
    private UIBase _pauseMenuInstance;
    public Stage startStage;
    public Stage currentStage;
    
    public static GameManager Instance
    {

        get
        {
            if (_isQuitting) return null;
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<GameManager>();
                if (_instance == null)
                {
                    _instance = new GameObject("GameSystem").AddComponent<GameManager>();
                    Debug.Log("오류상황");
                }

            }
            return _instance;
        }
    }

    public void StartStage()
    {
        NextStage(startStage);
    }


    public void NextStage(Stage nextStage)
    {
        if (currentStage != null)
        {
            Debug.Log("스테이지 나가는거 실행");
            currentStage.StageExit();
        }

        Debug.Log("현 스테이지 변경");
        currentStage = nextStage;
        Debug.Log("스테이지 진입 실행");
        currentStage.StageEnter();
    }
    public void GamePause()
    {
        Debug.Log("GamePause진입");
        if (InputSystem.Instance.keyState != KeyState.Pause)
        {
            Debug.Log("키타입 확인");
            if (UIManager.instance.showns.Count <= 0)
            {
                Debug.Log("보여지고있는 UI 없음 확인");
                InputSystem.Instance.keyState = KeyState.Pause;
                if (pauseMenu != null)
                {
                    if (_pauseMenuInstance == null)
                    {
                        _pauseMenuInstance = Instantiate(pauseMenu); ;
                        _pauseMenuInstance.name = pauseMenu.name;
                        Debug.Log("창 생성");
                    }
                    _pauseMenuInstance.Open();
                    Debug.Log("창 오픈");
                }
            }
        }
        Debug.Log("반환");
    }
    public void RegisterCondition(int stage, IClearCondition condition)
    {
        if (!this.condition.ContainsKey(stage))
        {
            this.condition[stage] = new List<IClearCondition>();
        }
        this.condition[stage].Add(condition);
    }

    public void RegisterClearAction(int stage, Action act)
    {
        if (!clearAction.ContainsKey(stage))
        {
            clearAction[stage] = act;
            return;
        }
        clearAction[stage] += act;
    }
    public void RegisterInitAction(Action act)
    {
        initAction += act;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnGameStateChanged?.Invoke(currentGameState);
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.Escape, GamePause);
    }
    private Coroutine holdCoroutine;
    public float holdTime = 0.7f;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            holdCoroutine = StartCoroutine(HoldReset());
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            if (holdCoroutine != null)
            {
                StopCoroutine(holdCoroutine);
                holdCoroutine = null;
            }
        }
    }
    public void StartGameNow()
    {
        currentGameState = GameState.Playing;
    }
    IEnumerator HoldReset()
    {
        yield return new WaitForSeconds(holdTime);
        ResetGame();
    }

    void ResetGame()
    {
        OnReset?.Invoke();
    }
    public void GoToEnd()
    {
        Debug.Log("왜 실행됨?");
        SceneManager.LoadScene(finalClearSceneName);
    }
}
