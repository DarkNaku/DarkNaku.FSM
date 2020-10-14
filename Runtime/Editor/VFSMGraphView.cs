using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;

public class VFSMGraphView : GraphView {
    private readonly Vector2 DEFAULT_NODE_SIZE = new Vector2(300f, 150f);

    private VFSMBehaviour _vfsm = null;
    private List<VFSMState> _states = null;
    private VFSMGraphData _graphData = null;

    public VFSMGraphView() {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);  
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        graphViewChanged += OnGraphViewChange;
        RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);

        Refresh();
    }

    private void OnMouseUp(MouseUpEvent e) {
        Debug.Log("Select");
    }

    private GraphViewChange OnGraphViewChange(GraphViewChange change) {
        if (change.edgesToCreate != null) {
            for (int i = 0; i < change.edgesToCreate.Count; i++) {
                var edge = change.edgesToCreate[i];
                var fromNode = edge.output.node as BaseNode;
                var toNode = edge.input.node as BaseNode;

                edge.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
                _graphData.SetLink(fromNode.GUID, edge.output.portName, toNode.GUID);
            }
        }

        if (change.elementsToRemove!= null) {
            for (int i = 0; i < change.elementsToRemove.Count; i++) {
                var element = change.elementsToRemove[i];

                if (element is BaseNode) {
                    var node = change.elementsToRemove[i] as BaseNode;

                    _graphData.RemoveNode(node.GUID);
                    RemoveState(node.GUID);
                } else if (element is Edge) {
                    var edge = change.elementsToRemove[i] as Edge;

                    edge.UnregisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
                    _graphData.RemoveLink((edge.output.node as BaseNode).GUID, edge.output.portName);
                }
            }
        }

        if (change.movedElements != null) {
            for (int i = 0; i < change.movedElements.Count; i++) {
                if (change.movedElements[i] is BaseNode) {
                    var node = change.movedElements[i] as BaseNode;
                    _graphData.SetNodePosition(node.GUID, node.GetPosition().position);
                }
            }
        }
        
        return change;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
        base.BuildContextualMenu(evt);

        if (evt.target is GraphView) {
            evt.menu.AppendAction("New State", (e) => {
                if (_states != null) {
                    var state = new VFSMState();
                    CreateStateNode(state);
                    _states.Add(state);
                }
            });
        }
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port) => {
            if ((startPort != port) && (startPort.node != port.node)) {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
    }

    public void Refresh() {
        if (UpdateTargets() == false) return;

        RemoveAllElements();

        if (_vfsm != null) {
            CreateNodes();

            if (LoadGraphData()) {
                UpdateLayout();
            }
        }
    }

    private void RemoveAllElements() {
        graphElements.ForEach((element) => {
            RemoveElement(element);
        });
    }

    private bool UpdateTargets() {
        if (Selection.activeGameObject == null) {
            if (_vfsm == null) {
                Debug.Log("0");
                return false;
            } else {
                Debug.Log("1");
                _vfsm = null;
                _states = null;
                return true;
            }
        }

        var vfsm = Selection.activeGameObject.GetComponent<VFSMBehaviour>();

        if (vfsm == _vfsm) {
            return false;
        } else {
            _vfsm = vfsm;
            _states = Utility.GetField<List<VFSMState>>(_vfsm, "_states");
            return true;
        }
    }

    private void CreateNodes() {
        CreateStartNode();

        for (int i = 0; i < _states.Count; i++) {
            CreateStateNode(_states[i]);
        }
    }

    private void CreateStartNode() {
        var node = new BaseNode {
            title = "START",
            GUID = Guid.NewGuid().ToString()
        };

        var port = CreatePort(node, "", Direction.Output, Port.Capacity.Single);
        node.outputContainer.Add(port);
        node.SetPosition(new Rect(Vector2.zero, DEFAULT_NODE_SIZE));
        node.RefreshExpandedState();
        node.RefreshPorts();

        AddElement(node);
    }

    private void CreateStateNode(VFSMState state) {
        if (state == null) {
            Debug.LogError("[VFSMGraphView] CreatePort : Node is null.");
            return;
        }

        var node = new BaseNode {
            title = state.StateName,
            GUID = state.GUID
        };

        var port = CreatePort(node, "", Direction.Input, Port.Capacity.Multi);
        node.inputContainer.Add(port);

        var _transitions = Utility.GetField<List<VFSMState.Transition>>(state, "_transitions");

        for (int i = 0; i < _transitions.Count; i++) {
            port = CreatePort(node, _transitions[i].EventName, Direction.Output, Port.Capacity.Single);
            port.portName = _transitions[i].EventName;
            node.outputContainer.Add(port);

            /*
            var label = port.contentContainer.Q<Label>("type");
            port.contentContainer.Remove(label);

            var textField = new TextField {
                name = string.Empty,
                value = _transitions[i].EventName
            };

            textField.RegisterValueChangedCallback((e) => {
                ChangeEventName((port.node as BaseNode).GUID, port.portName, e.newValue);
                port.portName = e.newValue;
            });
            port.contentContainer.Add(textField);

            var deleteButton = new Button(() => Debug.Log("Wanna Delete !!!")) {
                text = "X"
            };

            port.contentContainer.Add(deleteButton);
            */
        }

        node.SetPosition(new Rect(Vector2.zero, DEFAULT_NODE_SIZE));
        node.RefreshExpandedState();
        node.RefreshPorts();

        AddElement(node);
    }

    private Port CreatePort(BaseNode node, string portName, Direction direction, Port.Capacity capacity) {
        if (node == null) {
            Debug.LogError("[VFSMGraphView] CreatePort : Node is null.");
            return null;
        }

        var port = node.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
        port.portName = portName;

        return port;
    }

    private bool LoadGraphData() {
        if (_vfsm == null) {
            Debug.LogError("[VFSMGraphView] LoadGraphData : VFSMBehaviour is null.");
            return false;
        }

        var fileName = Utility.GetLocalIdentifierInFile(_vfsm).ToString().ToMD5().ToLower();
        var path = Path.Combine("Assets/Editor Default Resources", fileName + ".asset");

        if (File.Exists(path)) {
            _graphData = EditorGUIUtility.Load(path) as VFSMGraphData;
        } else {
            _graphData = ScriptableObject.CreateInstance<VFSMGraphData>();
            AssetDatabase.CreateAsset(_graphData, path);
        }

        return true;
    }

    private void UpdateLayout() {
        nodes.ForEach((n) => {
            if (n is BaseNode) {
                var node = n as BaseNode;
                var position = _graphData.GetNodePosition(node.GUID);

                if (position != null) {
                    node.SetPosition(new Rect((Vector2)position, DEFAULT_NODE_SIZE));
                }
            }
        });

        ports.ForEach((p) => {
            var toGUID = _graphData.GetLink((p.node as BaseNode).GUID, p.portName);

            if (toGUID != null) {
                nodes.ForEach((n) => {
                    if (n is BaseNode) {
                        var node = n as BaseNode;

                        if (string.Equals(node.GUID, toGUID)) {
                            var edge = new Edge {
                                output = p,
                                input = (Port)node.inputContainer[0]
                            };

                            edge.input.Connect(edge);
                            edge.output.Connect(edge);
                            Add(edge);
                        }
                    }
                });
            };
        });
    }

    private void ChangeEventName(string guid, string oldName, string newName) {
        for (int i = 0; i < _states.Count; i++) {
            if (string.Equals(_states[i].GUID, guid)) {
                var _transitions = Utility.GetField<List<VFSMState.Transition>>(_states[i], "_transitions");

                for (int j = 0; j < _transitions.Count; j++) {
                    if (string.Equals(_transitions[j].EventName, oldName)) {
                        Debug.Log("!!!");
                        Utility.SetField<string>(_transitions[j], "_eventName", newName);
                        break;
                    }
                }

                break;
            }
        }
    }

    private void RemoveState(string guid) {
        for (int i = 0; i < _states.Count; i++) {
            if (string.Equals(_states[i].GUID, guid)) {
                _states.Remove(_states[i]);
                break;
            }
        }
    }
}
