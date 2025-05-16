using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UNSLOW.UnityUtils
{
    /// <summary>
    /// ビルボード化スクリプト
    /// URP対応型
    /// 複数カメラでの利用を正しく解決できないので現在は非推奨
    /// Shader Graph側で解決可能であればSubgraphのBillboardを使うこと
    /// </summary>
    [Obsolete("BillboarderURP is deprecated, please use alternative methods.")]
    public class BillboarderURP : MonoBehaviour
    {
        private Camera _currentCamera;
        
        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }
        
        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }
        
        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
        {
            _currentCamera = cam;
        }

        private void OnRenderObject()
        {
            if (_currentCamera == null)
                return;

#if UNITY_EDITOR
            if (_currentCamera == UnityEditor.SceneView.lastActiveSceneView.camera)
            {
                Debug.Log(_currentCamera.name);
                return;
            }
#endif
            if ((_currentCamera.cullingMask & (1 << transform.gameObject.layer)) == 0)
                return;

            transform.rotation = _currentCamera.transform.rotation;
        }
    }
}
