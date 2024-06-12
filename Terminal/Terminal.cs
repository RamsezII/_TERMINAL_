using System.Collections.Generic;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal : MonoBehaviour
    {
        public static Terminal terminal;
        public readonly List<Command> commands = new();

        float nextCplCheck;
        bool cplFlag, bottomFlag;

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init1()
        {
            lines.Clear();
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init2()
        {
            if (FindObjectOfType<Terminal>() == null)
                Instantiate(Resources.Load<Terminal>(nameof(_TERMINAL_)));
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            name = typeof(Terminal).Name;
            terminal = this;

            DontDestroyOnLoad(gameObject);

            InitGUI();
            commands.Add(Shell.instance);
            ReadHistory();
        }

        private void Start()
        {
            ToggleWindow(false);
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void OnApplicationFocus(bool focus)
        {
            if (focus)
                ReadHistory();
            else
                SaveHistory();
        }

        private void OnApplicationQuit() => SaveHistory();

#if UNITY_EDITOR
        [ContextMenu(nameof(_ToggleWindow))]
        void _ToggleWindow() => ToggleWindow(!enabled);
#endif

        public void ToggleWindow(in string stdin)
        {
            this.stdin.text = stdin;
            RequestCursorMove(stdin.Length, true);
            ToggleWindow(true);
        }

        public void ToggleWindow(bool value)
        {
            if (value)
                tryFocus1 = true;
            enabled = value;
        }

        //----------------------------------------------------------------------------------------------------------

        private void LateUpdate()
        {
            lock (lines)
                if (lines_flag)
                {
                    lines_flag = false;
                    OnAddLine();
                }

            if (commands[^1].disposed.Value)
            {
                if (commands.Count == 1)
                    Debug.LogError("Main command disposed ???");
                commands.RemoveAt(commands.Count - 1);
            }

            if (cplFlag)
                if (Time.unscaledTime > nextCplCheck)
                {
                    cplFlag = false;
                    RefreshIntell();
                }
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void OnDestroy()
        {
            if (this == terminal)
                terminal = null;

            lock (commands)
            {
                for (int i = commands.Count - 1; i > 0; i--)
                    commands[i].Dispose();
                commands.Clear();
            }
        }
    }
}