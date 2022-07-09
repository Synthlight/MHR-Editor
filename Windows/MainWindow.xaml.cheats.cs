using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using MHR_Editor.Common;
using MHR_Editor.Common.Models;
using MHR_Editor.Generated.Models;
using MHR_Editor.Models.Enums;
using MHR_Editor.Models.Structs;

namespace MHR_Editor.Windows;

public partial class MainWindow {
    private void Btn_all_cheats_Click(object sender, RoutedEventArgs e) {
        Btn_max_sharpness_Click(sender, e);
        Btn_max_slots_Click(sender, e);
        Btn_max_skills_Click(sender, e);
        Btn_no_cost_Click(sender, e);
    }

    private static readonly string[] WEAPON_TYPES = new[] {"Bow", "ChargeAxe", "DualBlades", "GreatSword", "GunLance", "Hammer", "HeavyBowgun", "Horn", "InsectGlaive", "Lance", "LightBowgun", "LongSword", "ShortSword", "SlashAxe"};

    private IEnumerable<string> GetAllWeaponFilePaths(string type) {
        return WEAPON_TYPES.Select(s => @$"\natives\STM\data\Define\Player\Weapon\{s}\{s}{type}Data.user.2").ToList();
    }

    private void Btn_create_cheat_mods_Click(object sender, RoutedEventArgs e) {
        const string inPath                      = @"V:\MHR\re_chunk_000";
        const string outPath                     = @"R:\Games\Monster Hunter Rise\Mods\Cheat Mods";
        const string version                     = "v1.0";
        const string armorBasePath               = @"\natives\STM\data\Define\Player\Armor\ArmorBaseData.user.2";
        const string gemBasePath                 = @"\natives\STM\data\Define\Player\Equip\Decorations\DecorationsBaseData.user.2";
        const string armorRecipePath             = @"\natives\STM\data\Define\Player\Armor\ArmorProductData.user.2";
        const string layeredArmorRecipePath      = @"\natives\STM\data\Define\Player\Armor\PlOverwearProductUserData.user.2";
        const string decorationRecipePath        = @"\natives\STM\data\Define\Player\Equip\Decorations\DecorationsProductData.user.2";
        const string rampageDecorationRecipePath = @"\natives\STM\data\Define\Player\Equip\HyakuryuDeco\HyakuryuDecoProductData.user.2";

        var cheatMods = new CheatMod[] {
            new() {
                name   = "Armor With Max Slots Only",
                files  = new[] {armorBasePath},
                action = MaxSlots
            },
            new() {
                name   = "Armor With Max Skills Only",
                files  = new[] {armorBasePath},
                action = MaxSkills
            },
            new() {
                name  = "Armor With Max Slots & Skills",
                files = new[] {armorBasePath},
                action = data => {
                    MaxSlots(data);
                    MaxSkills(data);
                }
            },
            new() {
                name   = "Weapons With Max Slots Only",
                files  = GetAllWeaponFilePaths("Base"),
                action = MaxSlots
            },
            new() {
                name   = "Weapons With Max Sharpness Only",
                files  = GetAllWeaponFilePaths("Base"),
                action = MaxSharpness
            },
            new() {
                name  = "Weapons With Max Slots & Sharpness",
                files = GetAllWeaponFilePaths("Base"),
                action = data => {
                    MaxSlots(data);
                    MaxSharpness(data);
                }
            },
            new() {
                name   = "One Gem to Max Skill",
                files  = new[] {gemBasePath},
                action = MaxSkills
            },
            new() {
                name = "All In One",
                files = GetAllWeaponFilePaths("Base")
                        .Append(armorBasePath)
                        .Append(gemBasePath),
                action = data => {
                    MaxSlots(data);
                    MaxSharpness(data);
                    MaxSkills(data);
                }
            },
            new() {
                name = "No Crafting Requirements (All)",
                files = GetAllWeaponFilePaths("Product")
                        .Append(GetAllWeaponFilePaths("Process"))
                        .Append(GetAllWeaponFilePaths("Change"))
                        .Append(armorRecipePath)
                        .Append(layeredArmorRecipePath)
                        .Append(decorationRecipePath)
                        .Append(rampageDecorationRecipePath),
                action = NoCost
            },
            new() {
                name = "No Crafting Requirements (Weapons)",
                files = GetAllWeaponFilePaths("Product") // Forge
                        .Append(GetAllWeaponFilePaths("Process")) // Upgrade
                        .Append(GetAllWeaponFilePaths("Change")), // Layer
                action = NoCost
            },
            new() {
                name   = "No Crafting Requirements (Weapons, Layered Only)",
                files  = GetAllWeaponFilePaths("Change"),
                action = NoCost
            },
            new() {
                name   = "No Crafting Requirements (Decorations)",
                files  = new[] {decorationRecipePath, rampageDecorationRecipePath},
                action = NoCost
            },
            new() {
                name   = "No Crafting Requirements (Normal Armor)",
                files  = new[] {armorRecipePath},
                action = NoCost
            },
            new() {
                name   = "No Crafting Requirements (Layered Armor)",
                files  = new[] {layeredArmorRecipePath},
                action = NoCost
            }
        };

        foreach (var cheatMod in cheatMods) {
            var cheatPath = @$"{outPath}\{cheatMod.name}";
            Directory.CreateDirectory(cheatPath);

            var modInfo = new StringWriter();
            modInfo.WriteLine($"name={cheatMod.name}");
            modInfo.WriteLine($"version={version}");
            modInfo.WriteLine("description=A cheat mod.");
            modInfo.WriteLine("author=LordGregory");
            File.WriteAllText(@$"{cheatPath}\modinfo.ini", modInfo.ToString());

            foreach (var modFile in cheatMod.files) {
                var sourceFile = @$"{inPath}\{modFile}";
                var outFile    = @$"{cheatPath}\{modFile}";
                Directory.CreateDirectory(Path.GetDirectoryName(outFile)!);

                var dataFile = ReDataFile.Read(sourceFile);
                var data     = dataFile.rsz.objectData;
                cheatMod.action.Invoke(data);
                dataFile.Write(outFile);
            }
        }
    }

