using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Companion_Hoarder;
using Helpers;
using MountAndBlade.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Companion_Hoarder
{
	// Token: 0x02000007 RID: 7
	public class CompanionHoarderCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002225 File Offset: 0x00000425
		public CompanionHoarderCampaignBehavior()
		{
			this._companionSettlements = new Dictionary<Settlement, CampaignTime>();
			this._companions = new List<Hero>();
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000225C File Offset: 0x0000045C
		public override void RegisterEvents()
		{
		
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
	        CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			this._missingNotablesSettlements = new List<Settlement>();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002410 File Offset: 0x00000610
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Settlement, CampaignTime>>("companionSettlements", ref this._companionSettlements);
			dataStore.SyncData<List<Hero>>("companions", ref this._companions);
			dataStore.SyncData<List<Settlement>>("_missingNotablesSettlements", ref this._missingNotablesSettlements);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000244C File Offset: 0x0000064C
		public void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			this._companionTemplates = new List<CharacterObject>(from x in CharacterObject.Templates
																 where x.Occupation == Occupation.Wanderer
																 select x);
			this._nextRandomCompanionSpawnDate = CampaignTime.WeeksFromNow(this._randomCompanionSpawnFrequencyInWeeks);
			this.SetNumber();
		}

		

		// Token: 0x06000010 RID: 16 RVA: 0x00002934 File Offset: 0x00000B34
		private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			this._companionTemplates = new List<CharacterObject>(from x in CharacterObject.Templates
																 where x.Occupation == Occupation.Wanderer
																 select x);
		}

		
		

		// Token: 0x0600001C RID: 28 RVA: 0x000032A0 File Offset: 0x000014A0
		public void SetNumber()
		{

			int num5 = CompanionHoarderSubModule.settings.CompanionNumber;
			for (int n = 0; n < num5; n++)
			{
				CharacterObject randomElement = (from x in this._companionTemplates
												 where !this._companions.Contains(x.HeroObject)
												 select x).GetRandomElement<CharacterObject>();
				this.CreateCompanion(randomElement ?? this._companionTemplates.GetRandomElement<CharacterObject>());
			}
			this._companions.Shuffle<Hero>();
			MessageBox.Show("This code is working");
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00003474 File Offset: 0x00001674
		public void CreateCompanion(CharacterObject companionTemplate)
		{
			bool flag = companionTemplate == null;
			if (!flag)
			{
				Settlement settlement3 = (from settlement in Settlement.All
										  where settlement.Culture == companionTemplate.Culture && settlement.IsTown
										  select settlement).GetRandomElement<Settlement>();
				List<Settlement> list = new List<Settlement>();
				foreach (Settlement settlement4 in Settlement.All)
				{
					bool flag2 = settlement4.IsVillage && Campaign.Current.Models.MapDistanceModel.GetDistance(settlement4, settlement3) < 30f;
					if (flag2)
					{
						list.Add(settlement4);
					}
				}
				settlement3 = ((list.Any<Settlement>() ? list.GetRandomElement<Settlement>().Village.Bound : settlement3) ?? Settlement.All.GetRandomElement<Settlement>());
				Hero hero = HeroCreator.CreateSpecialHero(companionTemplate, settlement3, null, null, Campaign.Current.Models.AgeModel.HeroComesOfAge + 5 + MBRandom.RandomInt(27));
				Campaign.Current.GetCampaignBehavior<IHeroCreationCampaignBehavior>().DeriveSkillsFromTraits(hero, companionTemplate);
				CharacterObject template = hero.Template;
				bool flag3 = ((template != null) ? template.HeroObject : null) != null && hero.Template.HeroObject.Clan != null && hero.Template.HeroObject.Clan.IsMinorFaction;
				if (flag3)
				{
					hero.SupporterOf = hero.Template.HeroObject.Clan;
				}
				else
				{
					hero.SupporterOf = HeroHelper.GetRandomClanForNotable(hero);
				}
				CampaignEventDispatcher.Instance.OnHeroCreated(hero, false);
				this._companions.Add(hero);
			}
		}

	
		// Token: 0x04000004 RID: 4
		private Dictionary<Settlement, CampaignTime> _companionSettlements;

		// Token: 0x04000005 RID: 5
		private List<Hero> _companions;

		// Token: 0x04000006 RID: 6
		private List<CharacterObject> _companionTemplates;

		// Token: 0x04000007 RID: 7
		private List<Settlement> _missingNotablesSettlements;

		// Token: 0x04000008 RID: 8
		private CampaignTime _nextRandomCompanionSpawnDate;

		// Token: 0x04000009 RID: 9
		private float _randomCompanionSpawnFrequencyInWeeks = 6f;

		// Token: 0x0400000A RID: 10
		private float _companionSpawnCooldownForSettlementInWeeks = 6f;

		// Token: 0x0400000B RID: 11
		private const float NotableSpawnChance = 0.8f;

		// Token: 0x0400000C RID: 12
		private const float NotableDisappearPowerLimit = 100f;

		// Token: 0x0400000D RID: 13
		private const float TargetCompanionNumber = 25f;

		// Token: 0x0400000E RID: 14
		private const int GoldLimitForNotablesToStartGainingPower = 30000;

		// Token: 0x0400000F RID: 15
		private const int GoldLimitForNotablesToStartLosingPower = 5000;

		// Token: 0x04000010 RID: 16
		private const int GoldNeededToGainOnePower = 5000;
	}
}
