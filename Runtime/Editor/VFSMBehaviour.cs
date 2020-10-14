using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(VFSMBehaviour))]
public class FSMBehaviourV2Editor : Editor {
    private bool _isDirty = false;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        DrawFirstStateDropdown();

        if (_isDirty) {
            EditorUtility.SetDirty(target);
            _isDirty = false;
        }
    }

    private void DrawFirstStateDropdown() {
        var filedInfo = typeof(VFSMBehaviour).GetField("_firstState", BindingFlags.NonPublic | BindingFlags.Instance);
        var fsm = target as VFSMBehaviour;
        var states = fsm.GetStateNames();
        var firstState = filedInfo.GetValue(fsm) as string;
        var beforeIndex = states.IndexOf(firstState);
        var afterIndex = EditorGUILayout.Popup("First State", beforeIndex, states.ToArray());

        if (afterIndex != beforeIndex) {
            if ((afterIndex >= 0) && (afterIndex < states.Count)) {
                filedInfo.SetValue(fsm, states[afterIndex]);
                _isDirty = true;
            }
        }
    }
}
