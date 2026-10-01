using UnityEngine;

namespace PA2.Portfolio
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class Runner : MonoBehaviour
    {
        public float speed = 8;
        public float dashSpeed = 22;
        public float dashDuration = .22f;
        public float dashCooldown = 1.1f;
        public Transform model;
        public Animator animator;
        public TrailRenderer trail;
        public bool IsDashing => dashRemaining > 0;
        public float DashReady => Mathf.Clamp01(1 - cooldown / dashCooldown);
        public bool TestInput;
        public Vector2 SimulatedInput;
        public bool SimulatedDash;
        CharacterController controller;
        Vector3 velocity, direction = Vector3.forward;
        float dashRemaining, cooldown;
        void Awake() { controller = GetComponent<CharacterController>(); }
        void Update()
        {
            var session = RunSession.Instance;
            if (session == null || session.State != RunState.Playing)
            {
                if (animator) animator.SetBool("isRun", false);
                if (trail) trail.emitting = false;
                return;
            }
            float dt = Time.deltaTime;
            cooldown = Mathf.Max(0, cooldown - dt);
            Vector2 axes = TestInput ? SimulatedInput : new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector3 move = Vector3.ClampMagnitude(new Vector3(axes.x, 0, axes.y), 1);
            bool dash = TestInput ? SimulatedDash : Input.GetKeyDown(KeyCode.Space);
            SimulatedDash = false;
            if (dash && cooldown <= 0 && move.sqrMagnitude > .01f)
            {
                direction = move.normalized;
                dashRemaining = dashDuration;
                cooldown = dashCooldown;
                session.PlayTone(2);
                if (animator) animator.SetTrigger("doDodge");
            }
            if (IsDashing) { move = direction; dashRemaining = Mathf.Max(0, dashRemaining - dt); }
            if (move.sqrMagnitude > .01f && model)
                model.rotation = Quaternion.Slerp(model.rotation, Quaternion.LookRotation(move), dt * 18);
            if (controller.isGrounded && velocity.y < 0) velocity.y = -2;
            velocity.y -= 30 * dt;
            controller.Move((move * (IsDashing ? dashSpeed : speed) + velocity) * dt);
            if (animator) { animator.SetBool("isRun", move.sqrMagnitude > .01f); animator.SetBool("isWalk", false); }
            if (trail) trail.emitting = IsDashing;
            if (transform.position.y < -5) session.Recover();
        }
        public void Teleport(Vector3 point)
        {
            controller.enabled = false;
            transform.position = point;
            controller.enabled = true;
            velocity = Vector3.zero;
            dashRemaining = 0;
            if (trail) trail.Clear();
        }
    }
}
