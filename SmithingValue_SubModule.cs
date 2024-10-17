using System;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
//using TaleWorlds.CampaignSystem.ViewModelCollection.Craft;
//using TaleWorlds.CampaignSystem.ViewModelCollection.Craft.WeaponDesign;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;                 //reassign
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;    //reassign
using TaleWorlds.Library;
using TaleWorlds.Localization;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace SmithingValue
{
    public class SmithingValue_SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                new Harmony("SmithingValue").PatchAll(); // with Harmony
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                Exception innerException = ex.InnerException;
                string str = innerException == null ? (string)null : innerException.Message;
                //int num = (int)MessageBox.Show("SmithingValue Error patching:\n" + message + " \n\n" + str);
            }
        }
    }

// Add the value to the property list
[HarmonyPatch(typeof(WeaponDesignVM))]
[HarmonyPatch("RefreshStats")]
class PatchRefreshStats
{
    static AccessTools.FieldRef<WeaponDesignVM, MBBindingList<CraftingListPropertyItem>> primaryPropertyListRef = AccessTools.FieldRefAccess<WeaponDesignVM, MBBindingList<CraftingListPropertyItem>>("_primaryPropertyList");
    static AccessTools.FieldRef<WeaponDesignVM, Crafting> craftingRef = AccessTools.FieldRefAccess<WeaponDesignVM, Crafting>("_crafting");
    static void Postfix(WeaponDesignVM __instance)
    {
        ItemObject weapon = craftingRef(__instance).GetCurrentCraftedItemObject(false); // v1.2.4b: del: , null
        EquipmentElement equipment = new EquipmentElement(weapon);
        int price = Campaign.Current.Models.TradeItemPriceFactorModel.GetPrice(equipment, Campaign.Current.MainParty, null, true, 0, 0, 0);
        CraftingListPropertyItem valueItem = new CraftingListPropertyItem(new TextObject("Value: ", null), 999999f, (float)price, 0f, CraftingTemplate.CraftingStatTypes.NumStatTypes, false);
        valueItem.IsValidForUsage = true;
        primaryPropertyListRef(__instance).Add(valueItem);
    }
}

    // Prevent the value from being in the result screen
    [HarmonyPatch(typeof(WeaponDesignVM))]
    [HarmonyPatch("UpdateResultPropertyList")]
    class PatchUpdateResult
    {
        static AccessTools.FieldRef<WeaponDesignVM, MBBindingList<WeaponDesignResultPropertyItemVM>> designResultRef = AccessTools.FieldRefAccess<WeaponDesignVM, MBBindingList<WeaponDesignResultPropertyItemVM>>("_designResultPropertyList");
        static void Postfix(WeaponDesignVM __instance)
        {
            designResultRef(__instance).RemoveAt(designResultRef(__instance).Count - 1);
        }
    }
}