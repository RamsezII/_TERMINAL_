﻿using _ARK_;
using System;
using System.IO;
using UnityEngine;

namespace _TERMINAL_
{
    public abstract class Command : Schedulable
    {
        enum Bools : byte
        {
            stdout,
            status,
            stdin,
            closable,
            killable,
            _last_,
        }

        [Flags]
        public enum Flags : byte
        {
            _none_ = 0,
            Stdout = 1 << Bools.stdout,
            Status = 1 << Bools.status,
            Stdin = 1 << Bools.stdin,
            Closable = 1 << Bools.closable,
            Killable = 1 << Bools.killable,
            _all_ = (1 << Bools._last_) - 1,
        }

        static readonly string leftPrefixe = Util.app_path;
        public readonly string cmdName, cmdPrefixe;

        public string status, output;

        public Flags flags = Flags.Stdout | Flags.Status | Flags.Stdin | Flags.Closable;

        public Action onSuccess, onFailure;

        //----------------------------------------------------------------------------------------------------------

        public Command()
        {
            cmdName = this is Shell ? "~" : GetType().FullName;
            cmdPrefixe = Terminal.ColoredPrompt(leftPrefixe, cmdName);
            status = $"{cmdName}...";
        }

        //----------------------------------------------------------------------------------------------------------

        public virtual void OnCmdLine(in LineParser line) => OnCmdLine(line.Read(), line);
        public virtual void OnCmdLine(in string arg0, in LineParser line)
        {
            if (line.IsExec)
                Debug.LogWarning($"{cmdName} ({this}) does not implement \"{arg0}\"");
        }

        public virtual void OnGui()
        {
        }

        public void Succeed()
        {
            lock (disposed)
                if (!disposed._value)
                {
                    Debug.Log($"----- {cmdName} Success -----");
                    OnSuccess();
                    onSuccess?.Invoke();
                    Dispose();
                }
        }

        protected virtual void OnSuccess()
        {
        }

        public void Kill()
        {
            lock (disposed)
                if (!disposed._value)
                {
                    Debug.LogWarning($"----- {cmdName} Killed -----");
                    Dispose();
                }
        }

        protected virtual void OnFailure()
        {
            Debug.LogWarning($"----- {cmdName} Failure -----");
            Dispose();
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (Terminal.instance != null)
                Terminal.instance.commands.Remove(this);

            if (!string.IsNullOrWhiteSpace(output))
                Debug.Log($"{cmdName} output{{ {output} }}");

            Debug.Log($"----- {cmdName} Disposed -----".ToSubLog());
            onDispose?.Invoke();
        }
    }
}