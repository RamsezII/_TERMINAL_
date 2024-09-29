﻿using System;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        void UpdateStdin(in bool ctab, in bool csubmit)
        {
            Event e = Event.current;
            bool upArrow = e.type == EventType.KeyDown && e.keyCode == KeyCode.UpArrow;
            bool downArrow = e.type == EventType.KeyDown && e.keyCode == KeyCode.DownArrow;

            if (hold_alt)
                Debug.Log($"{nameof(hold_alt)}: {hold_alt}");

            if (!hold_alt && (upArrow || downArrow))
            {
                if (commands.Count == 1)
                    if (GetHistory(upArrow ? -1 : 1, out string line))
                    {
                        stdin.text = line;
                        RequestCursorMove(line.Length, true);
                    }
                e.Use();
            }
            else if (csubmit || ctab)
            {
                Command command = commands[^1];
                if (csubmit && string.IsNullOrWhiteSpace(stdin.text))
                {
                    stdin.text = string.Empty;
                    if (command.flags.HasFlag(Command.Flags.Closable))
                        ToggleWindow(false);
                }
                else
                {
                    CmdM cmdM = 0;
                    if (csubmit)
                        cmdM |= CmdM.Exec;
                    if (ctab)
                        cmdM |= CmdM.Tab;

                    if (e.type == EventType.KeyDown && e.alt)
                        if (e.keyCode == KeyCode.UpArrow)
                            cmdM |= CmdM.AltN;
                    if (e.keyCode == KeyCode.DownArrow)
                        cmdM |= CmdM.AltS;
                    if (e.keyCode == KeyCode.LeftArrow)
                        cmdM |= CmdM.AltW;
                    if (e.keyCode == KeyCode.RightArrow)
                        cmdM |= CmdM.AltE;

                    LineParser line = new(csubmit ? stdin.text : stdinOld, cmdM, stdinOld.Length);

                    try
                    {
                        string temp = stdin.text;
                        if (csubmit)
                        {
                            string log = command.cmdPrefixe + stdin.text;
                            if (this == instance)
                                print(log);
                            else
                                AddLine(log);
                            stdin.text = string.Empty;
                        }

                        command.OnCmdLine(line);

                        if (csubmit && commands.Count == 1)
                            AddToHistory(temp);

                        if (ctab || line.IsCpl)
                        {
                            stdin.text = line.rawtext;
                            RequestCursorMove(line.sel_move, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }
    }
}