﻿using System;
using UnityEngine;

namespace _TERMINAL_
{
    public partial class Terminal
    {
        [Serializable]
        public struct TerminalTheme
        {
            public Color
                backgroundColor,
                color_header_top,
                color_header_bottom;
        }

        [SerializeField] TerminalTheme theme;
        [SerializeField] GUIStyle style_header, style_body;

        const string
            PrefixeLeftColor = "#73CC26",
            PrefixeRightColor = "#73B2D9";

        //----------------------------------------------------------------------------------------------------------

        public static string ColoredPrompt(in string user_name, in string command_name) => $"{user_name.SetColor(PrefixeLeftColor)}:{command_name.SetColor(PrefixeRightColor)}$ ";
    }
}