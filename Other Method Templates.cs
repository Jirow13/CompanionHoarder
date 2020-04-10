using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace Companion_Hoarder
//{
   // class Other_Method_Templates
   // {

	//	[HarmonyPatch(typeof(UrbanCharactersCampaignBehavior))]
	//	[HarmonyPatch("SpawnUrbanCharacters")]
	//	public static class SpawnUrbanCharacters_Patch

//		{
	//		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	//		{
	//			var list = instructions.ToList();
	//			for (int i = 0; i < list.Count; i++)
	//			{


	//				var instruction = list[i];
	//				int Startindex = -1;
	//				var codes = instructions.ToList();
	//				if (instruction.opcode == OpCodes.Stloc_S && ((LocalBuilder)list[i].operand).LocalIndex == 15)

	//				{

	//					Startindex = i + 1;
	//					foreach (var cur in codes.GetRange(Startindex, 3))
	//					{
	//						cur.operand = null;
	//						cur.opcode = OpCodes.Nop;
	//					}
	//				}
	//				yield return instruction;
	//			}
	///		}
	//
	//	}
//	}
//}
