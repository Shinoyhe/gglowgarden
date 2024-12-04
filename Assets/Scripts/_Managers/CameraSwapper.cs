using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwapper : MonoBehaviour
{
    public GameObject dialogueCamera;
    public GameObject mainCamera;
    public CinemachineTargetGroup targetGroup;

    private Transform toRemoveA;
    private Transform toRemoveB;
    
    private float defaultBlend;
    [SerializeField] private float dialogueBlend = 0.5f;
    
    private CinemachineBrain brain;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera.SetActive(true);
        dialogueCamera.SetActive(false);
        
        brain = GetComponentInChildren<CinemachineBrain>();
        
        defaultBlend = brain.m_DefaultBlend.m_Time;
    }
    public void startConversation(Transform targetA, Transform targetB)
    {
        targetGroup.AddMember(targetA,1,0);
        targetGroup.AddMember(targetB, 1, 0);

        toRemoveA = targetA;
        toRemoveB = targetB;

        mainCamera.SetActive(false);
        dialogueCamera.SetActive(true);
        
        brain.m_DefaultBlend.m_Time = dialogueBlend;
    }
    public void endConversation()
    {
        targetGroup.RemoveMember(toRemoveA);
        targetGroup.RemoveMember(toRemoveB);

        mainCamera.SetActive(true);
        dialogueCamera.SetActive(false);
        
        StartCoroutine(WaitBeforeBlendChange());
    }
    
    IEnumerator WaitBeforeBlendChange(){
       yield return new WaitForSeconds(dialogueBlend+0.1f);
        brain.m_DefaultBlend.m_Time = defaultBlend;
    }
}
