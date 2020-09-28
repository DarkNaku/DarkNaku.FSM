using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class FSMBehaviourV2 : MonoBehaviour {
    [SerializeField][HideInInspector] private List<FSMStateV2> _states = new List<FSMStateV2>();
    [SerializeField][HideInInspector] private string _firstState = null;

    private Dictionary<string, FSMStateV2> _stateTable = new Dictionary<string, FSMStateV2>();
    private FSMStateV2 _current = null;
    private FSMStateV2 _next = null;

    public void TriggerEvent(string eventName, bool immediately = false) {
        if (_current == null) {
            Debug.LogErrorFormat("[FSM-{0}] TriggerEvent : Current State is null.", name);
            return;
        }

        var nextStateName = _current[eventName];

        if (_stateTable.ContainsKey(nextStateName)) {
            _next = _stateTable[nextStateName];
            if (immediately) TransferState();
        } else {
            Debug.LogErrorFormat("[FSM-{0}] TriggerEvent : Event({1}) dosen't exist in the state({2})", 
                name, eventName, _current.StateName);
        }
    }

    public List<string> GetStateNames() {
        var states = new List<string>();

        if (_states == null) return states;

        for (int i = 0; i < _states.Count; i++) {
            states.Add(_states[i].StateName);
        }

        return states;
    }

    private void Awake() {
        for (int i = 0; i < _states.Count; i++) {
            _states[i].OnInitialize.Invoke(this);
            _stateTable.Add(_states[i].StateName, _states[i]);
        }

        if (_stateTable.ContainsKey(_firstState)) {
            _current = _stateTable[_firstState];
        }
    }

    /*
    private void OnValidate() {
        for (int i = 0; i < _states.Count; i++) {
            if ((i + 1) >= _states.Count) break;

            var state = _states[i];

            for (int j = i + 1; j < _states.Count; j++) {
                Debug.AssertFormat(string.Equals(_states[j].StateName, state.StateName) == false,
                    "[FSMBehaviourV2] OnValidate : State duplicated - {0}", state.StateName);
            }
        }
    }
    */

    private void FixedUpdate() {
        _current?.FixedUpdateAction.Invoke(this);
    }

    private void Update() {
        _current?.UpdateAction.Invoke(this);
    }

    private void LateUpdate() {
        _current?.LateUpdateAction.Invoke(this);
        TransferState();
    }

    private void TransferState() {
        if (_next == null) return;

        var next = _next;
        _next = null;

        _current?.OnLeave.Invoke(this);
        _current = next;
        _current.OnEnter.Invoke(this);
    }
}
