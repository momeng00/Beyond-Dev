using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private InputSystem _registeredInputSystem;
    private readonly HashSet<CharacterControl> characters = new HashSet<CharacterControl>();
    private readonly HashSet<CharacterControl> pausedCharacters = new HashSet<CharacterControl>();
    private bool charactersPaused;

    public void RegisterCharacter(CharacterControl character)
    {
        if (character == null) return;
        characters.Add(character);
        if (charactersPaused && character.isActiveAndEnabled && pausedCharacters.Add(character))
            character.PauseCharacter(this);
    }

    public void UnregisterCharacter(CharacterControl character)
    {
        characters.Remove(character);
        if (pausedCharacters.Remove(character) && character != null)
            character.UnPauseCharacter(this);
    }

    public void PauseCharacters()
    {
        if (charactersPaused) return;
        charactersPaused = true;
        foreach (CharacterControl character in characters)
        {
            if (character == null || !character.isActiveAndEnabled) continue;
            if (pausedCharacters.Add(character)) character.PauseCharacter(this);
        }
    }

    public void ResumeCharacters()
    {
        foreach (CharacterControl character in pausedCharacters)
            if (character != null) character.UnPauseCharacter(this);
        pausedCharacters.Clear();
        charactersPaused = false;
    }

    public IEnumerator WaitForCharactersRespawn()
    {
        var pending = new List<Coroutine>();
        foreach (CharacterControl character in pausedCharacters)
        {
            if (character != null && character.isActiveAndEnabled &&
                character.RespawnCoroutine != null)
                pending.Add(character.RespawnCoroutine);
        }
        foreach (Coroutine respawn in pending) yield return respawn;
    }

    private void OnDisable()
    {
        ResumeCharacters();
    }

    private void OnDestroy()
    {
        ResumeCharacters();
        if (_registeredInputSystem == null)
            return;

        _registeredInputSystem.DeregisterAction(KeyState.Play_Key, KeyCode.Escape, GamePause);

        _registeredInputSystem = null;
    }

    
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
    public UIWindow pauseMenu;
    private UIBase _pauseMenuInstance;
    public Stage startStage;
    public Stage currentStage;
    public UnityEvent sceneStart;
    private bool sceneFlag=false;
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
        if (UIManager.instance.showns.Count > 0)
            UIManager.instance.HideLast();
        else if (pauseMenu != null && InputSystem.Instance.CanProcessGameplayInput)
            pauseMenu.Open();
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
        _registeredInputSystem = InputSystem.Instance;
        _registeredInputSystem.RegisterAction(KeyState.Play_Key, KeyCode.Escape, GamePause);
        if (!sceneFlag)
        {
            sceneFlag = true;
            sceneStart?.Invoke();
        }
    }
    private Coroutine holdCoroutine;
    public float holdTime = 0.7f;
    // Update is called once per frame
    void Update()
    {
        if (!InputSystem.Instance.CanProcessGameplayInput)
        {
            if (holdCoroutine != null) StopCoroutine(holdCoroutine);
            holdCoroutine = null;
            return;
        }
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
        NextStage(startStage);
    }
    IEnumerator HoldReset()
    {
        yield return new WaitForSeconds(holdTime);
        holdCoroutine = null;
        if (InputSystem.Instance.CanProcessGameplayInput) ResetGame();
    }

    public void ResetGame()
    {
        OnReset?.Invoke();
    }
    public void GoToEnd()
    {
        Debug.Log("왜 실행됨?");
        SceneManager.LoadScene(finalClearSceneName);
    }

    public void SceneChange(string sceneName)
    {
        Debug.Log("Scene채인치");
        SceneManager.LoadScene(sceneName);
    }
}
