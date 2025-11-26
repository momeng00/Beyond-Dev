using Unity.Cinemachine;
using Unity.Cinemachine.Samples;
using UnityEngine;
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
    private CameraBaseState cameraState;
    public CinemachineCamera cinemachineCamera;
    public PlatformerCamera2D platformerCamera2D;
    public MainCameraController Instance
    {
        get{ return _Instance; }
        private set { }
    }
    private MainCameraController _Instance;
    public void ChangeCamera(StateCameras state)
    {

    }
    public void ChangeCameraState()
    {
        cameraState.CameraStateExit();
        cameraState.CameraStateEnter();
    }
    private void Update()
    {
        cameraState.CameraStateUpdate();
    }
}