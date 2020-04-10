using System;
using TaleWorlds.MountAndBlade;

using HarmonyLib;
using System.Windows.Forms;

namespace Companion_Hoarder
{
     class CompanionHoarderSubModule : MBSubModuleBase
    {
        public static CompanionHoarderSettings settings = new CompanionHoarderSettings();

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
               Harmony harmony = new Harmony("mod.bannerlord.Companionhoarder");
                harmony.PatchAll();
            }
            catch (Exception ex)
            {
                string str = "Error patching:\n";
                string message = ex.Message;
                string str2 = " \n\n";
                Exception innerException = ex.InnerException;
                MessageBox.Show(str + message + str2 + (innerException?.Message));
            }
        }
    }
}
