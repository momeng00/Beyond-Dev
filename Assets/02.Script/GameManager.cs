using System;
using System.Collections;
using System.Collections.Generic;
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
    public string finalClearSceneName = "ClearScene";
    private static GameManager _instance;
    private static bool _isQuitting = false;
    public UIBase pauseMenu;
    private UIBase _pauseMenuInstance;
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
    
    public void CheckClear()
    {
        if (condition.Count <= 1)
            return;
        if (condition[stage]==null)
        {
            return;
        }
        bool clear = true;
        foreach(var condition in condition[stage])
        {
            if (!condition.IsSatisfied())
            {
                clear = false; 
            }
        }
        if (clear)
        {
            clearAction[stage]?.Invoke();
            //임시용
            NextStage();
            initAction?.Invoke();
        }
    }
    public void NextStage() //임시용
    {
        if (condition.ContainsKey(stage+1))
        {
            stage++;
        }
        else
        {
            Debug.Log("stage ERROR 끝이 났거나 오류가 발생!");
            stage = 0;
        }
    }
    public void NextStage(Stage nextStage)
    {
        currentStage.StageExit();

        currentStage = nextStage;

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
        RegisterCondition(0,new DummyCondition());
        OnGameStateChanged?.Invoke(currentGameState);
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.Escape, GamePause);
    }
    private Coroutine holdCoroutine;
    public float holdTime = 0.7f;
    // Update is called once per frame
    void Update()
    {
        CheckClear();
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
}
public class DummyCondition : IClearCondition
{
    public void ClearAction()
    {
        
    }

    public bool IsSatisfied() => false; 
}
