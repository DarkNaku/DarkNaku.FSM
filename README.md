# 개요
MonoBehaviour 베이스에 간단한 FSM(Finite State Machine) 프레임워크입니다.

## 개발 예정 기능

* 상태별로 전환 가능한 다음 상태 지정 기능
* Execute 함수를 명시적인 호출하는 대신 Update와 FixedUpdate 예약 함수 오버라이드

## 클래스

**FSMBehaviour\<S, M>** 는 머신 클래스이고 S는 enum 타입 M은 FSMBehaviour를 상속받는 클래스의 타입입니다.

```
public enum CAMERA_STATE { IDLE, MOVE, LOCK }
public class CameraController : FSMBehaviour<CAMERA_STATE, CameraController>
```

**FSMState\<S, M>** 상태 클래스이며 S와 M의 타입은 머신 클래스와 동일한 타입을 받아야 합니다.

```
public class IdleState : FSMState<CAMERA_STATE, CameraController>
```

## 속성

### FSMBehaviour\<S, M>

**public S State**

현재 상태를 확인하거나 변경 할 수 있는 속성입니다. 
주의 : 스테이트 반환 값이 아닌 외부에서 상태를 강제 변경하는 경우 스테이트 내부의 코루틴과 같은 동작이 다른 스테이트 중에 계속 동작하는 등과 같은 문제가 발생 할 수 있습니다.

**public bool IsRunning**

머신이 현재 동작 중 인지 확인 할 수 있는 속성입니다.

### FSMState\<S, M>

**protected M Machine**

머신을 참조하고 있는 속성 입니다.

**public abstract S State**

상태를 정의하는 속성이며, 상태 클래스를 상속받는 경우 반드시 구현해 줘야 합니다.

**protected object Param**

다음 상태에 전달할 매개변수를 일시적으로 전달할 용도의 변수이며 다음 상태의 OnEnter 이벤트의 파라미터로 전달됩니다.

## 함수

### FSMBehaviour\<S, M>

**public void StartFSM(S state)**

머신 동작을 시작하기 위한 함수입니다.

**public void StopFSM()**

머신 동작을 중지하기 위한 함수입니다.

**protected void AddStates(params FSMState\<S, M>[] states)**

머신에 상태를 추가하기 위한 함수입니다.

**protected void RemoveState(S state)**

머신에서 상태를 삭제하기 위한 함수입니다.

### FSMState\<S, M>

**public virtual void OnInitialize()**

상태 객체가 머신에 등록될 때 한번 호출 됩니다.

**public virtual S OnEnter()**

상태에 진입할 때 한번 호출 됩니다. 다음 상태를 반환해야 합니다.

**public virtual void OnLeave()**

상태에서 빠저나올 때 한번 호출 됩니다.

**protected virtual void OnTransition(S prevState, S nextState)**

머신의 상태가 변경 되는 경우 이전 상태와 변경된 상태를 매개변수로 호출 됩니다.

**public virtual S FixedUpdate()**

MonoBehaviour의 FixedUpdate와 동일한 주기로 호출 됩니다. 다음 상태를 반환해야 합니다.

**public virtual S Update()**

MonoBehaviour의 Update와 동일한 주기로 호출 됩니다. 다음 상태를 반환해야 합니다.

**public virtual S LateUpdate()**

MonoBehaviour의 LateUpdate와 동일한 주기로 호출 됩니다. 다음 상태를 반환해야 합니다.

**public virtual S EndOfFrame()**

MonoBehaviour의 라이프 사이클 중 프레임의 마지막에 호출 됩니다. 다음 상태를 반환해야 합니다.