    private void Btn_max_sharpness_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        MaxSharpness(file.rsz.objectData);
    }

    private void MaxSharpness(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case ISharpness weapon:
                    weapon.SharpnessValList[0].Value = 10;
                    weapon.SharpnessValList[1].Value = 10;
                    weapon.SharpnessValList[2].Value = 10;
                    weapon.SharpnessValList[3].Value = 10;
                    weapon.SharpnessValList[4].Value = 10;
                    weapon.SharpnessValList[5].Value = 10;
                    weapon.SharpnessValList[6].Value = 340;
                    foreach (var handicraft in weapon.HandicraftValList) {
                        handicraft.Value = 0;
                    }
                    break;
            }
        }
    }

    private void Btn_max_slots_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        MaxSlots(file.rsz.objectData);
    }

    private void MaxSlots(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case Snow_data_ArmorBaseUserData_Param armor:
                    armor.DecorationsNumList[0].Value =
                        armor.DecorationsNumList[1].Value =
                            armor.DecorationsNumList[2].Value = 0;
                    armor.DecorationsNumList[3].Value = 3;
                    break;
            }
            if (obj is ISlots slots) {
                slots.SlotNumList[0].Value =
                    slots.SlotNumList[1].Value =
                        slots.SlotNumList[2].Value = 0;
                slots.SlotNumList[3].Value = 3;
            }
            if (obj is IRampageSlots rampageSlots) {
                rampageSlots.RampageSlotNumList[0].Value =
                    rampageSlots.RampageSlotNumList[1].Value = 0;
                rampageSlots.RampageSlotNumList[2].Value = 1;
            }
        }
    }

    private void Btn_max_skills_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        MaxSkills(file.rsz.objectData);
    }

    private void MaxSkills(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case Snow_data_ArmorBaseUserData_Param armor:
                    foreach (var level in armor.SkillLvList) {
                        if (level.Value > 0) level.Value = 10;
                    }
                    break;
                case Snow_data_DecorationsBaseUserData_Param decoration:
                    if (decoration.SkillLvList[0].Value > 0) decoration.SkillLvList[0].Value = 10;
                    break;
            }
        }
    }

    private void Btn_no_cost_Click(object sender, RoutedEventArgs e) {
        if (file == null) return;

        NoCost(file.rsz.objectData);
    }

    private void NoCost(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case ICraftingCost recipe:
                    foreach (var item in recipe.ItemIdList) {
                        item.Value = (uint) Snow_data_ContentsIdSystem_ItemId.I_Unclassified_None;
                    }
                    foreach (var itemNum in recipe.ItemNumList) {
                        itemNum.Value = 0;
                    }
                    break;
            }
            switch (obj) {
                case Snow_data_WeaponProductUserData_Param weaponProdData:
                    weaponProdData.MaterialCategory    = Snow_data_NormalItemData_MaterialCategory.None;
                    weaponProdData.MaterialCategoryNum = 0;
                    break;
                case Snow_data_WeaponChangeUserData_Param weaponChangeData:
                    weaponChangeData.MaterialCategory    = Snow_data_NormalItemData_MaterialCategory.None;
                    weaponChangeData.MaterialCategoryNum = 0;
                    break;
                case Snow_data_WeaponProcessUserData_Param weaponProcessData:
                    weaponProcessData.MaterialCategory    = Snow_data_NormalItemData_MaterialCategory.None;
                    weaponProcessData.MaterialCategoryNum = 0;
                    break;
                case Snow_equip_PlOverwearProductUserData_Param layeredProdData:
                    if (layeredProdData.MaterialCategoryNum > 0) layeredProdData.MaterialCategoryNum = 1;
                    break;
                case Snow_data_HyakuryuDecoProductUserData_Param rampageDecoProdData:
                    if (rampageDecoProdData.Point > 0) rampageDecoProdData.Point = 1;
                    break;
            }
        }
    }

    public struct CheatMod {
        public string                  name;
        public IEnumerable<string>     files;
        public Action<List<RszObject>> action;
    }
}