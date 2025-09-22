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
                    string treeType = tree.treeType.Get();
                    int langIndex = 0;

                    if (this.Config.Lang.Equals("fr")) langIndex = 1;

                    Game1.chatBox.addMessage($"{trees[Convert.ToInt32(treeType)][langIndex]} | Stage: {limitStage(growthStage)} | Fertilized: {(fertilized ? "Yes" : "No")}", Color.ForestGreen);

                } else if(value is HoeDirt)
                {
                    Crop crop = (value as HoeDirt).crop;

                    if(crop != null)
                    {
                        string cropName = ItemRegistry.Create(crop.indexOfHarvest.Value).DisplayName;

                        int currentPhase = crop.currentPhase.Get() + 1;
                        string fertilizer = (value as HoeDirt).fertilizer.Get();
                        string fertilizerName = fertilizer != null ? ItemRegistry.Create(fertilizer).DisplayName : "No Fertilizer";

                        Game1.chatBox.addMessage($"{cropName} | Stage: {limitStage(currentPhase)} | {fertilizerName}", Color.YellowGreen);
                    }
                }

            }
        }
    }
}