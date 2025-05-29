using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UNSLOW.UnityUtils.Editor
{
    /// <summary>
    /// SceneViewのBlender風操作
    /// </summary>
    [InitializeOnLoad]
    public class SwitchViewPoint
    {
        static SwitchViewPoint()
        {
            SceneView.duringSceneGui += OnDuringSceneGUI;
        }

        ~SwitchViewPoint()
        {
            SceneView.duringSceneGui -= OnDuringSceneGUI;
        }
        
        private static void OnDuringSceneGUI(SceneView sceneView)
        {
            if(sceneView == null)
                return;
            
            if(Event.current.type != EventType.KeyDown)
                return;
            
            switch (Event.current.keyCode)
            {
                case KeyCode.Keypad1:
                    LookAt(Vector3.forward);
                    break;

                case KeyCode.Keypad3:
                    LookAt(Vector3.right);
                    break;

                case KeyCode.Keypad7:
                    LookAt(Vector3.up);
                    break;
                
                case KeyCode.Keypad5:
                    sceneView.orthographic = !sceneView.orthographic;
                    break;

                // SceneViewをGameViewに合わせる
                case KeyCode.Keypad0:
                    {
                        var camera = GetTargetCamera();
                        if (camera != null)
                        {
                            var cameraTransform = camera.transform;
                            sceneView.pivot = cameraTransform.position;
                            sceneView.rotation = cameraTransform.rotation;
                        }
                    }
                    break;
                
                case KeyCode.KeypadPlus:
                    MoveCameraForward(1.0f);
                    sceneView.Repaint();
                    break;
                
                case KeyCode.KeypadMinus:
                    MoveCameraForward(-1.0f);
                    sceneView.Repaint();
                    break;
                
                case KeyCode.KeypadMultiply:
                    sceneView.size = 1.0f;
                    sceneView.Repaint();
                    break;
                
                case KeyCode.KeypadDivide:
                    sceneView.Repaint();
                    break;
            }

            return;

            void LookAt(Vector3 forward)
            {
                if ((Event.current.modifiers & EventModifiers.Control) != 0)
                    forward = -forward;

                sceneView.LookAt(sceneView.pivot, Quaternion.LookRotation(forward));
            }
            
            void MoveCameraForward(float moveAmount)
            {
                var forward = sceneView.rotation * Vector3.forward;
                sceneView.pivot += forward * moveAmount;
                sceneView.Repaint();
            }
        }
        
        private static Camera GetTargetCamera()
        {
            // メインカメラがあればそれを用いる
            if(Camera.main != null)
                return Camera.main;

            // 無ければ先頭のゲームカメラを返す
            var cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
            return cameras.FirstOrDefault(camera => camera.cameraType == CameraType.Game);
        }
    }
}
