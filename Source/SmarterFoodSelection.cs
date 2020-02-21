﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;
using System.Reflection;

namespace Room_Food
{
	//[HarmonyPatch(typeof(TryFindBestFoodSourceFor), "Internal")]
	[StaticConstructorOnStartup]
	public static class SmarterFoodSelectionSupport
	{
		static SmarterFoodSelectionSupport()
		{
			try
			{
				Patch();
			}
			catch (Exception) { }
		}

		public static void Patch()
		{
			Harmony harmony = new Harmony("Uuugggg.rimworld.Room_Food.main");

			Type sfsType = AccessTools.TypeByName("WM.SmarterFoodSelection.Detours.FoodUtility.TryFindBestFoodSourceFor");
			if (sfsType != null && AccessTools.Method(sfsType, "Internal") is MethodInfo sfsMethod)
				harmony.Patch(sfsMethod, new HarmonyMethod(typeof(SmarterFoodSelectionSupport), "Prefix"), null);
		}
		//internal static bool Internal(Pawn getter, Pawn eater, bool desperate, out Thing foodSource, out ThingDef foodDef, bool canRefillDispenser = true, bool canUseInventory = true, bool allowForbidden = false, bool allowCorpse = true, Policy forcedPolicy = null)
		public static bool Prefix(ref bool __result, Pawn getter, Pawn eater, out Thing foodSource, out ThingDef foodDef, bool canRefillDispenser, bool allowForbidden)
		{
			Log.Message($"SmarterFoodSelectionSupport prefix {getter}, {eater}");
			if (FoodFinder.FindRoomFood(getter, eater, FoodPreferability.MealLavish, !eater.IsTeetotaler(), true, canRefillDispenser, allowForbidden) is Thing food)
			{
				foodDef = FoodUtility.GetFinalIngestibleDef(food);
				foodSource = food;
				__result = true;
				return false;
			}
			foodSource = null;
			foodDef = null;
			return true;
		}
	}
}
