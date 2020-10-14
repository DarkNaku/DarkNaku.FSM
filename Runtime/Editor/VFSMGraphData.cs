using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFSMGraphData : ScriptableObject {
    [SerializeField] private List<VFSMNodeData> _nodes = new List<VFSMNodeData>();
    public List<VFSMNodeData> Nodes { get { return _nodes; } }

    [SerializeField] private List<VFSMLinkData> _links = new List<VFSMLinkData>();
    public List<VFSMLinkData> Links { get { return _links; } }

    public Vector2? GetNodePosition(string guid) {
        for (int i = 0; i < Nodes.Count; i++) {
            if (string.Equals(Nodes[i].GUID, guid)) {
                return Nodes[i].Position;
            }
        }

        return null;
    }

    public void SetNodePosition(string guid, Vector2 position) {
        for (int i = 0; i < Nodes.Count; i++) {
            if (string.Equals(Nodes[i].GUID, guid)) {
                Nodes[i].Position = position;
                return;
            }
        }

        Nodes.Add(new VFSMNodeData {
            GUID = guid,
            Position = position
        });
    }

    public void RemoveNode(string guid) {
        for (int i = 0; i < Nodes.Count; i++) {
            if (string.Equals(Nodes[i].GUID, guid)) {
                Nodes.Remove(Nodes[i]);
                break;
            }
        }
    }

    public string GetLink(string fromGUID, string portName) {
        for (int i = 0; i < Links.Count; i++) {
            if (string.Equals(Links[i].FromGUID, fromGUID) && string.Equals(Links[i].PortName, portName)) {
                return Links[i].ToGUID;
            }
        }

        return null;
    }

    public void SetLink(string fromGUID, string portName, string toGUID) {
        for (int i = 0; i < Links.Count; i++) {
            if (string.Equals(Links[i].FromGUID, fromGUID) && string.Equals(Links[i].PortName, portName)) {
                Links[i].ToGUID = toGUID;
                return;
            }
        }

        Links.Add(new VFSMLinkData {
            FromGUID = fromGUID,
            ToGUID = toGUID,
            PortName = portName
        });
    }

    public void RemoveLink(string fromGUID, string portName) {
        for (int i = 0; i < Links.Count; i++) {
            if (string.Equals(Links[i].FromGUID, fromGUID) && string.Equals(Links[i].PortName, portName)) {
                Links.Remove(Links[i]);
                break;
            }
        }
    }
}
