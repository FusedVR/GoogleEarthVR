using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying : MonoBehaviour {

    public Transform head;

    public SteamVR_TrackedObject left;
    public SteamVR_TrackedObject right;

    private bool isFlying = false;
	
	// Update is called once per frame
	void Update () {
        if (!left.isActiveAndEnabled || !right.isActiveAndEnabled)
            return;

        var lDevice = SteamVR_Controller.Input((int)left.index);
        var rDevice = SteamVR_Controller.Input((int)right.index);

        if ( lDevice.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) ||rDevice.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) ) {
            isFlying = !isFlying;
        }

        if (isFlying) {
            Vector3 leftDir = left.transform.position - head.position;
            Vector3 rightDir = right.transform.position - head.position;

            Vector3 dir = leftDir + rightDir;

            transform.position += dir;

        }
	}
}
