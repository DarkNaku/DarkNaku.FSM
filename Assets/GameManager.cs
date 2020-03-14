using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_STATE { NONE, INITIALIZE, IDLE, SELECTED, DRAG }

public class GameManager : FSMBehaviour<GAME_STATE, GameManager> {
	[SerializeField] private Camera _camera = null;

	private GameObject Target { get; set; }

	private void Awake() {
		AddStates(new NoneState(), new InitializeState(), new IdleState(), new SelectedState(), new DragState());
	}

	protected override void OnTransition(GAME_STATE prevState, GAME_STATE nextState) {
		Debug.Log(string.Format("{0} => {1}", prevState, nextState));
	}

	public class NoneState : FSMState<GAME_STATE, GameManager> {
		public override GAME_STATE State { get { return GAME_STATE.NONE; } }
	}

	public class InitializeState : FSMState<GAME_STATE, GameManager> {
		private bool _initializing = false;
		public override GAME_STATE State { get { return GAME_STATE.INITIALIZE; } }

		public override GAME_STATE OnEnter() {
			FSM.StartCoroutine(CoInitialize());
			return State;
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
		
		public override GAME_STATE Update() {
			return _initializing ? State : GAME_STATE.IDLE;
		}
	}

	public class IdleState : FSMState<GAME_STATE, GameManager> {
		public override GAME_STATE State { get { return GAME_STATE.IDLE; } }
	
		public override GAME_STATE Update() {
			if (Input.GetMouseButtonDown(0)) {
				Ray ray = FSM._camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;

				if (Physics.Raycast(ray, out hit)) {
					FSM.Target = hit.transform.gameObject;
					return GAME_STATE.SELECTED;
				} else {
					FSM.Target = null;
					return GAME_STATE.DRAG;
				}
			}

			return State;
		}
	}

	public class SelectedState : FSMState<GAME_STATE, GameManager> {
		public override GAME_STATE State { get { return GAME_STATE.SELECTED; } }

		public override GAME_STATE Update() { 
			if (Input.GetMouseButtonUp(0)) {
				FSM.Target = null;
				return GAME_STATE.IDLE;
			}

			Vector3 pos = FSM._camera.ScreenToWorldPoint(Input.mousePosition);
			pos.z = FSM.Target.transform.position.z;
			FSM.Target.transform.position = pos;

			return State;
		}
	}

	public class DragState : FSMState<GAME_STATE, GameManager> {
		public override GAME_STATE State { get { return GAME_STATE.DRAG; } }
		
		public override GAME_STATE Update() {
			return Input.GetMouseButtonUp(0) ? GAME_STATE.IDLE : State;
		}
	}
}
