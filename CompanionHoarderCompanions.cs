using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;

namespace ComapanionHoarder
{
	[HarmonyPatch(typeof(UrbanCharactersCampaignBehavior), "CompanionHoarderNumbers")]
	class CompanionnHoarderNumbers
	{
		private static void Postfix(Hero hero, ref int __result)
		{
			__result = CompanionHoarderSubModule.settings.CompanionNumber;
		}
	}

	[HarmonyPatch(typeof(UrbanCharactersCampaignBehavior), "CompanionHoarderLimit")]
	class CompanionHoarderCreateCompanion
	{
		private  void Prefix(UrbanCharactersCampaignBehavior __instance)
		{
			foreach (Settlement settlement in Settlement.All)
			{
				if (settlement.IsTown)
				{
					for (int i = 0; i < 1; i++)
					{
						HeroCreator.CreateHeroAtOccupation(Occupation.Artisan, settlement);
					}
					float valueNormalized = settlement.Random.GetValueNormalized(0);
					int num = 1 + MBRandom.RoundRandomized(valueNormalized * 2f);
					for (int j = 0; j < num; j++)
					{
						HeroCreator.CreateHeroAtOccupation(Occupation.Merchant, settlement);
					}
					valueNormalized = settlement.Random.GetValueNormalized(1);
					int num2 = 2 + MBRandom.RoundRandomized(valueNormalized);
					for (int k = 0; k < num2; k++)
					{
						HeroCreator.CreateHeroAtOccupation(Occupation.GangLeader, settlement);
					}
				}
				else if (settlement.IsVillage)
				{
					int num3 = 1 + MBRandom.RandomInt(2);
					for (int l = 0; l < num3; l++)
					{
						HeroCreator.CreateHeroAtOccupation(Occupation.RuralNotable, settlement);
					}
					int num4 = 1;
					for (int m = 0; m < num4; m++)
					{
						HeroCreator.CreateHeroAtOccupation(Occupation.Headman, settlement);
					}
				}
			}
			int num5 = 50;
			for (int n = 0; n < num5; n++)
			{
				CharacterObject randomElement = (from x in this._companionTemplates
												 where !this._companions.Contains(x.HeroObject)
												 select x).GetRandomElement<CharacterObject>();
				this.CreateCompanion(randomElement ?? this._companionTemplates.GetRandomElement<CharacterObject>());
			}
			this._companions.Shuffle<Hero>();


		}

		private void CreateCompanion(CharacterObject characterObject)
		{
			throw new NotImplementedException();
		}

		// Token: 0x040015D2 RID: 5586
		private Dictionary<Settlement, CampaignTime> _companionSettlements;

		// Token: 0x040015D3 RID: 5587
		private List<Hero> _companions;

		// Token: 0x040015D4 RID: 5588
		private List<CharacterObject> _companionTemplates;

	}


}