using UnityEngine;
using UnityEngine.EventSystems;

namespace PA2.Portfolio
{
    public sealed class RunHUD : MonoBehaviour
    {
        static readonly Color Ink = new Color(.035f, .07f, .1f, .96f);
        static readonly Color White = new Color(.88f, .95f, .96f);
        static readonly Color Muted = new Color(.48f, .65f, .7f);
        static readonly Color Cyan = new Color(.3f, .95f, .8f);
        Font font;
        RectTransform root, menu, overlay, hud;
        UnityEngine.UI.Text clock, integrity, stage, toast, dash, resultTitle, resultBody, best, sound;
        UnityEngine.UI.Image healthBar, dashBar, progressBar, damage;
        UnityEngine.UI.Text[] relayLabels;
        UnityEngine.UI.Button resumeButton;
        RunState previous = (RunState)(-1);
        float flash;

        void Start()
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;
            var scaler = gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600, 900);
            scaler.matchWidthOrHeight = 1;
            gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            root = GetComponent<RectTransform>();
            if (!FindObjectOfType<EventSystem>()) new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            menu = Panel(root, "Menu", 40, 40, 565, 820, Ink);
            Label(menu, "PA2  /  PORTFOLIO EDITION", 36, 35, 490, 25, 16, Cyan);
            Label(menu, "RELAY\nRUN", 32, 102, 510, 210, 80, White, FontStyle.Bold);
            Label(menu, "PRECISION. MOMENTUM. ESCAPE.", 38, 322, 490, 28, 16, Cyan);
            Label(menu, "An obstacle-course time trial.\nActivate three relays and reach the exit\nbefore the facility shuts down.", 38, 382, 490, 96, 22, Muted);
            Button(menu, "ENTER THE COURSE   →", 38, 513, 488, 66, () => RunSession.Instance.Begin(), true);
            Label(menu, "WASD / ARROWS     MOVE\nSPACE                       DASH + EVADE\nESC                           PAUSE", 38, 618, 490, 95, 18, White);
            best = Label(menu, "", 38, 754, 490, 28, 16, Cyan);

            hud = Panel(root, "HUD", 0, 0, 1600, 900, Color.clear);
            Stretch(hud);
            var left = Panel(hud, "Integrity", 32, 28, 360, 124, Ink);
            Label(left, "RELAY RUN", 22, 15, 210, 30, 25, White, FontStyle.Bold);
            integrity = Label(left, "", 22, 53, 320, 26, 16, Muted);
            Panel(left, "Track", 22, 91, 316, 6, new Color(.16f,.23f,.27f));
            healthBar = Panel(left, "Health", 22, 91, 316, 6, Cyan).GetComponent<UnityEngine.UI.Image>();
            var top = Panel(hud, "Relays", 550, 28, 500, 76, Ink);
            top.anchorMin = top.anchorMax = new Vector2(.5f, 1); top.anchoredPosition = new Vector2(-250, -28);
            relayLabels = new UnityEngine.UI.Text[3];
            for (int i = 0; i < 3; i++) relayLabels[i] = Label(top, "0" + (i+1) + "  OFFLINE", 22 + i * 161, 26, 160, 25, 16, Muted);
            var right = Panel(hud, "Timer", 1312, 28, 256, 124, Ink);
            right.anchorMin = right.anchorMax = Vector2.one; right.anchoredPosition = new Vector2(-288, -28);
            Label(right, "TIME TO SHUTDOWN", 22, 15, 220, 22, 14, Muted);
            clock = Label(right, "", 20, 44, 220, 60, 43, White, FontStyle.Bold);
            var bottom = Panel(hud, "Dash", 32, 750, 360, 118, Ink);
            bottom.anchorMin = bottom.anchorMax = Vector2.zero; bottom.anchoredPosition = new Vector2(32, 150);
            dash = Label(bottom, "", 22, 18, 320, 30, 20, Cyan, FontStyle.Bold);
            Label(bottom, "SPACE  /  EVADE THROUGH HAZARDS", 22, 58, 325, 23, 13, Muted);
            Panel(bottom, "CooldownTrack", 22, 94, 316, 4, new Color(.16f,.23f,.27f));
            dashBar = Panel(bottom, "Cooldown", 22, 94, 316, 4, Cyan).GetComponent<UnityEngine.UI.Image>();
            stage = Label(hud, "", 520, 812, 800, 35, 19, White);
            stage.rectTransform.anchorMin = stage.rectTransform.anchorMax = new Vector2(.5f,0);
            stage.rectTransform.anchoredPosition = new Vector2(-250,88);
            toast = Label(hud, "", 445, 127, 850, 42, 20, Cyan, FontStyle.Bold);
            toast.alignment = TextAnchor.MiddleCenter;
            toast.rectTransform.anchorMin = toast.rectTransform.anchorMax = new Vector2(.5f,1);
            toast.rectTransform.anchoredPosition = new Vector2(-425,-127);
            var progress = Panel(hud, "CourseProgress", 0, 0, 1600, 4, new Color(.15f,.22f,.25f));
            progress.anchorMin = new Vector2(0,0); progress.anchorMax = new Vector2(1,0); progress.sizeDelta = new Vector2(0,4); progress.anchoredPosition = Vector2.zero;
            progressBar = Panel(progress, "Progress", 0, 0, 1600, 4, Cyan).GetComponent<UnityEngine.UI.Image>();
            damage = Panel(root, "DamageFlash", 0, 0, 1600, 900, Color.clear).GetComponent<UnityEngine.UI.Image>(); Stretch(damage.rectTransform);

