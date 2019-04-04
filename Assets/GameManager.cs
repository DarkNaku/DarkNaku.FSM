using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_STATE { INITIALIZE, IDLE, SELECTED, DRAG }

public class GameManager : FSMBehaviour<GAME_STATE, GameManager> {
	[SerializeField] private Camera _camera = null;

	private GameObject Target { get; set; }

	private void Start() {
		AddStates(new InitializeState(), new IdleState(), new SelectedState(), new DragState());
	}

	private void OnEnable() {
		OnTransition.AddListener(OnFireTransition);
	}

	private void OnDisable() {
		OnTransition.RemoveListener(OnFireTransition);
	}

	private void Update() {
		Execute();
	}

	private void OnFireTransition(GAME_STATE prev, GAME_STATE next) {
		Debug.Log(string.Format("{0} => {1}", prev, next));
	}

	public class InitializeState : FSMState<GAME_STATE, GameManager> {
		private bool _initializing = false;
		public override GAME_STATE State { get { return GAME_STATE.INITIALIZE; } }

		public override void OnEnter() {
			Machine.StartCoroutine(CoInitialize());
		}

		public override void OnLeave() {
			_initializing = false;
		}

		private IEnumerator CoInitialize() {
			_initializing = true;
			Debug.Log("Initialize Start");

			float time = 0F;

			while (time < 3F) {
				if (_initializing == false) yield break;
				Debug.Log(string.Format("{0} Sec", time));
				yield return null;
				time += Time.deltaTime;
			}

			Debug.Log("Initialize End");
			_initializing = false;
		}
		
		public override GAME_STATE NextState() {
			return _initializing ? State : GAME_STATE.IDLE;
		}
	}

	public class IdleState : FSMState<GAME_STATE, GameManager> {
		public override GAME_STATE State { get { return GAME_STATE.IDLE; } }
		
		public override GAME_STATE NextState() {
			if (Input.GetMouseButtonDown(0)) {
				Ray ray = Machine._camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;

				if (Physics.Raycast(ray, out hit)) {
					Machine.Target = hit.transform.gameObject;
					return GAME_STATE.SELECTED;
				} else {
					Machine.Target = null;
					return GAME_STATE.DRAG;
				}
			}

			return State;
		}
	}

	public class SelectedState : FSMState<GAME_STATE, GameManager> {
		public override GAME_STATE State { get { return GAME_STATE.SELECTED; } }

		public override GAME_STATE NextState() {
			if (Input.GetMouseButtonUp(0)) {
				Machine.Target = null;
				return GAME_STATE.IDLE;
			}

			return State;
		}

		public override void Update() { 
			Vector3 pos = Machine._camera.ScreenToWorldPoint(Input.mousePosition);
			pos.z = Machine.Target.transform.position.z;
			Machine.Target.transform.position = pos;
		}
	}

	public class DragState : FSMState<GAME_STATE, GameManager> {
		public override GAME_STATE State { get { return GAME_STATE.DRAG; } }
		
		public override GAME_STATE NextState() {
			if (Input.GetMouseButtonUp(0)) {
				return GAME_STATE.IDLE;
			}

			return State;
		}
	}
}
