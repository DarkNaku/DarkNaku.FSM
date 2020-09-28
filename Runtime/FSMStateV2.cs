using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class FSMStateV2 {
    [System.Serializable]
    public class Transition {
        [SerializeField] private string _eventName = null;
        public string EventName { get { return _eventName; } }

        [SerializeField] private string _guid = null;
        public string GUID { get { return _guid; } }
    }

    [System.Serializable]
    public class EventAction : UnityEvent<FSMBehaviourV2> { }

    [SerializeField] private string _guid = null;
    public string GUID { 
        get { 
            if (string.IsNullOrEmpty(_guid)) {
                Debug.LogWarning("[FSMStateV2] GUID : Is null. Something wrong.");
                _guid = Guid.NewGuid().ToString();
            }

            return _guid; 
        } 
    }

    [SerializeField] private List<Transition> _transitions = new List<Transition>();

    [SerializeField] private string _stateName = null;
    public string StateName { get { return _stateName;  } }

    [SerializeField] private EventAction _onInitialize = new EventAction();
    public EventAction OnInitialize { get { return _onInitialize; } }

    [SerializeField] private EventAction _onEnter = new EventAction();
    public EventAction OnEnter { get { return _onEnter; } }

    [SerializeField] private EventAction _onLeave = new EventAction();
    public EventAction OnLeave { get { return _onLeave ; } }

    [SerializeField] private EventAction _fixedUpdateAction = new EventAction();
    public EventAction FixedUpdateAction { get { return _fixedUpdateAction; } }

    [SerializeField] private EventAction _updateAction = new EventAction();
    public EventAction UpdateAction { get { return _updateAction; } }

    [SerializeField] private EventAction _lateUpdateAction = new EventAction();
    public EventAction LateUpdateAction { get { return _lateUpdateAction; } }

    private Dictionary<string, string> _transitionTable = null;
    public string this [string eventName] {
        get {
            if (_transitionTable == null) {
                _transitionTable = new Dictionary<string, string>();

                for (int i = 0; i < _transitions.Count; i++) {
                    _transitionTable.Add(_transitions[i].EventName, _transitions[i].GUID);
                }
            }

            return _transitionTable[eventName];
        }
    }

    public FSMStateV2() {
        _guid = Guid.NewGuid().ToString();
    }
}
