using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantationsDetails
{
    class ModConfig
    {
        public string Lang { get; set; } = "en";

        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("LeftShift + MouseRight");
    }
}
