using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class VFSMGraph : EditorWindow {
    private ScrollView _inspector = null;
    private VFSMGraphView _graphView = null;

    [MenuItem("DaraNaku/FSM Graph Window")]
    public static void OpenFSMGraph() {
        var window = GetWindow<VFSMGraph>();
        window.titleContent = new GUIContent("Visual Finite State Machine Graph");
    }

    private void Awake() {
        rootVisualElement.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

        DrawInspector();
        DrawGraphView();
    }

    private void OnGUI() {
        _graphView.style.width = new StyleLength(rootVisualElement.contentRect.width - _inspector.contentRect.width);
    }

    private void OnEnable(){
        Selection.selectionChanged += OnSelectionChanged;
    }

    private void OnDisable() {
        Selection.selectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged() {
        _graphView.Refresh();
    }

    private void DrawInspector() {
        var leftPanel = new VisualElement();
        leftPanel.style.backgroundColor = new StyleColor(Color.red);
        leftPanel.style.width = new StyleLength(250f);

        _inspector = new ScrollView(ScrollViewMode.Vertical);
        _inspector.StretchToParentSize();
        leftPanel.Add(_inspector);

        rootVisualElement.Add(leftPanel);

        UpdateInspector();
    }

    private void DrawGraphView() {
        _graphView = new VFSMGraphView();
        _graphView.style.backgroundColor = new StyleColor(Color.blue);
        rootVisualElement.Add(_graphView);
    }

    private void UpdateInspector() {
        /*
        var list = new ListView();
        list.style.backgroundColor = new StyleColor(Color.green);

        var textField = new TextField {
            name = string.Empty,
            value = "Hello"
        };

        textField.RegisterValueChangedCallback((e) => {
        });

        list.Add(textField);
        _inspector.Add(list);
        */

        for (int i = 0; i < 100; i++) {
            var textField = new TextField {
                name = string.Empty,
                value = "Hello"
            };

            _inspector.Add(textField);
        }
    }
}
