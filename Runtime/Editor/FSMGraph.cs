using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMGraph : EditorWindow {
    private FSMGraphView _graphView = null;

    [MenuItem("DaraNaku/FSM Graph Window")]
    public static void OpenFSMGraph() {
        var window = GetWindow<FSMGraph>();
        window.titleContent = new GUIContent("FSM Graph");
    }

    private void OnEnable() {
        _graphView = new FSMGraphView {
            name = "FSM Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void OnDisable() {
        rootVisualElement.Remove(_graphView);
    }
}
