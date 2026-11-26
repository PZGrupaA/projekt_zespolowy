using UnityEngine;

namespace Zaliczenie
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float levelMinX;
        public float levelMaxX = 1000f;
        public float fixedY = 80f;

        Camera view;
        float furthestX;

        void Awake()
        {
            view = GetComponent<Camera>();
        }

        public void ResetToStart()
        {
            furthestX = levelMinX;
        }

        void LateUpdate()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    target = player.transform;
            }

            if (target == null || view == null)
                return;

            float halfWidth = view.orthographicSize * view.aspect;
            float minCameraX = levelMinX + halfWidth;
            float maxCameraX = levelMaxX - halfWidth;
            if (maxCameraX < minCameraX)
                maxCameraX = minCameraX;

            if (target.position.x > furthestX)
                furthestX = target.position.x;

            float cameraX = Mathf.Clamp(furthestX, minCameraX, maxCameraX);
            transform.position = new Vector3(cameraX, fixedY, -10f);

            float leftEdge = cameraX - halfWidth + 10f;
            if (target.position.x < leftEdge)
            {
                Vector3 stuck = target.position;
                stuck.x = leftEdge;
                target.position = stuck;
                Rigidbody2D body = target.GetComponent<Rigidbody2D>();
                if (body != null && body.velocity.x < 0f)
                {
                    Vector2 velocity = body.velocity;
                    velocity.x = 0f;
                    body.velocity = velocity;
                }
            }
        }
    }
}
