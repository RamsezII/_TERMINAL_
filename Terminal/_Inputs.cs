﻿using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        static void CatchTabAndEnter(in bool focus, out bool downTab, out bool downSubmit)
        {
            downTab = false;
            downSubmit = false;

            Event e = Event.current;

            switch (e.type)
            {
                case EventType.KeyDown:
                    {
                        downTab = e.character == '\t';
                        downSubmit = e.character == '\n' || e.character == '\r';

                        if (focus)
                        {
                            bool ktab = e.keyCode == KeyCode.Tab;
                            bool ksubmit = e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter;

                            if (ktab || ksubmit || downTab || downSubmit)
                                e.Use();
                        }
                    }
                    break;
            }
        }
    }
}