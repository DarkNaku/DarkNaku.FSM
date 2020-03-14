using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

public abstract class FSMState<S, M>
                            where S : struct, System.IConvertible, System.IComparable
                            where M : FSMBehaviour<S, M> {

    protected M FSM { get; private set; }
    public abstract S State { get; }
    public virtual void OnInitialize() { }
    public virtual S OnEnter() { return State; }
    public virtual void OnLeave() { }
    public virtual S FixedUpdate() { return State; }
    public virtual S Update() { return State; }
    public virtual S LateUpdate() { return State; }
    public virtual S EndOfFrame() { return State; }

    public void Initialize(M machine) {
        Assert.IsNotNull(machine);
        FSM = machine;
        OnInitialize();
    }
}

public class FSMBehaviour<S, M> : MonoBehaviour
                            where S : struct, System.IConvertible, System.IComparable
                            where M : FSMBehaviour<S, M> {

    private Dictionary<S, FSMState<S, M>> _states = new Dictionary<S, FSMState<S, M>>();

    private S _state;
    public S State {
        get { return _state; }
        protected set {
            if (value.CompareTo(_state) == 0) return;
            S prevState = _state;
            _states[_state].OnLeave();
            _state = value;
            S state = _states[_state].OnEnter();
            OnTransition(prevState, _state);
            State = state;
        }
    }

    public bool IsRunning { get; private set; }

    public void StartFSM(S state) {
        Assert.IsTrue(_states.Count > 0);
        State = state;
        StartCoroutine(CoEndOfFrame());
    }

    public void StopFSM() {
        State = default(S);
        IsRunning = false;
    }

    protected void FixedUpdate() {
        if (IsRunning == false) return;
        State = _states[State].FixedUpdate();
        OnFixedUpdate();
    }

    protected void Update() {
        if (IsRunning == false) return;
        State = _states[State].Update();
        OnUpdate();
    }

    protected void LateUpdate() {
        if (IsRunning == false) return;
        State = _states[State].LateUpdate();
        OnLateUpdate();
    }

    protected void EndOfFrame() {
        if (IsRunning == false) return;
        State = _states[State].EndOfFrame();
        OnEndOfFrame();
    }

    protected virtual void OnTransition(S prevState, S nextState) { }
    protected virtual void OnFixedUpdate() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnLateUpdate() { }
    protected virtual void OnEndOfFrame() { }

    protected void AddStates(params FSMState<S, M>[] states) {
        Assert.IsNotNull(states, "[FSM] AddStates : Parameter can be not null.");

        for (int i = 0; i < states.Length; i++) {
            AddState(states[i]);
        }
    }

    private void AddState(FSMState<S, M> state) {
        Assert.IsFalse(_states.ContainsKey(state.State),
                string.Format("[FSM] AddState : {0} has already been added.", state.State));
        state.Initialize(this as M);
        _states.Add(state.State, state);
    }

    protected void RemoveState(S state) {
        Assert.IsTrue(_states.ContainsKey(state),
                string.Format("[FSM] RemoveState : {0} is not on the list of states.", state));
        _states.Remove(state);
    }

    private IEnumerator CoEndOfFrame() {
        if (IsRunning) yield break;

        IsRunning = true;

        while (IsRunning) {
            yield return new WaitForEndOfFrame();
            EndOfFrame();
        }
    }
}