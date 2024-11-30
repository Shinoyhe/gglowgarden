using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraSwapper : MonoBehaviour
{
    public GameObject dialogueCamera;
    public GameObject mainCamera;
    public CinemachineTargetGroup targetGroup;

    private Transform toRemoveA;
    private Transform toRemoveB;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera.SetActive(true);
        dialogueCamera.SetActive(false);
    }
    public void startConversation(Transform targetA, Transform targetB)
    {
        targetGroup.AddMember(targetA,1,0);
        targetGroup.AddMember(targetB, 1, 0);

        toRemoveA = targetA;
        toRemoveB = targetB;

        mainCamera.SetActive(false);
        dialogueCamera.SetActive(true);
    }
    public void endConversation()
    {
        targetGroup.RemoveMember(toRemoveA);
        targetGroup.RemoveMember(toRemoveB);

        mainCamera.SetActive(true);
        dialogueCamera.SetActive(false);
    }
}
