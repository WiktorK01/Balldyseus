using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchillesRotationResetter : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Press 'R' to set current rotation as default
        {
            SetRotationAsDefault();
        }
    }

    void SetRotationAsDefault()
    {
        // Step 1: Record the current global rotation
        Quaternion currentGlobalRotation = transform.rotation;

        // Step 2: Create a temporary parent object
        GameObject tempParent = new GameObject("TempParent");
        tempParent.transform.position = transform.position;
        tempParent.transform.rotation = Quaternion.identity;

        // Step 3: Parent the object to the temporary parent
        transform.SetParent(tempParent.transform);

        // Step 4: Reset the local rotation to zero
        transform.localRotation = Quaternion.identity;

        // Step 5: Reparent the object back to its original parent and set the global rotation back
        transform.SetParent(null);
        transform.rotation = currentGlobalRotation;

        // Step 6: Destroy the temporary parent
        Destroy(tempParent);
    }
}