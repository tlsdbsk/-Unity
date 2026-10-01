using UnityEngine;
namespace PA2.Portfolio
{
    public enum PickupKind { Relay, Cell, Exit }
    public sealed class CoursePickup : MonoBehaviour
    {
        public PickupKind kind;
        public int index;
        public Transform visual;
        public Vector3 checkpoint;
        bool used;
        Vector3 visualOrigin;
        void Start() { if (visual) visualOrigin = visual.localPosition; }
        void Update()
        {
            if (visual && !used)
            {
                visual.Rotate(Vector3.up, 45 * Time.deltaTime, Space.World);
                visual.localPosition = visualOrigin + Vector3.up * Mathf.Sin(Time.time * 2.5f) * .15f;
            }
        }
        void OnTriggerEnter(Collider other) { Touch(other); }
        void OnTriggerStay(Collider other) { Touch(other); }
        void Touch(Collider other)
        {
            if (used || !other.GetComponent<Runner>()) return;
            var run = RunSession.Instance;
            if (!run || run.State != RunState.Playing) return;
            if (kind == PickupKind.Exit) { run.TryExit(); return; }
            bool accepted = kind == PickupKind.Cell ? run.CollectCell() : run.ActivateRelay(index, checkpoint);
            if (!accepted) return;
            used = true;
            if (visual) visual.gameObject.SetActive(false);
            if (kind == PickupKind.Cell) gameObject.SetActive(false);
        }
    }
}
