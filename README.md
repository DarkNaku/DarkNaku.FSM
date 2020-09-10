[![](https://img.shields.io/badge/release-1.0.0-brightgreen)](https://github.com/DarkNaku/DarkNaku.FSM/tree/release/1.0.0) ![](https://img.shields.io/github/forks/DarkNaku/DarkNaku.FSM) ![](https://img.shields.io/github/stars/DarkNaku/DarkNaku.FSM) ![](https://img.shields.io/github/issues/DarkNaku/DarkNaku.FSM?color=red) ![](https://img.shields.io/badge/license-MIT-green) [![](https://img.shields.io/github/watchers/DarkNaku/DarkNaku.FSM?label=Watch)](https://github.com/DarkNaku/DarkNaku.FSM/subscription)



# 개요

MonoBehaviour 베이스에 간단한 FSM(Finite State Machine) 프레임워크입니다.



## 사용법

1. 상태를 정의할 enum 타입을 선언합니다.
```c#
public enum CAMERA_STATE { IDLE, MOVE, LOCK }
```

2. FSMBehaviour<S, M>을 상속받는 머신 클래스를 선언합니다.

```c#
public class CameraController : FSMBehaviour<CAMERA_STATE, CameraController> {
}
```

3. FSMState<S, M>을 상속받는 상태 클래스들을 선언합니다.
```c#
public class IdleState : FSMState<CAMERA_STATE, CameraController> {
	public override CAMERA_STATE State { get { return CAMERA_STATE.IDLE; } }
}

public class MoveState : FSMState<CAMERA_STATE, CameraController> {
	public override CAMERA_STATE State { get { return CAMERA_STATE.MOVE; } }
}

public class LockState : FSMState<CAMERA_STATE, CameraController> {
	public override CAMERA_STATE State { get { return CAMERA_STATE.LOCK; } }
}
```

4. 머신의 Awake 이벤트에서 상태 객체들을 생성하여 머신에 등록합니다.
```c#
public class CameraController : FSMBehaviour<CAMERA_STATE, CameraController> {
	private void Awake() {
		AddState(new IdleState());
		AddState(new MoveState());
		AddState(new LockState());
	}
}
```

5. 머신의 초기화 함수를 호출합니다.
```c#
public class CameraController : FSMBehaviour<CAMERA_STATE, CameraController> {
	private void Awake() {
		AddState(new IdleState());
		AddState(new MoveState());
		AddState(new LockState());
		Initialize(CAMERA_STATE.IDLE);
	}
}
```



## FSMBehaviour\<S, M>

머신 클래스 타입이며, S는 상태를 나타내는 enum 타입이고 M은 FSMBehaviour를 상속받는 클래스의 타입입니다.



#### 속성

```c#
public S State;
```

현재 머신의 상태를 나타내는 속성입니다. 읽기 전용이며, 상속받은 클래스 내부에서만 쓰기가 가능합니다.

```c#
public bool Paused;
```

머신을 중지 시킬 수 있는 속성입니다. 기본값은 false 입니다.

```c#
public TransitionEvent OnTransition;
```

상태 전환 이벤트 등록 속성입니다.



#### 함수

```c#
protected void Initialize(S state);
```

머신의 초기 상태를 지정하기 위한 초기화 함수 입니다. 반드시 상태를 등록한 이후에 호출해야 합니다.

```c#
protected void AddState(FSMState<S, M> state);
```

머신에 상태를 추가하기 위한 함수입니다.

```c#
protected void RemoveState(S state);
```

머신에서 상태를 삭제하기 위한 함수입니다.



## FSMState\<S, M>

상태 클래스이며 S는 enum 타입, M은 머신 클래스 타입이며, FSMBehaviour<S, M>에서 사용한 타입과 동일한 타입을 선언해야 합니다. 



#### 속성

```c#
protected M FSM;
```

머신 참조 속성 입니다. 상태 내부에서 머신의 내부의 속성 및 함수를 참조하기 위한 용도 입니다.

```c#
public abstract S State;
```

상태를 정의하는 속성이며, 상태 클래스를 상속받는 경우 반드시 구현해 줘야 합니다.



#### 함수

```c#
public virtual void OnInitialize();
```

상태 객체가 머신에 등록될 때 한번 호출 됩니다.

```c#
public virtual S OnEnter();
```

상태에 진입할 때 한번 호출 됩니다. 다음 상태를 반환해야 합니다.

```c#
public virtual void OnLeave();
```

상태에서 빠저나올 때 한번 호출 됩니다.

```c#
public virtual S FixedUpdate();
```

MonoBehaviour의 FixedUpdate와 동일한 주기로 호출 됩니다. 다음 상태를 반환해야 합니다.

```c#
public virtual S Update();
```

MonoBehaviour의 Update와 동일한 주기로 호출 됩니다. 다음 상태를 반환해야 합니다.

```c#
public virtual S LateUpdate();
```

MonoBehaviour의 LateUpdate와 동일한 주기로 호출 됩니다. 다음 상태를 반환해야 합니다.