using Artisan.Autocraft;
using Artisan.CraftingLists;
using Artisan.GameInterop;
using Artisan.RawInformation;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using ECommons.Logging;
using OtterGui;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Artisan.IPC
{
    internal static class IPC
    {
        private static bool stopCraftingRequest;

        public static bool StopCraftingRequest
        {
            get => stopCraftingRequest;
            set
            {
                if (value)
                {
                    StopCrafting();
                }
                else
                {
                    if (!Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.WaitingForDutyFinder] && !Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty])
                        ResumeCrafting();
                }
                stopCraftingRequest = value;
            }
        }

        public static ArtisanMode CurrentMode;
        internal static void Init()
        {
            Svc.PluginInterface.GetIpcProvider<bool>("Artisan.GetEnduranceStatus").RegisterFunc(GetEnduranceStatus);
            Svc.PluginInterface.GetIpcProvider<bool, object>("Artisan.SetEnduranceStatus").RegisterAction(SetEnduranceStatus);

            Svc.PluginInterface.GetIpcProvider<bool>("Artisan.IsListRunning").RegisterFunc(IsListRunning);
            Svc.PluginInterface.GetIpcProvider<bool>("Artisan.IsListPaused").RegisterFunc(IsListPaused);
            Svc.PluginInterface.GetIpcProvider<bool, object>("Artisan.SetListPause").RegisterAction(SetListPause);

            Svc.PluginInterface.GetIpcProvider<bool>("Artisan.GetStopRequest").RegisterFunc(GetStopRequest);
            Svc.PluginInterface.GetIpcProvider<bool, object>("Artisan.SetStopRequest").RegisterAction(SetStopRequest);

            Svc.PluginInterface.GetIpcProvider<ushort, int, object>("Artisan.CraftItem").RegisterAction(CraftX);
            Svc.PluginInterface.GetIpcProvider<bool>("Artisan.IsBusy").RegisterFunc(IsBusy);

            Svc.PluginInterface.GetIpcProvider<Dictionary<int, string>>("Artisan.GetLists").RegisterFunc(GetLists);
            Svc.PluginInterface.GetIpcProvider<int, object>("Artisan.StartListById").RegisterAction(StartListById);

            Svc.PluginInterface.GetIpcProvider<ushort, int, object>("Artisan.CraftItemWithSubcrafts").RegisterAction(CraftItemWithSubcrafts);
        }

        internal static void Dispose()
        {
            Svc.PluginInterface.GetIpcProvider<bool>("Artisan.GetEnduranceStatus").UnregisterFunc();
            Svc.PluginInterface.GetIpcProvider<bool, object>("Artisan.SetEnduranceStatus").UnregisterAction();

            Svc.PluginInterface.GetIpcProvider<bool>("Artisan.IsListRunning").UnregisterFunc();
            Svc.PluginInterface.GetIpcProvider<bool>("Artisan.IsListPaused").UnregisterFunc();
            Svc.PluginInterface.GetIpcProvider<bool, object>("Artisan.SetListPause").UnregisterAction();

            Svc.PluginInterface.GetIpcProvider<bool>("Artisan.GetStopRequest").UnregisterFunc();
            Svc.PluginInterface.GetIpcProvider<bool, object>("Artisan.SetStopRequest").UnregisterAction();

            Svc.PluginInterface.GetIpcProvider<ushort, int, object>("Artisan.CraftItem").UnregisterAction();
            Svc.PluginInterface.GetIpcProvider<ushort, int, object>("Artisan.IsBusy").UnregisterFunc();

            Svc.PluginInterface.GetIpcProvider<Dictionary<int, string>>("Artisan.GetLists").UnregisterFunc();
            Svc.PluginInterface.GetIpcProvider<int, object>("Artisan.StartListById").UnregisterAction();

            Svc.PluginInterface.GetIpcProvider<ushort, int, object>("Artisan.CraftItemWithSubcrafts").UnregisterAction();
        }

        static bool GetEnduranceStatus()
        {
            return Endurance.Enable;
        }

        static void SetEnduranceStatus(bool s)
        {
            Endurance.ToggleEndurance(s);
        }

        static bool IsListRunning()
        {
            return CraftingListUI.Processing;
        }

        static bool IsListPaused()
        {
            return CraftingListUI.Processing && CraftingListFunctions.Paused;
        }

        static void SetListPause(bool s)
        {
            if (IsListPaused())
                CraftingListFunctions.Paused = s;
        }

        static bool GetStopRequest()
        {
            return StopCraftingRequest;
        }

        static void SetStopRequest(bool s)
        {
            if (s)
                DuoLog.Information("Artisan has been requested to stop by an external plugin.");
            else
                DuoLog.Information("Artisan has been requested to restart by an external plugin.");

            StopCraftingRequest = s;
        }

        public unsafe static void CraftX(ushort recipeId, int amount)
        {
            if (LuminaSheets.RecipeSheet!.FindFirst(x => x.Value.RowId == recipeId, out var recipe))
            {
                PreCrafting.Tasks.Add((() => PreCrafting.TaskSelectRecipe(recipe.Value), TimeSpan.FromMilliseconds(500)));
                P.TM.Enqueue(() => PreCrafting.Tasks.Count == 0);
                P.TM.DelayNext(100);
                P.TM.Enqueue(() =>
                {
                    Endurance.IPCOverride = true;
                    Endurance.RecipeID = recipeId;
                    P.Config.CraftX = amount;
                    P.Config.CraftingX = true;
                    Endurance.ToggleEndurance(true);
                });
            }
            else
            {
                throw new Exception("RecipeID not found.");
            }
        }

        public static bool IsBusy()
        {
            return Endurance.Enable || CraftingListUI.Processing || P.TM.NumQueuedTasks > 0 || P.CTM.NumQueuedTasks > 0 || !(Crafting.CurState is Crafting.State.IdleBetween or Crafting.State.IdleNormal);
        }

        /// <summary>
        /// Returns a dictionary mapping each crafting list ID to its name.
        /// </summary>
        public static Dictionary<int, string> GetLists()
        {
            return P.Config.NewCraftingLists.ToDictionary(x => x.ID, x => x.Name ?? string.Empty);
        }

        /// <summary>
        /// Starts a crafting list by its ID.
        /// </summary>
        /// <param name="listId">The ID of the crafting list to start.</param>
        public static void StartListById(int listId)
        {
            var list = P.Config.NewCraftingLists.FirstOrDefault(x => x.ID == listId);
            if (list == null)
                throw new Exception($"Crafting list with ID {listId} not found.");

            // porting-note(api12): upstream uses ECommons' TryGetFirst extension; LINQ FirstOrDefault is equivalent and avoids importing ECommons.GenericHelpers here
            var window = P.ws.Windows.FirstOrDefault(x => x.WindowName.Contains(listId.ToString(), StringComparison.CurrentCultureIgnoreCase));
            if (window != null)
                window.IsOpen = false;

            CraftingListUI.selectedList = list;
            CraftingListUI.StartList();
        }

        /// <summary>
        /// Builds a temporary crafting list containing the given recipe plus all of its sub-crafts,
        /// starts it, and automatically removes the temp list from the config once it finishes running.
        /// Throws if the recipe id cannot be resolved.
        /// </summary>
        /// <param name="recipeId">The recipe row id to craft.</param>
        /// <param name="quantity">How many of the main recipe to craft.</param>
        public static void CraftItemWithSubcrafts(ushort recipeId, int quantity)
        {
            if (!LuminaSheets.RecipeSheet!.FindFirst(x => x.Value.RowId == recipeId, out var recipe))
                throw new Exception("RecipeID not found.");

            // Self-heal: drop any temp lists leaked by a prior run that crashed before its auto-cleanup
            // fired. Negative ids are exclusively ours (user lists are always positive), so this only ever
            // removes our own orphans. Questionable crafts sequentially, so no live temp list is in flight here.
            P.Config.NewCraftingLists.RemoveAll(x => x.ID < 0);

            // Sentinel ID: user lists are always positive (SetID picks 1..50000), so a negative id can never
            // collide with a user list and is trivially findable for cleanup. Unique per call to survive
            // back-to-back invocations.
            var tempId = -Math.Abs(Environment.TickCount);
            while (P.Config.NewCraftingLists.Any(x => x.ID == tempId))
                tempId--;

            var tempList = new NewCraftingList
            {
                ID = tempId,
                Name = "[Questionable] temp",
            };

            // Mirror the UI flow (e.g. ListEditor "Add to List (with all sub-crafts)"): add intermediate
            // ingredient recipes first, then add the main recipe itself with the requested quantity.
            CraftingListUI.AddAllSubcrafts(recipe.Value, tempList, quantity, 1);

            if (tempList.Recipes.Any(x => x.ID == recipe.Value.RowId))
                tempList.Recipes.First(x => x.ID == recipe.Value.RowId).Quantity += quantity;
            else
                tempList.Recipes.Add(new ListItem { ID = recipe.Value.RowId, Quantity = quantity });

            P.Config.NewCraftingLists.Add(tempList);
            P.Config.Save();

            CraftingListUI.selectedList = tempList;
            CraftingListUI.StartList();

            // Auto-cleanup: once the list has actually started (Processing went true) and then finished
            // (Processing went back to false), remove the temp list and unregister so this fires exactly once.
            bool started = false;
            void Cleanup(IFramework _)
            {
                // Dispose race: Artisan.P is set to null! in Dispose(); bail and detach if we're tearing down.
                if (P is null)
                {
                    Svc.Framework.Update -= Cleanup;
                    return;
                }

                if (CraftingListUI.Processing && CraftingListUI.selectedList?.ID == tempId)
                {
                    started = true;
                    return;
                }

                if (!started)
                    return;

                // Processing transitioned true -> false: the run is over (finished, stopped, or logout).
                Svc.Framework.Update -= Cleanup;
                P.Config.NewCraftingLists.RemoveAll(x => x.ID == tempId);
                P.Config.Save();
            }

            Svc.Framework.Update += Cleanup;
        }

        public enum ArtisanMode
        {
            None = 0,
            Endurance = 1,
            Lists = 2,
        }
    }
}
