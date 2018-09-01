﻿using System;
using System.IO;

namespace ArbitesEto2
{
    public class MdConfig
    {
        public static MdConfig Main { get; set; }

        public static int SoftwareVersion = 2;
        public int ConfigVersion { get; set; }

        public bool KeyMenuTopmost { get; set; }

        public bool DisplayOutput { get; set; }

        public int UploadDelay { get; set; }

        public string CurrentInputMethod { get; set; }

        public static void Init()
        {

            Main = MdCore.DeserializeFromPath<MdConfig>(Path.Combine(MdConstant.Root, MdConstant.N_CONFIG));
        }

        public MdConfig()
        {
            this.CurrentInputMethod = "US-ANSI" + MdConstant.E_INPUT_METHOD;
            this.KeyMenuTopmost = true;
            this.DisplayOutput = false;
            this.UploadDelay = 10;
            this.ConfigVersion = 2;
        }

        public DisplayCharacterContainer GetCurrentInputMethod()
        {
            return MdCore.DeserializeFromPath<DisplayCharacterContainer>(Path.Combine(MdConstant.Root, MdConstant.D_INPUT_METHOD, this.CurrentInputMethod));
        }
    }
}
