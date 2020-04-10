using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using System.Reflection.Emit;
using System.Windows.Forms;
using TaleWorlds.Core;
using System;
using NetworkMessages.FromServer;
using TaleWorlds.CampaignSystem;

namespace Companion_Hoarder
{


		[HarmonyPatch(typeof(UrbanCharactersCampaignBehavior))]
		[HarmonyPatch("SpawnUrbanCharacters")]
		public static class SpawnUrbanCharacters_Patch

			{
			public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				var list = instructions.ToList();
			for (int i = 0; i < list.Count; i++)
			{


				var instruction = list[i];
				int Startindex = -1;
				var codes = instructions.ToList();
				if (instruction.opcode == OpCodes.Stloc_S && ((LocalBuilder)list[i].operand).LocalIndex == 15)

				{

					Startindex = i + 1;
					foreach (var cur in codes.GetRange(Startindex, 3))
					{
						cur.operand = null;
						cur.opcode = OpCodes.Nop;
					}
				}
				yield return instruction;
			}
			}
	
		}
		
	


	//[HarmonyPatch(typeof(UrbanCharactersCampaignBehavior))]
	//[HarmonyPatch("SpawnUrbanCharacters")]
	//public static class SpawnUrbanCharacters_Patch

	//{



	//	public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, object IL_0194)
	//	{
	//		var list = instructions.ToList();
	//		for (int i = 0; i < list.Count; i++)
	//		{


	//			var instruction = list[i];
	//			int Startindex = -1;
	//			var codes = instructions.ToList();
	//			if (instruction.opcode == OpCodes.Callvirt && instruction.operand == typeof(IDisposable).GetMethod("Dispose", AccessTools.all))

	//			{

	//				Startindex = i + 2;
	//				foreach (var cur in codes.GetRange(Startindex, 34))
	//				{
	//					cur.operand = null;
	//					cur.opcode = OpCodes.Nop;
	//				}

	//				codes[Startindex + 1].operand = typeof(CompanionHoarderCampaignBehavior).GetMethod(nameof(CompanionHoarderCampaignBehavior.SetNumber));
	//			}

	//			{
					//yield return new CodeInstruction(OpCodes.Call, typeof(CompanionHoarderCampaignBehavior).GetMethod("SetNumber"));//Injected code
					//yield return new CodeInstruction(OpCodes.Brtrue, instruction.operand);//If true, break to exactly where the original instruction went
					//	yield return new CodeInstruction(OpCodes.Ldsfld)    class Companion_Hoarder.CompanionHoarderSettings Companion_Hoarder.CompanionHoarderSubModule::settings
					//	yield return new CodeInstruction(OpCodes.ldfld) int32 Companion_Hoarder.CompanionHoarderSettings::CompanionNumber
					//	yield return new CodeInstruction(OpCodes.Stloc_0);
					//  yield return new instruction(OpCodes.Ldc_L4_0);
					// yield return new CodeInstruction(OpCodes.stloc.s n
					//yield return new CodeInstruction(OpCodes.Br_S, IL_0194);
					//yield return new CodeInstruction(OpCodes.Nop);
					//yield return new CodeInstruction(OpCodes.Ldarg_0);
					//  yield return new CodeInstruction(OpCodes.ldfld     class [mscorlib] System.Collections.Generic.List`1<class [TaleWorlds.CampaignSystem] TaleWorlds.CampaignSystem.CharacterObject> TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.UrbanCharactersCampaignBehavior::_companionTemplates
					//yield return new CodeInstruction(OpCodes.Ldarg_0);
					//  yield return new CodeInstruction(OpCodes.ldftn instance bool TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.UrbanCharactersCampaignBehavior::'<SpawnUrbanCharacters>b__20_0'(class [TaleWorlds.CampaignSystem] TaleWorlds.CampaignSystem.CharacterObject)
					//   yield return new CodeInstruction(OpCodes.newobj instance void class [mscorlib] System.Func`2<class [TaleWorlds.CampaignSystem] TaleWorlds.CampaignSystem.CharacterObject, bool>::.ctor(object, native int)
					//	yield return new CodeInstruction(OpCodes.call class [mscorlib] System.Collections.Generic.IEnumerable`1<!!0> [System.Core] System.Linq.Enumerable::Where<class [TaleWorlds.CampaignSystem] TaleWorlds.CampaignSystem.CharacterObject>(class [mscorlib] System.Collections.Generic.IEnumerable`1<!!0>, class [mscorlib] System.Func`2<!!0, bool>)
					//  yield return new CodeInstruction(OpCodes.call      !!0 [TaleWorlds.Core] TaleWorlds.Core.Extensions::GetRandomElement<class [TaleWorlds.CampaignSystem] TaleWorlds.CampaignSystem.CharacterObject>(class [mscorlib] System.Collections.Generic.IEnumerable`1<!!0>)
					//   yield return new CodeInstruction(OpCodes.stloc.s randomElement
					//yield return new CodeInstruction(OpCodes.Ldarg_0);
					//  yield return new CodeInstruction(OpCodes.ldloc.s randomElement
					//	yield return new CodeInstruction(OpCodes.IL_0178: dup
					//	yield return new CodeInstruction(OpCodes.brtrue.s IL_0187
					//	yield return new CodeInstruction(OpCodes.IL_017B: pop
					//yield return new CodeInstruction(OpCodes.Ldarg_0);
					//	yield return new CodeInstruction(OpCodes.ldfld class [mscorlib] System.Collections.Generic.List`1<class [TaleWorlds.CampaignSystem] TaleWorlds.CampaignSystem.CharacterObject> TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.UrbanCharactersCampaignBehavior::_companionTemplates
					//  yield return new CodeInstruction(OpCodes.call      !!0 [TaleWorlds.Core] TaleWorlds.Core.Extensions::GetRandomElement<class [TaleWorlds.CampaignSystem] TaleWorlds.CampaignSystem.CharacterObject>(class [mscorlib] System.Collections.Generic.IEnumerable`1<!!0>)
					//  yield return new CodeInstruction(OpCodes.call instance void TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.UrbanCharactersCampaignBehavior::CreateCompanion(class [TaleWorlds.CampaignSystem] TaleWorlds.CampaignSystem.CharacterObject)
					//yield return new CodeInstruction(OpCodes.Nop);
					//yield return new CodeInstruction(OpCodes.Nop);
					//  yield return new CodeInstruction(OpCodes.ldloc.s n
					//  yield return new CodeInstruction(OpCodes.ldc.i4.1
					//  yield return new CodeInstruction(OpCodes.add
					//  yield return new CodeInstruction(OpCodes.stloc.s n
					//  yield return new CodeInstruction(OpCodes.ldloc.s n
					//  yield return new CodeInstruction(OpCodes.ldloc.0
					//	yield return new CodeInstruction(OpCodes.clt
					//	yield return new CodeInstruction(OpCodes.stloc.s V_22
					//  yield return new CodeInstruction(OpCodes.ldloc.s V_22
					//  yield return new CodeInstruction(OpCodes.brtrue.s IL_0156


	//			}
	//			yield return instruction;
	//		}
	//	}

