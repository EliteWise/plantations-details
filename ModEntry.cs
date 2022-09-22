using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using static System.Net.Mime.MediaTypeNames;

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
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
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
                    return lang == "en" ? strArray1[0] : strArray1[4];

            }

            return "ERROR";
        }

        // Limit value between 0 & 5 //

        private int limitStage(int stage)
        {
            return Math.Clamp(stage, 0, 5);
        }

        private Dictionary<int, List<string>> trees = new Dictionary<int, List<string>>()
        {
            { 2, new List<string> { "Maple", "Érable" } },
            { 1, new List<string> { "Oak", "Chêne" } },
            { 3, new List<string> { "Pine", "Pin" } },
            { 8, new List<string> { "Mahogany", "Acajou" } },
            { 7, new List<string> { "Mushroom", "Arbre champignon" } },
            { 6, new List<string> { "Palm", "Palmier" } },
            { 9, new List<string> { "Palm", "Palmier" } }
        };

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (this.Config.ToggleKey.JustPressed())
            { 
                Game1.currentLocation.terrainFeatures.TryGetValue(e.Cursor.Tile, out var value);
                if (value == null) return;

                if (value is Tree)
                {
                    Tree tree = (value as Tree);

                    int growthStage = tree.growthStage.Get() + 1;
                    bool fertilized = tree.fertilized.Get();
                    int treeType = tree.treeType.Get();
                    int langIndex = 0;

                    if (this.Config.Lang.Equals("fr")) langIndex = 1;

                    Game1.chatBox.addMessage($"{trees[treeType][langIndex]} | Stage: {limitStage(growthStage)} | Fertilized: {(fertilized ? "Yes" : "No")}", Color.ForestGreen);

                } else if(value is HoeDirt)
                {
                    Crop crop = (value as HoeDirt).crop;

                    if(crop != null)
                    {
                        string cropName = GetNameFromCrop(crop.netSeedIndex.Get());

                        int currentPhase = crop.currentPhase.Get() + 1;
                        int fertilizer = (value as HoeDirt).fertilizer.Get();

                        Game1.chatBox.addMessage($"{cropName} | Stage: {limitStage(currentPhase)} | {(fertilizer != 0 ? GetNameFromCrop(fertilizer) : "No Fertilizer")}", Color.YellowGreen);
                    }
                }

            }
        }
    }
}