using System;
using System.Collections.Generic;

namespace libdiablo3.Api
{
    public static class ItemTypes
    {
        public static readonly Dictionary<int, ItemType> Types = new Dictionary<int, ItemType>
        {
            { 485534122, new ItemType("Weapon", 485534122, -1, 0) },
            { 133016072, new ItemType("Melee", 133016072, 485534122, 0) },
            { 140775496, new ItemType("Swing", 140775496, 133016072, 0) },
            { 1846932879, new ItemType("GenericSwingWeapon", 1846932879, 140775496, 0) },
            { 109694, new ItemType("Axe", 109694, 1846932879, 28416) },
            { 119458520, new ItemType("Axe2H", 119458520, 1846932879, 67328) },
            { 140782159, new ItemType("Sword", 140782159, 1846932879, 28416) },
            { -1307049751, new ItemType("Sword2H", -1307049751, 1846932879, 67328) },
            { 4026134, new ItemType("Mace", 4026134, 1846932879, 28416) },
            { 89494384, new ItemType("Mace2H", 89494384, 1846932879, 67328) },
            { -2094596416, new ItemType("FistWeapon", -2094596416, 140775496, 8192) },
            { -1620551894, new ItemType("CombatStaff", -1620551894, 140775496, 8192) },
            { -1488678091, new ItemType("MightyWeapon1H", -1488678091, 140775496, 256) },
            { -1488678058, new ItemType("MightyWeapon2H", -1488678058, 140775496, 256) },
            { 140658708, new ItemType("Staff", 140658708, 1846932879, 75264) },
            { 372302218, new ItemType("Thrust", 372302218, 133016072, 0) },
            { 998499313, new ItemType("GenericThrustWeapon", 998499313, 372302218, 0) },
            { -199811863, new ItemType("CeremonialDagger", -199811863, 372302218, 1024) },
            { -262576534, new ItemType("Dagger", -262576534, 998499313, 28416) },
            { -1203595600, new ItemType("Polearm", -1203595600, 998499313, 9472) },
            { 140519163, new ItemType("Spear", 140519163, 998499313, 28416) },
            { 285570513, new ItemType("Ranged", 285570513, 485534122, 0) },
            { -293202082, new ItemType("BowClass", -293202082, 285570513, 0) },
            { 395678127, new ItemType("GenericBowWeapon", 395678127, -293202082, 0) },
            { 110504, new ItemType("Bow", 110504, 395678127, 36352) },
            { -1338851342, new ItemType("Crossbow", -1338851342, 395678127, 36352) },
            { 763102523, new ItemType("HandXbow", 763102523, -293202082, 2048) },
            { 165564792, new ItemType("GenericRangedWeapon", 165564792, 285570513, 0) },
            { 4385866, new ItemType("Wand", 4385866, 285570513, 512) },
            { 119253633, new ItemType("Armor", 119253633, -1, 0) },
            { 3851110, new ItemType("Helm", 3851110, 119253633, 12032) },
            { -947867741, new ItemType("GenericHelm", -947867741, 3851110, 12032) },
            { 576647032, new ItemType("SpiritStone_Monk", 576647032, 3851110, 8192) },
            { -1499089042, new ItemType("WizardHat", -1499089042, 3851110, 512) },
            { -333341566, new ItemType("VoodooMask", -333341566, 3851110, 1024) },
            { -131821392, new ItemType("Gloves", -131821392, 119253633, 12032) },
            { 120334087, new ItemType("Boots", 120334087, 119253633, 12032) },
            { -940830407, new ItemType("Shoulders", -940830407, 119253633, 12032) },
            { -1028103400, new ItemType("ChestArmor", -1028103400, 119253633, 12032) },
            { 828360981, new ItemType("GenericChestArmor", 828360981, -1028103400, 12032) },
            { 121411562, new ItemType("Cloak", 121411562, -1028103400, 2048) },
            { 3635495, new ItemType("Belt", 3635495, 119253633, 12032) },
            { -948083356, new ItemType("GenericBelt", -948083356, 3635495, 12032) },
            { -479768568, new ItemType("Belt_Barbarian", -479768568, 3635495, 258) },
            { 3994699, new ItemType("Legs", 3994699, 119253633, 12032) },
            { -1999984446, new ItemType("Bracers", -1999984446, 119253633, 12032) },
            { 1440677334, new ItemType("Offhand", 1440677334, -1, 0) },
            { 344906995, new ItemType("GenericOffHand", 344906995, 1440677334, 0) },
            { -720431272, new ItemType("OffhandOther", -720431272, 344906995, 12032) },
            { 332825721, new ItemType("Shield", 332825721, 344906995, 28416) },
            { 124739, new ItemType("Orb", 124739, 1440677334, 514) },
            { 4041621, new ItemType("Mojo", 4041621, 1440677334, 1026) },
            { 269990204, new ItemType("Quiver", 269990204, 1440677334, 2050) },
            { -740765630, new ItemType("Jewelry", -740765630, -1, 2) },
            { 4214896, new ItemType("Ring", 4214896, -740765630, 126722) },
            { -365243096, new ItemType("Amulet", -365243096, -740765630, 126722) },
            { 1637769035, new ItemType("FollowerSpecial", 1637769035, -740765630, 0) },
            { 129668150, new ItemType("TemplarSpecial", 129668150, 1637769035, 28418) },
            { -953512528, new ItemType("ScoundrelSpecial", -953512528, 1637769035, 44802) },
            { -464307745, new ItemType("EnchantressSpecial", -464307745, 1637769035, 77570) },
            { -792466723, new ItemType("Socketable", -792466723, -1, 12224) },
            { 115609, new ItemType("Gem", 115609, -792466723, 12104) },
            { 322821082, new ItemType("SpellRune", 322821082, -1, 12032) },
            { 1098049466, new ItemType("Runestone_Unattuned", 1098049466, 322821082, 12096) },
            { 715453891, new ItemType("Runestone_A", 715453891, 322821082, 12034) },
            { 715453892, new ItemType("Runestone_B", 715453892, 322821082, 12034) },
            { 715453893, new ItemType("Runestone_C", 715453893, 322821082, 12034) },
            { 715453894, new ItemType("Runestone_D", 715453894, 322821082, 12034) },
            { 715453895, new ItemType("Runestone_E", 715453895, 322821082, 12034) },
            { -2019730316, new ItemType("CraftingReagent", -2019730316, -1, 12096) },
            { 1151156500, new ItemType("Utility", 1151156500, -1, 12096) },
            { -592249216, new ItemType("ChaosShard", -592249216, 1151156500, 12097) },
            { -1380314094, new ItemType("GeneralUtility", -1380314094, 1151156500, 12097) },
            { 327230447, new ItemType("Scroll", 327230447, 1151156500, 12097) },
            { -1266506293, new ItemType("ScrollIdentify", -1266506293, 327230447, 12097) },
            { -342766503, new ItemType("CraftingPlan", -342766503, -1, 12097) },
            { 185803478, new ItemType("CraftingPlanGeneric ", 185803478, -342766503, 12097) },
            { -1323275628, new ItemType("CraftingPlanLegendary ", -1323275628, -342766503, 12097) },
            { -1515023331, new ItemType("CraftingPlan_Smith", -1515023331, -342766503, 12097) },
            { 371712870, new ItemType("CraftingPlan_Jeweler", 371712870, -342766503, 12097) },
            { -1669219336, new ItemType("CraftingPlanLegendary_Smith", -1669219336, -342766503, 12097) },
            { 953860425, new ItemType("ScrollCalling", 953860425, 327230447, 12097) },
            { 1169689462, new ItemType("ScrollGreed", 1169689462, 327230447, 12097) },
            { 468192851, new ItemType("ScrollCompanion", 468192851, 327230447, 16193) },
            { -803496825, new ItemType("ScrollGirththing", -803496825, 327230447, 16193) },
            { 1379106330, new ItemType("ScrollReforgeA", 1379106330, 327230447, 12097) },
            { 1379106331, new ItemType("ScrollReforgeB", 1379106331, 327230447, 12097) },
            { 1379106332, new ItemType("ScrollReforgeC", 1379106332, 327230447, 12097) },
            { 224120761, new ItemType("Potion", 224120761, 1151156500, 12096) },
            { -1916071921, new ItemType("HealthPotion", -1916071921, 224120761, 130881) },
            { -910124122, new ItemType("PowerPotion", -910124122, 224120761, 16193) },
            { 112994, new ItemType("Dye", 112994, 1151156500, 12097) },
            { 126166628, new ItemType("Glyph", 126166628, -1, 12097) },
            { 1883419642, new ItemType("HealthGlyph", 1883419642, 126166628, 12097) },
            { 138327602, new ItemType("Quest", 138327602, -1, 12096) },
            { -938309842, new ItemType("QuestUsable", -938309842, 138327602, 12097) },
            { 3826054, new ItemType("Gold", 3826054, -1, 12032) },
            { 3940472, new ItemType("Junk", 3940472, -1, 12096) },
            { 3646475, new ItemType("Book", 3646475, -1, 12097) },
            { -1079338204, new ItemType("Ornament", -1079338204, -1, 12097) },
            { 167097204, new ItemType("KnowledgeUtility", 167097204, 1151156500, 12097) },
            { -1083019022, new ItemType("PageOfTraining_Smith", -1083019022, 167097204, 12097) },
            { -1621997189, new ItemType("PageOfTraining_Jeweler", -1621997189, 167097204, 12097) },
            { -5794479, new ItemType("TrainingTome", -5794479, 167097204, 12096) },
            { -1352920439, new ItemType("NephalemCube", -1352920439, 1151156500, 12097) },
            { 1626390053, new ItemType("TalismanUnlock", 1626390053, 1151156500, 12097) },
            { -1812121245, new ItemType("StoneOfWealth", -1812121245, 1151156500, 12097) },
            { -2007738575, new ItemType("StoneOfRecall", -2007738575, 1151156500, 12097) },
            { -1945202604, new ItemType("Calldown", -1945202604, -1, 16193) },
        };
    }
}

