using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;





namespace ComapanionHoarder
{
	[HarmonyPatch(typeof(UrbanCharactersCampaignBehavior), "PatchBehavior" )]
	static class CrbanCharactersCampaignBehaviorPatch
	{
		public static bool Prefix(UrbanCharactersCampaignBehavior __instance)
		{


			_ = typeof(UrbanCharactersCampaignBehavior);
					return true;
				

		}
	} 
}