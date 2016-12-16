﻿using System;
using System.Collections.Generic;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Menus;

using Entoarox.Framework;
using Entoarox.Framework.UI;

namespace Entoarox.ExtendedMinecart
{
    internal class Config
    {
        public bool RefuelingEnabled = true;
        public bool AlternateDesertMinecart = false;
        public bool AlternateFarmMinecart = false;
        public bool FarmDestinationEnabled = true;
        public bool DesertDestinationEnabled = true;
        public bool WoodsDestinationEnabled = true;
        public bool BeachDestinationEnabled = true;
        public bool WizardDestinationEnabled = true;
    }
    internal static class GLExtension
    {
        public static void SetTile(this GameLocation self, int x, int y, int index ,string layer, int sheet=0)
        {
            EntoFramework.GetLocationHelper().SetStaticTile(self, layer, x, y, index, self.map.TileSheets[sheet].Id);
        }
        public static void SetTile(this GameLocation self, int x, int y, int index, string layer, string value, int sheet = 0)
        {
            EntoFramework.GetLocationHelper().SetStaticTile(self, layer, x, y, index, self.map.TileSheets[sheet].Id);
            EntoFramework.GetLocationHelper().SetTileProperty(self, layer, x, y, "Action", value);
        }
    }
    public class ExtendedMinecart : Mod
    {
        private static List<KeyValuePair<string, string>> DestinationData = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("Farm","Farm"),
            new KeyValuePair<string, string>("Town","Town"),
            new KeyValuePair<string, string>("Mine","Mine"),
            new KeyValuePair<string, string>("BusStop","Bus"),
            new KeyValuePair<string, string>("Mountain","Quarry"),
            new KeyValuePair<string, string>("Desert","Desert"),
            new KeyValuePair<string, string>("Woods","Woods"),
            new KeyValuePair<string, string>("Beach","Beach"),
            new KeyValuePair<string, string>("Forest","Wizard")
        };
        private static FrameworkMenu Menu;
        private static Dictionary<string, ButtonFormComponent> Destinations;
        private static Random Rand = new Random();
        private Config Config;
        private bool CheckRefuel = true;
        public override void Entry(IModHelper helper)
        {
            GameEvents.UpdateTick += GameEvents_UpdateTick;
            Config = helper.ReadConfig<Config>();
        }
        private void GameEvents_UpdateTick(object s, EventArgs e)
        {
            if (!Game1.hasLoadedGame || Game1.CurrentEvent!=null)
                return;
            GameEvents.UpdateTick -= GameEvents_UpdateTick;
            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            Destinations = new Dictionary<string, ButtonFormComponent>();
            foreach(KeyValuePair<string, string> item in DestinationData)
            {
                switch(item.Key)
                {
                    case "Farm":
                        if (!Config.FarmDestinationEnabled)
                            continue;
                        break;
                    case "Desert":
                        if (!Config.DesertDestinationEnabled)
                            continue;
                        break;
                    case "Woods":
                        if (!Config.WoodsDestinationEnabled)
                            continue;
                        break;
                    case "Beach":
                        if (!Config.BeachDestinationEnabled)
                            continue;
                        break;
                    case "Forest":
                        if (!Config.WizardDestinationEnabled)
                            continue;
                        break;
                }
                Destinations.Add(item.Key, new ButtonFormComponent(new Microsoft.Xna.Framework.Point(-1, 3 + 11 * Destinations.Count),65, item.Value, (t, p, m) => AnswerResolver(item.Key)));
            }
            Menu = new FrameworkMenu(new Microsoft.Xna.Framework.Point(85, Destinations.Count * 11 + 22));
            Menu.AddComponent(new LabelComponent(new Microsoft.Xna.Framework.Point(-3, -16), "Choose destination"));
            foreach (ButtonFormComponent c in Destinations.Values)
                Menu.AddComponent(c);
            // # Farm
            if (Config.FarmDestinationEnabled)
            {
                try
                {
                    GameLocation farm = Game1.getFarm();
                    if (Config.AlternateFarmMinecart)
                    {
                        farm.SetTile(18, 5, 483, "Front", 1);
                        farm.SetTile(19, 5, 484, "Front", 1);
                        farm.SetTile(19, 5, 217, "Buildings", 1);
                        farm.SetTile(20, 5, 485, "Front", 1);

                        farm.SetTile(18, 6, 508, "Buildings", 1);
                        farm.SetTile(19, 6, 509, "Back", 1);
                        farm.SetTile(20, 6, 510, "Buildings", 1);

                        farm.SetTile(18, 7, 533, "Buildings", 1);
                        farm.SetTile(19, 7, 534, "Back", 1);
                        farm.SetTile(20, 7, 535, "Buildings", 1);

                        farm.SetTile(19, 6, 933, "Buildings", 1);
                        farm.SetTile(19, 7, 958, "Buildings", "MinecartTransport", 1);
                    }
                    else
                    {
                        // Clear annoying flower
                        farm.removeTile(79, 12, "Buildings");
                        // Cut dark short
                        farm.SetTile(77, 11, 375, "Back", 1);
                        farm.SetTile(78, 11, 376, "Back", 1);
                        farm.SetTile(79, 11, 376, "Back", 1);
                        // Lay tracks
                        farm.SetTile(78, 12, 729, "Back", 1);
                        farm.SetTile(78, 13, 754, "Back", 1);
                        farm.SetTile(78, 14, 755, "Back", 1);
                        farm.SetTile(79, 12, 730, "Back", 1);
                        // Trim grass
                        farm.SetTile(77, 13, 175, "Back", 1);
                        farm.SetTile(77, 14, 175, "Back", 1);
                        farm.SetTile(77, 15, 175, "Back", 1);
                        farm.SetTile(78, 15, 175, "Back", 1);
                        farm.SetTile(79, 13, 175, "Back", 1);
                        farm.SetTile(79, 14, 175, "Back", 1);
                        farm.SetTile(79, 15, 175, "Back", 1);
                        // Clean up fence
                        farm.SetTile(78, 11, 436, "Buildings", 1);
                        farm.removeTile(78, 14, "Buildings");
                        // Plop down minecart
                        farm.SetTile(78, 12, 933, "Buildings", 1);
                        farm.SetTile(78, 13, 958, "Buildings", "MinecartTransport", 1);
                        // Keep exit clear
                        farm.setTileProperty(78, 14, "Back", "NoFurniture", "T");
                    }
                }
                catch(Exception err)
                {
                    Monitor.Log(LogLevel.Error, "Could not patch the Farm due to a unknown error", err);
                }
            }
            if (Config.DesertDestinationEnabled)
            {
                try
                {
                    // # Desert
                    GameLocation desert = Game1.getLocationFromName("Desert");
                    xTile.Tiles.TileSheet parent = Game1.getLocationFromName("Mountain").map.GetTileSheet("outdoors");
                    desert.map.AddTileSheet(new xTile.Tiles.TileSheet("z_path_objects_custom_sheet", desert.map, parent.ImageSource, parent.SheetSize, parent.TileSize));
                    desert.map.DisposeTileSheets(Game1.mapDisplayDevice);
                    desert.map.LoadTileSheets(Game1.mapDisplayDevice);
                    if (Config.AlternateDesertMinecart)
                    {
                        // Backdrop
                        desert.SetTile(33, 1, 221, "Front");
                        desert.SetTile(34, 1, 222, "Front");
                        desert.SetTile(35, 1, 223, "Front");

                        desert.SetTile(33, 2, 237, "Front");
                        desert.SetTile(34, 2, 254, "Buildings");
                        desert.SetTile(34, 2, 238, "Front");
                        desert.SetTile(35, 2, 239, "Front");

                        desert.SetTile(33, 3, 253, "Buildings");
                        desert.SetTile(34, 3, 254, "Buildings");
                        desert.SetTile(35, 3, 255, "Buildings");

                        desert.SetTile(33, 4, 269, "Buildings");
                        desert.SetTile(34, 4, 270, "Back");
                        desert.SetTile(35, 4, 271, "Buildings");
                        // Cart
                        desert.SetTile(34, 3, 933, "Front", 2);
                        desert.SetTile(34, 4, 958, "Buildings", "MinecartTransport", 2);
                    }
                    else
                    {
                        // Backdrop
                        desert.SetTile(33, 39, 221, "Front");
                        desert.SetTile(34, 39, 222, "Front");
                        desert.SetTile(35, 39, 223, "Front");

                        desert.SetTile(33, 40, 237, "Front");
                        desert.SetTile(34, 40, 254, "Buildings");
                        desert.SetTile(34, 40, 238, "Front");
                        desert.SetTile(35, 40, 239, "Front");

                        desert.SetTile(33, 41, 253, "Buildings");
                        desert.SetTile(34, 41, 254, "Buildings");
                        desert.SetTile(35, 41, 255, "Buildings");

                        desert.SetTile(33, 42, 269, "Buildings");
                        desert.SetTile(34, 42, 270, "Back");
                        desert.SetTile(35, 42, 271, "Buildings");
                        // Cart
                        desert.SetTile(34, 41, 933, "Front", 2);
                        desert.SetTile(34, 42, 958, "Buildings", "MinecartTransport", 2);
                    }
                }
                catch (Exception err)
                {
                    Monitor.Log(LogLevel.Error, "Could not patch the Desert due to a unknown error", err);
                }
            }
            if (Config.WoodsDestinationEnabled)
            {
                try
                {
                    // # Woods
                    GameLocation woods = Game1.getLocationFromName("Woods");
                    woods.SetTile(46, 3, 933, "Front", 1);
                    woods.SetTile(46, 4, 958, "Buildings", "MinecartTransport", 1);
                }
                catch (Exception err)
                {
                    Monitor.Log(LogLevel.Error, "Could not patch the Woods due to a unknown error", err);
                }
            }
            if(Config.WizardDestinationEnabled)
            {
                try
                {
                    // # Wizard
                    GameLocation forest = Game1.getLocationFromName("Forest");
                    forest.SetTile(13, 37, 483, "Front");
                    forest.SetTile(14, 37, 484, "Front");
                    forest.SetTile(14, 37, 217, "Buildings");
                    forest.SetTile(15, 37, 485, "Front");

                    forest.SetTile(13, 38, 508, "Buildings");
                    forest.SetTile(14, 38, 509, "Back");
                    forest.SetTile(15, 38, 510, "Buildings");

                    forest.SetTile(13, 39, 533, "Buildings");
                    forest.SetTile(15, 39, 535, "Buildings");

                    forest.SetTile(14, 38, 933, "Buildings");
                    forest.SetTile(14, 39, 958, "Buildings", "MinecartTransport");
                }
                catch (Exception err)
                {
                    Monitor.Log(LogLevel.Error, "Could not patch the Forest due to a unknown error", err);
                }
            }
            if(Config.BeachDestinationEnabled)
            {
                try
                {
                    // # Beach
                    GameLocation beach = Game1.getLocationFromName("Beach");
                    xTile.Tiles.TileSheet parent = Game1.getLocationFromName("Mountain").map.GetTileSheet("outdoors");
                    beach.map.AddTileSheet(new xTile.Tiles.TileSheet("z_path_objects_custom_sheet", beach.map, parent.ImageSource, parent.SheetSize, parent.TileSize));
                    beach.map.DisposeTileSheets(Game1.mapDisplayDevice);
                    beach.map.LoadTileSheets(Game1.mapDisplayDevice);
                    beach.removeTile(67, 2, "Buildings");
                    beach.removeTile(67, 5, "Buildings");
                    beach.removeTile(67, 4, "Buildings");
                    beach.SetTile(67, 2, 933, "Buildings", 2);
                    beach.SetTile(67, 3, 958, "Buildings", "MinecartTransport", 2);
                }
                catch (Exception err)
                {
                    Monitor.Log(LogLevel.Error, "Could not patch the Beach due to a unknown error", err);
                }
        }
        }
        private void MenuEvents_MenuChanged(object s, EventArgs e)
        {
            if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is DialogueBox) || Game1.currentLocation.lastQuestionKey != "Minecart")
                return;
            Game1.currentLocation.lastQuestionKey = null;
            Game1.dialogueUp = false;
            Game1.player.CanMove = true;
            if (Config.RefuelingEnabled)
            {
                if (CheckRefuel && !Game1.player.mailReceived.Contains("MinecartNeedsRefuel") && Rand.NextDouble() < 0.05)
                    Game1.player.mailReceived.Add("MinecartNeedsRefuel");
                if (Game1.player.mailReceived.Contains("MinecartNeedsRefuel"))
                {
                    if (Game1.player.hasItemInInventory(382, 5))
                        Game1.currentLocation.createQuestionDialogue("The mincart has run out of fuel, use 5 coal to refuel it?", new Response[2] { new Response("Yes", "Yes"), new Response("No", "No") }, (a, b) =>
                        {
                            if (b == "Yes")
                            {
                                a.removeItemsFromInventory(382, 5);
                                a.mailReceived.Remove("MinecartNeedsRefuel");
                            }
                        });
                    else
                        Game1.drawObjectDialogue("The minecart is out of fuel and requires 5 coal to be refueled.");
                }
            }
            foreach (KeyValuePair<string, ButtonFormComponent> item in Destinations)
            {
                item.Value.Disabled = false;
                if (item.Key == Game1.currentLocation.Name)
                    item.Value.Disabled = true;
            }
            if (!Game1.player.mailReceived.Contains("ccCraftsRoom"))
                Destinations["Mountain"].Disabled = true;
            if (Config.DesertDestinationEnabled && !Game1.player.mailReceived.Contains("ccVault"))
                Destinations["Desert"].Disabled = true;
            if (Config.WoodsDestinationEnabled && !Game1.player.mailReceived.Contains("beenToWoods"))
                Destinations["Woods"].Disabled = true;
            if (Config.BeachDestinationEnabled && !(Game1.getLocationFromName("Beach") as StardewValley.Locations.Beach).bridgeFixed)
                Destinations["Beach"].Disabled = true;
            CheckRefuel = false;
            Game1.activeClickableMenu = Menu;
        }
        private void AnswerResolver(string answer)
        {
            Menu.ExitMenu();
            CheckRefuel = true;
            switch (answer)
            {
                case "Mountain":
                    Game1.warpFarmer("Mountain", 124, 12, 2);
                    break;
                case "BusStop":
                    Game1.warpFarmer("BusStop", 4, 4, 2);
                    break;
                case "Mine":
                    Game1.warpFarmer("Mine", 13, 9, 1);
                    break;
                case "Town":
                    Game1.warpFarmer("Town", 105, 80, 1);
                    break;
                case "Farm":
                    if(Config.AlternateFarmMinecart)
                        Game1.warpFarmer("Farm", 19, 8, 1);
                    else
                        Game1.warpFarmer("Farm", 78, 14, 1);
                    break;
                case "Desert":
                    if (Config.AlternateDesertMinecart)
                        Game1.warpFarmer("Desert", 34, 5, 1);
                    else
                        Game1.warpFarmer("Desert", 34, 43, 1);
                    break;
                case "Woods":
                    Game1.warpFarmer("Woods", 46, 5, 1);
                    break;
                case "Forest":
                    Game1.warpFarmer("Forest", 14, 40, 1);
                    break;
                case "Beach":
                    Game1.warpFarmer("Beach", 67, 4, 1);
                    break;
            }
        }
    }
}
