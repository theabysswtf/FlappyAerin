using UnityEngine;

namespace Engine
{

    public interface ICameraService : IService
    {
        public Vector2 ScreenToWorld(Vector2 screen);
    }
    
    
    [RequireComponent(typeof(Camera))]
    public class CameraService : MonoBehaviour, ICameraService
    {
        void Awake()
        {
            Cam = GetComponent<Camera>();
            ServiceFactory.AddService((ICameraService)this);
        }

        Camera Cam { get; set; }
        public Vector2 Position => transform.position;

        Vector2 Resolution
        {
            get
            {
                var pixelWidth = Cam.pixelWidth;
                return new Vector2(pixelWidth, pixelWidth / Cam.aspect);
            }
        }
        public Vector2 Bounds
        {
            get
            {
                var bounds = Resolution / 2.0f;
                bounds /= 64.0f;
                return bounds;
            }
        }

        public Vector2 ScreenToWorld (Vector2 screen)
        {
            var scr = screen - Resolution / 2.0f;
            scr /= 64.0f;
            return scr;
        }
    }
}
