using UnityEngine;
namespace PA2.Portfolio
{
    public enum HazardMotion { Shuttle, Rotor, Pulse }
    public sealed class CourseHazard : MonoBehaviour
    {
        public HazardMotion motion;
        public float rate = 1.4f;
        public float phase;
        public float amplitude = 5;
        public Transform warning;
        Vector3 origin;
        Collider hitbox;
        Renderer surface;
        MaterialPropertyBlock properties;
        void Awake()
        {
            origin = transform.position;
            hitbox = GetComponent<Collider>();
            surface = GetComponent<Renderer>();
            properties = new MaterialPropertyBlock();
        }
        void Update()
        {
            var run = RunSession.Instance;
            if (!run || run.State != RunState.Playing) return;
            float t = run.Elapsed * rate + phase;
            if (motion == HazardMotion.Shuttle) transform.position = origin + Vector3.right * Mathf.Sin(t) * amplitude;
            else if (motion == HazardMotion.Rotor) transform.rotation = Quaternion.Euler(0, t * 40, 0);
            else
            {
                float cycle = Mathf.Repeat(t, 4);
                bool active = cycle >= 1.5f && cycle < 3.1f;
                hitbox.enabled = active;
                transform.position = origin + Vector3.down * (active ? 0 : 2.8f);
                if (warning) warning.localScale = new Vector3(2.4f, .035f, 2.4f) * (cycle < 1.5f ? .8f + .2f * Mathf.Sin(cycle * 14) : 1);
                properties.SetColor("_EmissionColor", active ? new Color(1, .12f, .07f) : Color.black);
                surface.SetPropertyBlock(properties);
            }
        }
        void OnTriggerEnter(Collider other) { Hit(other); }
        void OnTriggerStay(Collider other) { Hit(other); }
        void Hit(Collider other)
        {
            if (other.GetComponent<Runner>() && RunSession.Instance) RunSession.Instance.Damage(20);
        }
    }
}
