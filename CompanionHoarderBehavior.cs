using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using MountAndBlade.CampaignBehaviors;


namespace ComapanionHoarder
{
    public class CompanionHoarderBehavior : CampaignBehaviorBase
    {

		// Token: 0x06003352 RID: 13138 RVA: 0x00020239 File Offset: 0x0001E439
		public CompanionHoarderBehavior()
		{
			this._companionSettlements = new Dictionary<Settlement, CampaignTime>();
			this._companions = new List<Hero>();
		}

		// Token: 0x06003353 RID: 13139 RVA: 0x000D4D10 File Offset: 0x000D2F10
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedEvent2.AddNonSerializedListener(this, new Action(this.OnAfterNewGameCreated));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTick));
			CampaignEvents.NewCompanionAdded.AddNonSerializedListener(this, new Action<Hero>(this.CompanionAdded));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.OnGameEarlyLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameEarlyLoaded));
			this._missingNotablesSettlements = new List<Settlement>();
		}

		// Token: 0x06003354 RID: 13140 RVA: 0x000D4E10 File Offset: 0x000D3010
		private void OnGameEarlyLoaded(CampaignGameStarter obj)
		{
			foreach (Hero hero in Hero.All)
			{
				if (hero.IsNotable && hero.CurrentSettlement == null && hero.PartyBelongedTo == null && hero.PartyBelongedToAsPrisoner == null && hero.IsActive && hero.IsAlive && hero.HomeSettlement != null && !hero.IsOccupiedByAnEvent())
				{
					EnterSettlementAction.ApplyForCharacterOnly(hero, hero.HomeSettlement);
				}
			}
		}

		// Token: 0x06003355 RID: 13141 RVA: 0x0002026D File Offset: 0x0001E46D
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Settlement, CampaignTime>>("companionSettlements", ref this._companionSettlements);
			dataStore.SyncData<List<Hero>>("companions", ref this._companions);
			dataStore.SyncData<List<Settlement>>("_missingNotablesSettlements", ref this._missingNotablesSettlements);
		}

		// Token: 0x06003356 RID: 13142 RVA: 0x000D4EA8 File Offset: 0x000D30A8
		public void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			this._companionTemplates = new List<CharacterObject>(from x in CharacterObject.Templates
																 where x.Occupation == Occupation.Wanderer
																 select x);
			this._nextRandomCompanionSpawnDate = CampaignTime.WeeksFromNow(this._randomCompanionSpawnFrequencyInWeeks);
			this.SpawnUrbanCharacters();
		}

		// Token: 0x06003357 RID: 13143 RVA: 0x000D4F00 File Offset: 0x000D3100
		private void DetermineRelation(Hero hero1, Hero hero2, float randomValue, float chanceOfConflict)
		{
			float num = 0.3f;
			if (randomValue < num)
			{
				int num2 = (int)((num - randomValue) * (num - randomValue) / (num * num) * 100f);
				if (num2 > 0)
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero1, hero2, num2, true);
					return;
				}
			}
			else if (randomValue > 1f - chanceOfConflict)
			{
				int num3 = -(int)((randomValue - (1f - chanceOfConflict)) * (randomValue - (1f - chanceOfConflict)) / (chanceOfConflict * chanceOfConflict) * 100f);
				if (num3 < 0)
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero1, hero2, num3, true);
				}
			}
		}

		// Token: 0x06003358 RID: 13144 RVA: 0x000D4F74 File Offset: 0x000D3174
		public void SetInitialRelationsBetweenNotablesAndLords()
		{
			foreach (Settlement settlement in Settlement.All)
			{
				for (int i = 0; i < settlement.Notables.Count; i++)
				{
					Hero hero = settlement.Notables[i];
					if (hero.IsNotable)
					{
						foreach (Hero hero2 in settlement.MapFaction.Heroes)
						{
							if (hero2.IsNoble && hero2 != hero && hero2 == hero2.Clan.Leader && hero2.MapFaction == settlement.MapFaction)
							{
								float chanceOfConflict = (float)HeroHelper.NPCPersonalityClashWithNPC(hero, hero2) * 0.01f * 2.5f;
								float num = MBRandom.RandomFloat;
								float num2 = Campaign.MapDiagonal;
								foreach (Settlement settlement2 in hero2.Clan.Settlements)
								{
									float num3 = (settlement == settlement2) ? 0f : settlement2.Position2D.Distance(settlement.Position2D);
									if (num3 < num2)
									{
										num2 = num3;
									}
								}
								float num4 = (num2 < 100f) ? (1f - num2 / 100f) : 0f;
								float num5 = num4 * MBRandom.RandomFloat + (1f - num4);
								if (MBRandom.RandomFloat < 0.2f)
								{
									num5 = 1f / (0.5f + 0.5f * num5);
								}
								num *= num5;
								if (num > 1f)
								{
									num = 1f;
								}
								this.DetermineRelation(hero, hero2, num, chanceOfConflict);
							}
						}
						for (int j = i + 1; j < settlement.Notables.Count; j++)
						{
							Hero hero3 = settlement.Notables[j];
							if (hero3.IsNotable)
							{
								float chanceOfConflict2 = (float)HeroHelper.NPCPersonalityClashWithNPC(hero, hero) * 0.01f * 2.5f;
								float randomValue = MBRandom.RandomFloat;
								if (hero.CharacterObject.Occupation == hero3.CharacterObject.Occupation)
								{
									randomValue = 1f - 0.25f * MBRandom.RandomFloat;
								}
								this.DetermineRelation(hero, hero3, randomValue, chanceOfConflict2);
							}
						}
					}
				}
			}
		}

		// Token: 0x06003359 RID: 13145 RVA: 0x000D5238 File Offset: 0x000D3438
		public void OnAfterNewGameCreated()
		{
			this.InitCompanions();
			foreach (Settlement settlement in Settlement.All)
			{
				if (settlement.IsTown)
				{
					if (settlement.Notables.Count<Hero>() < 4)
					{
						this._missingNotablesSettlements.Add(settlement);
					}
				}
				else if (settlement.IsVillage && settlement.Notables.Count<Hero>() < 2)
				{
					this._missingNotablesSettlements.Add(settlement);
				}
			}
			this.SetInitialRelationsBetweenNotablesAndLords();
			int num = 50;
			for (int i = 0; i < num; i++)
			{
				this.UpdateNotableSupports();
			}
		}

		// Token: 0x0600335A RID: 13146 RVA: 0x000202A5 File Offset: 0x0001E4A5
		private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			this._companionTemplates = new List<CharacterObject>(from x in CharacterObject.Templates
																 where x.Occupation == Occupation.Wanderer
																 select x);
		}

		// Token: 0x0600335B RID: 13147 RVA: 0x000202DB File Offset: 0x0001E4DB
		public void CompanionAdded(Hero hero)
		{
			if (hero.CompanionOf != null && this._companions.Contains(hero))
			{
				this._companions.Remove(hero);
			}
		}

		// Token: 0x0600335C RID: 13148 RVA: 0x000D52EC File Offset: 0x000D34EC
		public void WeeklyTick()
		{
			foreach (KeyValuePair<Settlement, CampaignTime> keyValuePair in new Dictionary<Settlement, CampaignTime>(this._companionSettlements))
			{
				if (keyValuePair.Value.ElapsedWeeksUntilNow > this._companionSpawnCooldownForSettlementInWeeks)
				{
					this._companionSettlements.Remove(keyValuePair.Key);
				}
			}
			if (this._nextRandomCompanionSpawnDate.IsPast)
			{
				int num = 0;
				for (int i = 0; i < num; i++)
				{
					CharacterObject randomElement = (from x in this._companionTemplates
													 where !this._companions.Contains(x.HeroObject)
													 select x).GetRandomElement<CharacterObject>();
					this.CreateCompanion(randomElement ?? this._companionTemplates.GetRandomElement<CharacterObject>());
					this._nextRandomCompanionSpawnDate = CampaignTime.WeeksFromNow(this._randomCompanionSpawnFrequencyInWeeks);
				}
			}
		}

		// Token: 0x0600335D RID: 13149 RVA: 0x000D53CC File Offset: 0x000D35CC
		private void UpdateNotableSupports()
		{
			foreach (Hero hero in Hero.All)
			{
				if (hero.IsNotable)
				{
					if (hero.SupporterOf == null)
					{
						using (List<Clan>.Enumerator enumerator2 = Clan.All.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								Clan clan = enumerator2.Current;
								int relation = hero.GetRelation(clan.Leader);
								if (relation > 50)
								{
									float num = (float)(relation - 50) / 2000f;
									if (MBRandom.RandomFloat < num)
									{
										hero.SupporterOf = clan;
									}
								}
							}
							continue;
						}
					}
					int relation2 = hero.GetRelation(hero.SupporterOf.Leader);
					if (relation2 < 0)
					{
						hero.SupporterOf = null;
					}
					else if (relation2 < 25)
					{
						float num2 = (float)(25 - relation2) / 500f;
						if (MBRandom.RandomFloat < num2)
						{
							hero.SupporterOf = null;
						}
					}
				}
			}
		}

		// Token: 0x0600335E RID: 13150 RVA: 0x000D54E0 File Offset: 0x000D36E0
		private void DailyTick()
		{
			this.UpdateNotableSupports();
			foreach (Settlement settlement in Settlement.All)
			{
				if (settlement.IsFortification)
				{
					foreach (Hero hero in settlement.Notables)
					{
						if (hero.OwnedWorkshops.IsEmpty<Workshop>() && hero.OwnedCaravans.IsEmpty<MobileParty>() && hero.OwnedCommonAreas.IsEmpty<CommonArea>() && (hero.Issue == null || hero.Issue.IsOngoingWithoutQuest) && (float)hero.Power < 100f && MBRandom.RandomFloat < this.GetNotableDisappearProbability(hero))
						{
							this.DisableNotable(hero);
							if (Campaign.Current.IssueManager.Issues.ContainsKey(hero))
							{
								Campaign.Current.IssueManager.Issues[hero].CompleteIssueWithAiLord(settlement.OwnerClan.Leader);
							}
						}
						if (hero.Gold > 30000)
						{
							GiveGoldAction.ApplyBetweenCharacters(hero, null, 5000, true);
							hero.AddPower(1);
						}
						else if (hero.Gold < 5000)
						{
							GiveGoldAction.ApplyBetweenCharacters(null, hero, 2500, true);
							hero.AddPower(-1);
						}
					}
				}
			}
		}

		// Token: 0x0600335F RID: 13151 RVA: 0x000D5680 File Offset: 0x000D3880
		private void HourlyTick()
		{
			foreach (Settlement settlement in Settlement.All)
			{
				if ((double)MBRandom.RandomFloat < 0.01 * (double)settlement.Notables.Count)
				{
					foreach (MobileParty mobileParty in settlement.Parties)
					{
						if (mobileParty.IsLordParty && mobileParty.LeaderHero != null && mobileParty.LeaderHero != Hero.MainHero)
						{
							foreach (Hero hero in settlement.Notables)
							{
								int traitLevel = mobileParty.LeaderHero.GetTraitLevel(DefaultTraits.Honor);
								float num = (hero.IsGangLeader && traitLevel > 0) ? 0.1f : 0.3f;
								if (MBRandom.RandomFloat < num && CharacterRelationManager.GetHeroRelation(hero, mobileParty.LeaderHero) < 100)
								{
									ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero, mobileParty.LeaderHero, 1, true);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06003360 RID: 13152 RVA: 0x00020300 File Offset: 0x0001E500
		private float GetNotableDisappearProbability(Hero hero)
		{
			return (100f - (float)hero.Power) / 5f / 100f;
		}

		// Token: 0x06003361 RID: 13153 RVA: 0x0002031B File Offset: 0x0001E51B
		private void DisableNotable(Hero notable)
		{
			notable.ChangeState(Hero.CharacterStates.Disabled);
			LeaveSettlementAction.ApplyForCharacterOnly(notable);
		}

		// Token: 0x06003362 RID: 13154 RVA: 0x000D5814 File Offset: 0x000D3A14
		public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (mobileParty == MobileParty.MainParty && settlement.IsTown && !this._companionSettlements.ContainsKey(settlement) && this._companions.Count > 0)
			{
				int index = 0;
				MBRandom.ChooseWeighted<Hero>(this._companions, (Hero x) => (float)((x.Culture == settlement.Culture) ? 5 : 1), out index);
				Hero hero2 = this._companions[index];
				hero2.ChangeState(Hero.CharacterStates.Active);
				EnterSettlementAction.ApplyForCharacterOnly(hero2, settlement);
				this._companions.Remove(hero2);
			}
		}

		// Token: 0x06003363 RID: 13155 RVA: 0x000D58B0 File Offset: 0x000D3AB0
		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
		{
			if (victim.IsNotable)
			{
				if (detail == KillCharacterAction.KillCharacterActionDetail.Lost)
				{
					if (victim.CurrentSettlement != null && victim.IsNotable && !this._missingNotablesSettlements.Contains(victim.CurrentSettlement))
					{
						this._missingNotablesSettlements.Add(victim.CurrentSettlement);
					}
				}
				else
				{
					Hero newNotable = HeroCreator.CreateRelativeNotableHero(victim);
					if (victim.CurrentSettlement != null || victim.VisitedSettlements.Any<KeyValuePair<Settlement, float>>())
					{
						this.ChangeDeadNotable(victim, newNotable, victim.CurrentSettlement ?? victim.VisitedSettlements.Last<KeyValuePair<Settlement, float>>().Key);
					}
				}
			}
			if (this._companions.Contains(victim))
			{
				this._companions.Remove(victim);
			}
		}

		// Token: 0x06003364 RID: 13156 RVA: 0x000D595C File Offset: 0x000D3B5C
		private void ChangeDeadNotable(Hero deadNotable, Hero newNotable, Settlement notableSettlement)
		{
			EnterSettlementAction.ApplyForCharacterOnly(newNotable, notableSettlement);
			foreach (Hero otherHero in Hero.All)
			{
				int relation = deadNotable.GetRelation(otherHero);
				if (relation != 0)
				{
					newNotable.SetPersonalRelation(otherHero, relation);
				}
			}
			if (deadNotable.Issue != null)
			{
				Campaign.Current.IssueManager.ChangeIssueOwner(deadNotable.Issue, newNotable);
			}
		}

		// Token: 0x06003365 RID: 13157 RVA: 0x000D59E0 File Offset: 0x000D3BE0
		public void InitCompanions()
		{
			this._companions.Clear();
			this._companionSettlements.Clear();
			foreach (Hero hero in Hero.All)
			{
				if (hero.CanBeCompanion && !hero.IsTemplate)
				{
					this._companions.Add(hero);
				}
			}
		}

		// Token: 0x06003366 RID: 13158 RVA: 0x000D5A60 File Offset: 0x000D3C60
		private void SpawnUrbanCharacters()
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
			int num5 = 200;
			for (int n = 0; n < num5; n++)
			{
				CharacterObject randomElement = (from x in this._companionTemplates
												 where !this._companions.Contains(x.HeroObject)
												 select x).GetRandomElement<CharacterObject>();
				this.CreateCompanion(randomElement ?? this._companionTemplates.GetRandomElement<CharacterObject>());
			}
			this._companions.Shuffle<Hero>();
		}

		// Token: 0x06003367 RID: 13159 RVA: 0x000D5BE0 File Offset: 0x000D3DE0
		private void CreateCompanion(CharacterObject companionTemplate)
		{
			if (companionTemplate == null)
			{
				return;
			}
			Settlement settlement3 = (from settlement in Settlement.All
									  where settlement.Culture == companionTemplate.Culture && settlement.IsTown
									  select settlement).GetRandomElement<Settlement>();
			List<Settlement> list = new List<Settlement>();
			foreach (Settlement settlement2 in Settlement.All)
			{
				if (settlement2.IsVillage && Campaign.Current.Models.MapDistanceModel.GetDistance(settlement2, settlement3) < 30f)
				{
					list.Add(settlement2);
				}
			}
			settlement3 = ((list.Any<Settlement>() ? list.GetRandomElement<Settlement>().Village.Bound : settlement3) ?? Settlement.All.GetRandomElement<Settlement>());
			Hero hero = HeroCreator.CreateSpecialHero(companionTemplate, settlement3, null, null, Campaign.Current.Models.AgeModel.HeroComesOfAge + 5 + MBRandom.RandomInt(27));
			Campaign.Current.GetCampaignBehavior<IHeroCreationCampaignBehavior>().DeriveSkillsFromTraits(hero, companionTemplate);
			CharacterObject template = hero.Template;
			if (((template != null) ? template.HeroObject : null) != null && hero.Template.HeroObject.Clan != null && hero.Template.HeroObject.Clan.IsMinorFaction)
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

		// Token: 0x06003368 RID: 13160 RVA: 0x000D5D80 File Offset: 0x000D3F80
		private void SpawnNotablesIfNeeded()
		{
			List<Settlement> list = new List<Settlement>();
			foreach (Settlement settlement in this._missingNotablesSettlements)
			{
				float randomFloat = MBRandom.RandomFloat;
				float num = settlement.Notables.Any<Hero>() ? (0.8f / (float)settlement.Notables.Count<Hero>()) : 0.8f;
				if (randomFloat <= num)
				{
					List<Occupation> list2 = new List<Occupation>();
					if (settlement.IsTown)
					{
						list2 = new List<Occupation>
						{
							Occupation.GangLeader,
							Occupation.Artisan,
							Occupation.Merchant
						};
					}
					else if (settlement.IsVillage)
					{
						list2 = new List<Occupation>
						{
							Occupation.RuralNotable,
							Occupation.Headman
						};
					}
					foreach (Hero hero in settlement.Notables)
					{
						if (list2.Contains(hero.CharacterObject.Occupation))
						{
							list2.Remove(hero.CharacterObject.Occupation);
						}
					}
					if (list2.Any<Occupation>())
					{
						EnterSettlementAction.ApplyForCharacterOnly(HeroCreator.CreateHeroAtOccupation(list2.GetRandomElement<Occupation>(), settlement), settlement);
					}
				}
				if (settlement.IsTown)
				{
					if (settlement.Notables.Count<Hero>() >= 4)
					{
						list.Add(settlement);
					}
				}
				else if (settlement.IsVillage && settlement.Notables.Count<Hero>() >= 2)
				{
					list.Add(settlement);
				}
			}
			foreach (Settlement item in list)
			{
				this._missingNotablesSettlements.Remove(item);
			}
		}

		// Token: 0x06003369 RID: 13161 RVA: 0x000D5F80 File Offset: 0x000D4180
		public void SpecialCharacterActions()
		{
			foreach (Settlement settlement in Campaign.Current.Settlements)
			{
				if (settlement.IsTown || settlement.IsVillage)
				{
					List<CharacterObject> list = new List<CharacterObject>();
					using (IEnumerator<Hero> enumerator2 = settlement.HeroesWithoutParty.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Hero specialCharacter = enumerator2.Current;
							bool flag = false;
							float randomFloat = MBRandom.RandomFloat;
							foreach (Hero hero in settlement.HeroesWithoutParty)
							{
								if (specialCharacter != hero && !specialCharacter.AwaitingTrial && !hero.AwaitingTrial && specialCharacter.Template == hero.Template && specialCharacter.SpcDaysInLocation <= hero.SpcDaysInLocation)
								{
									flag = true;
								}
							}
							if (settlement.IsTown && specialCharacter.IsPreacher && specialCharacter.GetTraitLevel(DefaultTraits.Generosity) < 1)
							{
								flag = true;
							}
							if (specialCharacter.IsMerchant && specialCharacter.Power < 60 && settlement.IsTown && settlement.Town.Workshops.All((Workshop x) => x.Owner != specialCharacter) && randomFloat < (float)(specialCharacter.Power / 500))
							{
								flag = true;
							}
							if (specialCharacter.IsArtisan || specialCharacter.IsRuralNotable)
							{
								flag = false;
							}
							if (flag)
							{
								list.Add(specialCharacter.CharacterObject);
							}
						}
					}
					foreach (CharacterObject character in list)
					{
						this.MoveSpecialCharacter(character, settlement);
					}
				}
			}
			foreach (Settlement settlement2 in Campaign.Current.Settlements)
			{
				if (settlement2.IsTown)
				{
					foreach (Hero hero2 in settlement2.HeroesWithoutParty)
					{
						hero2.SpcDaysInLocation++;
					}
				}
			}
		}

		// Token: 0x0600336A RID: 13162 RVA: 0x000D62A8 File Offset: 0x000D44A8
		public bool MoveSpecialCharacter(CharacterObject character, Settlement startPoint)
		{
			bool result = false;
			Settlement settlement = null;
			float num = 9999f;
			foreach (Settlement settlement2 in Campaign.Current.Settlements)
			{
				if (settlement2.IsTown && settlement2 != startPoint)
				{
					float num2 = 10000f;
					float num3;
					if (Campaign.Current.Models.MapDistanceModel.GetDistance(settlement2, startPoint, 60f, out num3))
					{
						num2 = (num3 + 10f) * (MBRandom.RandomFloat + 0.1f);
					}
					if (num2 < num)
					{
						settlement = settlement2;
						num = num2;
					}
				}
			}
			if (settlement != null)
			{
				this.CharacterChangeLocation(character.HeroObject, startPoint, settlement);
				result = true;
			}
			return result;
		}

		// Token: 0x0600336B RID: 13163 RVA: 0x0002032A File Offset: 0x0001E52A
		public void CharacterChangeLocation(Hero hero, Settlement startLocation, Settlement endLocation)
		{
			if (startLocation != null)
			{
				LeaveSettlementAction.ApplyForCharacterOnly(hero);
			}
			EnterSettlementAction.ApplyForCharacterOnly(hero, endLocation);
			if (hero != null)
			{
				hero.SpcDaysInLocation = 0;
			}
		}

		// Token: 0x040015D2 RID: 5586
		private Dictionary<Settlement, CampaignTime> _companionSettlements;

		// Token: 0x040015D3 RID: 5587
		private List<Hero> _companions;

		// Token: 0x040015D4 RID: 5588
		private List<CharacterObject> _companionTemplates;

		// Token: 0x040015D5 RID: 5589
		private List<Settlement> _missingNotablesSettlements;

		// Token: 0x040015D6 RID: 5590
		private CampaignTime _nextRandomCompanionSpawnDate;

		// Token: 0x040015D7 RID: 5591
		private float _randomCompanionSpawnFrequencyInWeeks = 6f;

		// Token: 0x040015D8 RID: 5592
		private float _companionSpawnCooldownForSettlementInWeeks = 6f;

		// Token: 0x040015D9 RID: 5593
		private const float NotableSpawnChance = 0.8f;

		// Token: 0x040015DA RID: 5594
		private const float NotableDisappearPowerLimit = 100f;

		// Token: 0x040015DB RID: 5595
		private const float TargetCompanionNumber = 25f;

		// Token: 0x040015DC RID: 5596
		private const int GoldLimitForNotablesToStartGainingPower = 30000;

		// Token: 0x040015DD RID: 5597
		private const int GoldLimitForNotablesToStartLosingPower = 5000;

		// Token: 0x040015DE RID: 5598
		private const int GoldNeededToGainOnePower = 5000;
	}
}
