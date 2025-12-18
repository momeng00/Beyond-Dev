using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.Cinemachine.Samples;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Cinemachine.Samples.PlatformerCamera2D;

public class MainCameraController : MonoBehaviour
{
    //카메라의 기능을 블럭처럼 끼고 뺄 수 있도록 구현을 하려고 함.
    //현재 생각이 되고 있는 카메라의 기능은 원하는 곳으로 위치가 이동이 되었는가?
    //FSM을 응용해서 원하는 기능을 끼고 끌 수 있도록 제작이 되어야함. Enter와 Exit, Update
    //위를 이용해서 Start문과 Update문을 구현하고 이동 및 상태를 컨트롤 할 수 있도록 합시다.
    //ScritableObject를 사용할 수 있다면 사용해서 구현을 해보도록 합시다.
    //체크가 지속적으로 이뤄지지않도록 구현이 되는 것이 목표.
    //기본 Base를 만들고 상태를 전환하는 기능을 만들어서 상태를 전환하는 것이 목표. 
    //Main시스템을 통해서 Left, Right, Falling_R, Falling_L을 만들어 봅시다.
    //PlatfomerCamera2D의 기능을 가져와야하고 
    //만약 Update문이나 카메라의 상태를 체크해야한다면 체크하는 기능을 가져올 수 있어야함
    //Platfromer Camera를 하나 가지고 있고 Check상태로 들어가면 나머지를 무시하고 Check만 진행하는 방식
    //캐릭터에 대한 정보 및 캐릭터에 대한 중심 오브젝트, Cinemachine카메라, 


    //찍고있는 카메라를 변경하는 것
    //찍고있는 카메라의 값 변경
    //시네머신에서 카메라의 움직임에 따라 흔들림의 강약조절
    //본 기능은 Cinemachine Camera 컴포넌트의 noise 드롭다운에서 활성화
    //Cinemachin Basic Multi Channel Perlin 컴포넌트에서 두개의 게인값으로 조절
    public float criticalVelocity;
    private CameraBehaviorBase cameraBehavior;
    public PlatformerCamera2D platformerCamera2D;
    private HashSet<Rigidbody2D> _players = new HashSet<Rigidbody2D>();
    private StateCameras currentStateCameras = StateCameras.Right;
    private bool isRight = true;
    public bool IsRight
    {
        set { isRight = value; }
    }
    public void Register(Rigidbody2D rb)
    {
        _players.Add(rb);
    }
    public void Unregister(Rigidbody2D rb)
    {
        _players.Remove(rb);
    }

    public bool GetAverageVelocityY()
    {
        //if (Time.frameCount != _lastCalculatedFrame)
        //{
        //    _lastCalculatedFrame = Time.frameCount;
        //} 프레임 캐싱이라고 프레임 id를 저장할 수 있음. 필요해지면 쓸것

        if (_players.Count == 0) return false;
        
        float total = 0f;
        foreach (var rb in _players)
        {
            total += rb.linearVelocity.y;
        }
        return (total / _players.Count) < criticalVelocity;
    }
    private static MainCameraController _instance;
    public static MainCameraController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<MainCameraController>();
                if (_instance == null)
                    Debug.Log("CameraController가 없음");
            }
            return _instance;
        }
    }
    public void ChangeCamera(StateCameras state)
    {
        if (currentStateCameras < state)
        {
            currentStateCameras = state;
        }   
    }
    public void ReportCameraSelect(StateCameras state)
    {
        platformerCamera2D.ChangeCameraState(state);
    }
    public void ChangeCameraState()
    {
        cameraBehavior.CameraStateEnter();
    }
    public void ExitCameraState()
    {
        cameraBehavior.CameraStateExit();
    }

    private void Update()
    {   
        if(cameraBehavior != null)
            cameraBehavior.CameraStateUpdate();
    }
    private void LateUpdate()
    {
        ReportCameraSelect(currentStateCameras);
        if (isRight)
            currentStateCameras = StateCameras.Right;
        else 
            currentStateCameras = StateCameras.Left;

    }

    public void CameraReset()
    {

        var brain = Camera.main.GetComponent<CinemachineBrain>();
        var activeCam = brain.ActiveVirtualCamera as CinemachineVirtualCameraBase;
        Debug.Log(activeCam);
        if (activeCam != null)
        {
            // [강력한 방법] "이전 프레임의 위치 정보는 유효하지 않다"고 선언
            // 이렇게 하면 시네머신은 댐핑 계산을 포기하고, 타겟의 현재 위치로 카메라를 즉시 강제 이동시킵니다.
            activeCam.PreviousStateIsValid = false;
        }

    }

}