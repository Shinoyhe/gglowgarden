using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
    const string WARP_ENABLED = "_ENABLE";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Camera.main.cullingMatrix = Matrix4x4.Perspective(Camera.main.fieldOfView*1.1f, Camera.main.aspect, 0.001f, Camera.main.farClipPlane) * Camera.main.worldToCameraMatrix;
    }
    
    private void Awake() {
        if (Application.isPlaying){
            Shader.EnableKeyword(WARP_ENABLED);
        }
        else {
            Shader.DisableKeyword(WARP_ENABLED);
        }
    }
    
    private void OnEnable() {
        RenderPipelineManager.beginCameraRendering += ChangeCameraRendering;
        RenderPipelineManager.endCameraRendering += ResetCameraRendering;
        
        
    }
    
    private void OnDisable() {
        RenderPipelineManager.beginCameraRendering -= ChangeCameraRendering;
        RenderPipelineManager.endCameraRendering -= ResetCameraRendering;
        Shader.DisableKeyword(WARP_ENABLED);
        Camera.main.ResetCullingMatrix();
    }
    
    static void ChangeCameraRendering(ScriptableRenderContext ctx, Camera cam){
        Debug.Log("I work");
        // float w = Screen.width*2;
        // float h = Screen.height*2;
        
    }
    
    static void ResetCameraRendering(ScriptableRenderContext ctx, Camera cam){
        cam.ResetCullingMatrix();
    }
}
