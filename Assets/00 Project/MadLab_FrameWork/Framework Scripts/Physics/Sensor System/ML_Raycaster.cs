using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace MadLab.Sensor {
    public class ML_Raycaster : OptimizedUpdate
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 direction;
        [SerializeField] private float maxDistance;
        public float MaxDistance { 
            private set { } 
            get { return maxDistance; }
        }
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private bool update;

        [SerializeField, ShowIf("$update")] private UnityEvent onRayHit;
        [SerializeField] private bool drawGizmos = true;

        bool invokeEvent = false;

        private void Start() {
            if (update) {
                SetupOptimizedUpdate();
                invokeEvent = onRayHit.GetPersistentEventCount() > 0;
            }
        }

        public override void UpdateMe(float deltaTime)
        {
            RaycastHit hit;
            if (SendRay(out hit)){
                if (invokeEvent)
                    onRayHit.Invoke();
            }
        }
        
        public Vector3 GetHitPoint()
        {
            return transform.position + offset + direction * maxDistance;
        }

        public bool SendRay(out RaycastHit hit) {
            return Physics.Raycast(transform.position + offset, direction, out hit, maxDistance, targetLayers, QueryTriggerInteraction.Collide);
        }

        public bool SendRay(Vector3 customDirection, out RaycastHit hit) {
            direction = customDirection;
            return Physics.Raycast(transform.position + offset, direction, out hit, maxDistance, targetLayers, QueryTriggerInteraction.Collide);
        }
        
        public bool SendRay(Vector3 customDirection, out RaycastHit hit, LayerMask customLayers) {
            direction = customDirection;
            return Physics.Raycast(transform.position + offset, direction, out hit, maxDistance, customLayers, QueryTriggerInteraction.Collide);
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (!drawGizmos)
                return;

            float dis = maxDistance;
            RaycastHit hit;
            if (SendRay(out hit)){
                Gizmos.color = Color.red;
                dis = hit.distance;
            }else{
                Gizmos.color = Color.green;
            }
            
            Gizmos.DrawRay(transform.position + offset, direction * dis);
        }
        #endif
    }
}