//	}



	[HarmonyPatch(typeof(UrbanCharactersCampaignBehavior))]
	[HarmonyPatch("OnSettlementEntered")]
	public static class OnSettlementEntered_Patch
	//Patch to remove action to place companions on a companionsettlement list so more than one companion can spawn in a tavern
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var list = instructions.ToList();
			for (int i = 0; i < list.Count; i++)
			{


				var instruction = list[i];
				int Startindex = -1;
				var codes = instructions.ToList();
				if (instruction.opcode == OpCodes.Ldc_I4_1)

				{

					Startindex = i + 1;
					foreach (var cur in codes.GetRange(Startindex + 5, 6))
					{
						cur.operand = null;
						cur.opcode = OpCodes.Nop;
					}
				}
				yield return instruction;
			}
		}
	}


	[HarmonyPatch(typeof(UrbanCharactersCampaignBehavior))]
	[HarmonyPatch("WeeklyTick")]
	public static class WeeklyTick_Patch
	// This is so all companions are unique and stops a new companion spawning
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var list = instructions.ToList();
			for (int i = 0; i < list.Count; i++)
			{


				var instruction = list[i];
				int Startindex = -1;
				var codes = instructions.ToList();
				if (instruction.opcode == OpCodes.Ldflda)

				{

					Startindex = i + 1;
					foreach (var cur in codes.GetRange(Startindex + 2, 17))
					{
						cur.operand = null;
						cur.opcode = OpCodes.Nop;
					}
				}
				yield return instruction;
			}
		}
	}
}




	

	//static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	//{
	//	string targetString = "CannotPickUp";
	//	int stringIndex = -1;
	//	List<CodeInstruction> codes = Modify_ForceWear(instructions).ToList();
	//	// Find Ldstr "CannotPickUp"
	//	for (int i = 0; i < codes.Count; i++)
	//	{
	//		var code = codes[i];
	//		if (code.opcode == OpCodes.Ldstr && code.operand as string != null && (code.operand as string).Equals(targetString))
	//		{
	//		stringIndex = i;
	//		break;
	//	}
	//}








