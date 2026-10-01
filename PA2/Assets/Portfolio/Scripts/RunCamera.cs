using UnityEngine;
namespace PA2.Portfolio
{
    public sealed class RunCamera : MonoBehaviour
    {
        public Transform target;
        Vector3 smoothing;
        float shake;
        public void Kick() { shake = .3f; }
        void LateUpdate()
        {
            var run = RunSession.Instance;
            if (!target || run == null) return;
            bool menu = run.State == RunState.Menu;
            Vector3 focus = menu ? new Vector3(-3, 0, 18) : target.position + new Vector3(0, 0, 6);
            Vector3 desired = focus + new Vector3(0, 20, -17);
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref smoothing, .18f, 100, Time.unscaledDeltaTime);
            transform.rotation = Quaternion.Euler(50, 0, 0);
            shake = Mathf.Max(0, shake - Time.unscaledDeltaTime);
            if (shake > 0) transform.position += new Vector3(Mathf.Sin(Time.unscaledTime * 65), Mathf.Cos(Time.unscaledTime * 51), 0) * shake * .4f;
        }
    }
}
