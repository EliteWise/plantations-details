using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace PlantationsDetails
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
        }

        public string GetNameFromCrop(int index)
        {
            string lang = this.Config.Lang.ToLower();
            string path = (lang == "en" ? "Data\\ObjectInformation" : "Data\\ObjectInformation." + lang + "-" + lang.ToUpper());
            Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>(path);

            foreach (var c in dictionary)
            {
                string[] strArray1 = c.Value.Split('/');
                if (c.Key == index)
                    return strArray1[0];

            }

            return "ERROR";
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (e.Button == SButton.N)
            { 
                Game1.currentLocation.terrainFeatures.TryGetValue(e.Cursor.Tile, out var value);
                if (value == null) return;

                if (value is Tree)
                {
                    Tree tree = (value as Tree);
                    if (tree == null) return;

                    int growthStage = tree.growthStage.Get() + 1;
                    bool fertilized = tree.fertilized.Get();

                    Game1.chatBox.addMessage("Stage: " + growthStage + " Fertilized: " + (fertilized ? "Yes" : "No"), Color.ForestGreen);

                } else if(value is HoeDirt)
                {
                    Crop crop = (value as HoeDirt).crop;
                    if (crop == null) return;

                    string cropName = GetNameFromCrop(crop.netSeedIndex.Get());
                   
                    int currentPhase = crop.currentPhase.Get() + 1;
                    int fertilized = (value as HoeDirt).fertilizer.Get();

                    Game1.chatBox.addMessage("Name: " + cropName + " Stage: " + currentPhase + " Fertilized: " + (fertilized != 0 ? "Yes" : "No"), Color.ForestGreen);
                }

            }
        }
    }
}