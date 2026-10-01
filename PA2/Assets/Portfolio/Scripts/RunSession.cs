using UnityEngine;
using UnityEngine.SceneManagement;

namespace PA2.Portfolio
{
    public enum RunState { Menu, Playing, Paused, Won, Lost }

    public sealed class RunSession : MonoBehaviour
    {
        public static RunSession Instance { get; private set; }
        public Runner player;
        public RunHUD hud;
        public RunCamera view;
        public float timeLimit = 150;
        public RunState State { get; private set; }
        public float Remaining { get; private set; }
        public float Health { get; private set; } = 100;
        public int Relays { get; private set; }
        public int Hits { get; private set; }
        public int Cells { get; private set; }
        public float Elapsed => timeLimit - Remaining;
        public Vector3 Checkpoint { get; private set; } = new Vector3(0, .1f, 3);
        public float Best => PlayerPrefs.GetFloat("RelayRun.Best", 0);
        public string Notice { get; private set; } = "";
        public float NoticeUntil { get; private set; }
        public bool SilentTest { get; set; }
        float protectedUntil;
        AudioSource audioSource;
        AudioClip good, bad, dash;

        void Awake()
        {
            Instance = this;
            Time.timeScale = 1;
            Remaining = timeLimit;
            State = RunState.Menu;
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = PlayerPrefs.GetInt("RelayRun.Muted", 0) == 1 ? 0 : .35f;
            good = Tone(720, .16f); bad = Tone(130, .2f); dash = Tone(340, .09f);
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M)) ToggleSound();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (State == RunState.Playing) Pause();
                else if (State == RunState.Paused) Resume();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (State == RunState.Menu) Begin();
                else if (State == RunState.Won || State == RunState.Lost) Retry();
            }
            if (State != RunState.Playing) return;
            Remaining = Mathf.Max(0, Remaining - Time.deltaTime);
            if (Remaining <= 0) Finish(false);
        }
        public void Begin()
        {
            if (State != RunState.Menu) return;
            State = RunState.Playing;
            Announce("ACTIVATE 3 RELAYS  /  REACH THE EXIT", 4);
            PlayTone(0);
        }
        public void Pause() { if (State != RunState.Playing) return; State = RunState.Paused; Time.timeScale = 0; }
        public void Resume() { if (State != RunState.Paused) return; State = RunState.Playing; Time.timeScale = 1; }
        public void Retry() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
        public bool Damage(float amount)
        {
            if (State != RunState.Playing || Time.time < protectedUntil || player.IsDashing) return false;
            protectedUntil = Time.time + 1.2f;
            Health = Mathf.Max(0, Health - amount);
            Hits++;
            view.Kick();
            hud.Flash();
            PlayTone(1);
            Announce("IMPACT  /  -" + amount.ToString("0") + " INTEGRITY", 1.5f);
            if (Health <= 0) Finish(false);
            return true;
        }
        public void Recover()
        {
            if (State != RunState.Playing) return;
            // Falling always returns the player, even during a dash or damage grace period.
            Damage(15);
            player.Teleport(Checkpoint);
            Announce("RETURNED TO CHECKPOINT", 2);
        }
        public bool ActivateRelay(int index, Vector3 point)
        {
            if (State != RunState.Playing || index != Relays) return false;
            Relays++;
            Checkpoint = point;
            Health = Mathf.Min(100, Health + 10);
            PlayTone(0);
            Announce(Relays == 3 ? "ALL RELAYS ONLINE  /  EXIT UNLOCKED" : "RELAY ONLINE  /  CHECKPOINT SAVED", 3);
            return true;
        }
        public bool CollectCell()
        {
            if (State != RunState.Playing) return false;
            Cells++;
            Health = Mathf.Min(100, Health + 20);
            PlayTone(0);
            Announce("ENERGY CELL  /  +20 INTEGRITY", 2);
            return true;
        }
        public void TryExit()
        {
            if (Relays == 3) Finish(true);
            else Announce("ACTIVATE ALL 3 RELAYS FIRST", 2);
        }
        public string Grade => Hits == 0 && Elapsed < 65 ? "S" : Hits <= 2 && Elapsed < 95 ? "A" : Hits <= 5 ? "B" : "C";
        public void Finish(bool won)
        {
            if (State != RunState.Playing) return;
            State = won ? RunState.Won : RunState.Lost;
            if (won && !SilentTest && (Best <= 0 || Elapsed < Best))
            {
                PlayerPrefs.SetFloat("RelayRun.Best", Elapsed);
                PlayerPrefs.Save();
            }
            PlayTone(won ? 0 : 1);
        }
        public void Announce(string message, float seconds) { Notice = message; NoticeUntil = Time.unscaledTime + seconds; }
        public bool Muted => audioSource.volume == 0;
        public void ToggleSound()
        {
            audioSource.volume = Muted ? .35f : 0;
            if (!SilentTest) { PlayerPrefs.SetInt("RelayRun.Muted", Muted ? 1 : 0); PlayerPrefs.Save(); }
        }
        public void PlayTone(int kind) { if (!SilentTest) audioSource.PlayOneShot(kind == 0 ? good : kind == 1 ? bad : dash); }
        static AudioClip Tone(float frequency, float duration)
        {
            int count = (int)(22050 * duration);
            float[] data = new float[count];
            for (int i = 0; i < count; i++)
            {
                float t = i / 22050f;
                data[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * Mathf.Sin(Mathf.PI * i / count) * .25f;
            }
            var clip = AudioClip.Create("Synth cue", count, 1, 22050, false);
            clip.SetData(data, 0); return clip;
        }
        void OnApplicationFocus(bool focus) { if (!focus && State == RunState.Playing && !SilentTest) Pause(); }
        void OnDestroy()
        {
            Time.timeScale = 1;
            if (Instance == this) Instance = null;
            Destroy(good); Destroy(bad); Destroy(dash);
        }
    }
}
