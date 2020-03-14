using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    [SerializeField] private GameManager _gm = null;

    private void Update() {
		if (Input.GetKeyDown(KeyCode.S)) _gm.StartFSM(GAME_STATE.INITIALIZE);
		if (Input.GetKeyDown(KeyCode.E)) _gm.StopFSM();
    }
}