            overlay = Panel(root, "Overlay", 0, 0, 1600, 900, new Color(.015f,.03f,.045f,.88f)); Stretch(overlay);
            var card = Panel(overlay, "ResultCard", 430, 175, 740, 550, Ink);
            card.anchorMin = card.anchorMax = new Vector2(.5f,.5f); card.anchoredPosition = new Vector2(-370,275);
            Label(card, "RELAY RUN   /   SESSION REPORT", 42, 34, 650, 28, 17, Cyan);
            resultTitle = Label(card, "", 38, 102, 670, 80, 55, White, FontStyle.Bold);
            resultBody = Label(card, "", 42, 206, 650, 155, 23, Muted);
            resumeButton = Button(card, "RESUME", 42, 398, 310, 62, () => RunSession.Instance.Resume(), true);
            Button(card, "RETRY COURSE", 378, 398, 320, 62, () => RunSession.Instance.Retry(), false);
            Label(card, "ESC  PAUSE / RESUME     •     ENTER  RETRY", 42, 491, 650, 25, 14, Muted);
            var soundButton = Button(root, "", 1405, 818, 163, 50, () => RunSession.Instance.ToggleSound(), false);
            soundButton.GetComponent<RectTransform>().anchorMin = soundButton.GetComponent<RectTransform>().anchorMax = new Vector2(1,0);
            soundButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-195,82);
            sound = soundButton.GetComponentInChildren<UnityEngine.UI.Text>();
        }
        void Update()
        {
            var run = RunSession.Instance; if (!run || !clock) return;
            if (previous != run.State)
            {
                previous = run.State;
                menu.gameObject.SetActive(run.State == RunState.Menu);
                hud.gameObject.SetActive(run.State != RunState.Menu);
                overlay.gameObject.SetActive(run.State == RunState.Paused || run.State == RunState.Won || run.State == RunState.Lost);
                resumeButton.gameObject.SetActive(run.State == RunState.Paused);
                resultTitle.text = run.State == RunState.Paused ? "SYSTEM PAUSED" : run.State == RunState.Won ? "RUN COMPLETE / " + run.Grade : "SIGNAL LOST";
                resultBody.text = run.State == RunState.Paused ? "Take a breath. Your timer is paused.\n\nActivate relays in order. Dash through hazards.\nCyan cells restore integrity." :
                    "TIME   " + run.Elapsed.ToString("0.00") + "s     /     IMPACTS   " + run.Hits + "\n\nRELAYS   " + run.Relays + " / 3     /     CELLS   " + run.Cells +
                    (run.State == RunState.Won ? "\n\nFewer hits. Faster time. A better run." : "\n\n" + (run.Health <= 0 ? "Integrity depleted. Time your dash to evade." : "Time expired. Keep moving toward the next relay."));
            }
            clock.text = Mathf.FloorToInt(run.Remaining / 60).ToString("00") + ":" + Mathf.FloorToInt(run.Remaining % 60).ToString("00");
            clock.color = run.Remaining < 30 ? new Color(1,.4f,.3f) : White;
            integrity.text = "INTEGRITY   " + run.Health.ToString("000") + " / 100";
            healthBar.rectTransform.sizeDelta = new Vector2(316 * run.Health / 100, 6);
            healthBar.color = run.Health <= 30 ? new Color(1,.4f,.3f) : Cyan;
            dash.text = run.player.DashReady >= 1 ? "DASH  /  READY" : "DASH  /  RECHARGING";
            dashBar.rectTransform.sizeDelta = new Vector2(316 * run.player.DashReady, 4);
            for (int i = 0; i < 3; i++) { relayLabels[i].text = "0" + (i+1) + (run.Relays > i ? "  ONLINE" : "  OFFLINE"); relayLabels[i].color = run.Relays > i ? Cyan : Muted; }
            float z = run.player.transform.position.z;
            stage.text = z < 34 ? "01  /  CROSS TRAFFIC     →     ACTIVATE THE RELAY" : z < 68 ? "02  /  ANGULAR MOMENTUM     →     KEEP MOVING" : "03  /  PULSE ARRAY     →     WATCH THE WARNING TILES";
            progressBar.rectTransform.sizeDelta = new Vector2(hud.rect.width * Mathf.Clamp01(z / 108), 4);
            toast.text = Time.unscaledTime < run.NoticeUntil ? run.Notice : "";
            best.text = run.Best > 0 ? "PERSONAL BEST   " + run.Best.ToString("0.00") + "s" : "THREE SECTORS. ONE WAY OUT.";
            sound.text = run.Muted ? "SOUND OFF [M]" : "SOUND ON [M]";
            flash = Mathf.MoveTowards(flash,0,Time.unscaledDeltaTime * 1.5f);
            damage.color = new Color(1,.12f,.08f,flash);
        }
        public void Flash() { flash = .25f; }
        RectTransform Panel(Transform parent, string name, float x,float y,float w,float h,Color color)
        {
            var obj = new GameObject(name,typeof(RectTransform),typeof(UnityEngine.UI.Image));
            var rect = obj.GetComponent<RectTransform>(); rect.SetParent(parent,false);
            rect.anchorMin = rect.anchorMax = new Vector2(0,1); rect.pivot = new Vector2(0,1);
            rect.anchoredPosition = new Vector2(x,-y); rect.sizeDelta = new Vector2(w,h);
            var img = obj.GetComponent<UnityEngine.UI.Image>(); img.color = color; img.raycastTarget = false;
            return rect;
        }
        UnityEngine.UI.Text Label(Transform parent,string value,float x,float y,float w,float h,int size,Color color,FontStyle style=FontStyle.Normal)
        {
            var obj = new GameObject("Text",typeof(RectTransform),typeof(UnityEngine.UI.Text));
            var rect = obj.GetComponent<RectTransform>(); rect.SetParent(parent,false);
            rect.anchorMin = rect.anchorMax = new Vector2(0,1); rect.pivot = new Vector2(0,1);
            rect.anchoredPosition = new Vector2(x,-y); rect.sizeDelta = new Vector2(w,h);
            var label = obj.GetComponent<UnityEngine.UI.Text>(); label.font = font; label.text = value; label.fontSize = size; label.color = color;
            label.fontStyle = style; label.raycastTarget = false; label.horizontalOverflow = HorizontalWrapMode.Wrap;
            return label;
        }
        UnityEngine.UI.Button Button(Transform parent,string title,float x,float y,float w,float h,UnityEngine.Events.UnityAction action,bool primary)
        {
            var rect = Panel(parent,"Button",x,y,w,h,primary ? Cyan : new Color(.13f,.23f,.28f));
            rect.GetComponent<UnityEngine.UI.Image>().raycastTarget = true;
            var button = rect.gameObject.AddComponent<UnityEngine.UI.Button>(); button.onClick.AddListener(action);
            var colors = button.colors; colors.highlightedColor = new Color(.75f,1,1); colors.pressedColor = new Color(.45f,.7f,.7f); button.colors = colors;
            var label = Label(rect,title,0,0,w,h,18,primary ? Ink : White,FontStyle.Bold); label.alignment = TextAnchor.MiddleCenter;
            return button;
        }
        static void Stretch(RectTransform rect) { rect.anchorMin=Vector2.zero; rect.anchorMax=Vector2.one; rect.offsetMin=rect.offsetMax=Vector2.zero; }
    }
}
