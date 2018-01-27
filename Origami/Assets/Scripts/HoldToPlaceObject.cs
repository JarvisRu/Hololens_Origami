using UnityEngine;

public class HoldToPlaceObject : MonoBehaviour {

    bool placing = false;

    private void Start()
    {
        SpatialMapping.Instance.DrawVisualMeshes = false;
    }

    void OnHoldStart()
    {
        placing = true;
        SpatialMapping.Instance.DrawVisualMeshes = true;
    }

    void OnHoldCompleted()
    {
        SpatialMapping.Instance.DrawVisualMeshes = false;
        placing = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If the user is in placing mode,
        // update the placement to match the user's gaze.
        if (placing)
        {
            // Do a raycast into the world that will only hit the Spatial Mapping mesh.
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
                30.0f, SpatialMapping.PhysicsRaycastMask))
            {
                // Move this object's parent object to
                // where the raycast hit the Spatial Mapping mesh.
                this.transform.position = hitInfo.point;

                // Rotate this object's parent object to face the user.
                Quaternion toQuat = Camera.main.transform.localRotation;
                toQuat.x = 0;
                toQuat.z = 0;
                this.transform.rotation = toQuat;
            }
        }
    }
}
