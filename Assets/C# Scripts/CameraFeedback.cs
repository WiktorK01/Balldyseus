using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class CameraFeedback : MonoBehaviour
{
    [SerializeField] MMF_Player cameraShakeVertical;
    [SerializeField] MMF_Player cameraShakeHorizontal;

    public void CameraShakeVertical(){
        cameraShakeVertical.Initialization();
        cameraShakeVertical.PlayFeedbacks();
    }

    public void CameraShakeHorizontal(){
        cameraShakeHorizontal.Initialization();
        cameraShakeHorizontal.PlayFeedbacks();
    }
}
