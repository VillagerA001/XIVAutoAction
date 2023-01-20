﻿using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using ImGuiNET;
using RotationSolver;
using RotationSolver.Configuration.RotationConfig;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Rotations.CustomRotation;
using RotationSolver.SigReplacers;
using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace RotationSolver.Windows.RotationConfigWindow;

internal partial class RotationConfigWindow
{
    private void DrawRotationTab()
    {
        ImGui.Text(LocalizationManager.RightLang.Configwindow_AttackItem_Description);

        ImGui.BeginChild("Attack Items", new Vector2(0f, -1f), true);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));

        if (ImGui.BeginTabBar("Job Items"))
        {
            DrawRoleItems();
            ImGui.EndTabBar();
        }
        ImGui.PopStyleVar();
        ImGui.EndChild();
    }

    private static void DrawRoleItems()
    {
        foreach (var key in IconReplacer.CustomRotationsDict.Keys)
        {
            var rotations = IconReplacer.CustomRotationsDict[key];
            if (rotations == null || rotations.Length == 0) continue;

            if (ImGui.BeginTabItem(key.ToName()))
            {
                DrawRotations(rotations);
                ImGui.EndTabItem();
            }

            //Display the tooltip on the header.
            if (ImGui.IsItemHovered() && _roleDescriptionValue.TryGetValue(key, out string roleDesc))
            {
                ImGui.SetTooltip(roleDesc);
            }
        }
    }

    private static void DrawRotations(IconReplacer.CustomRotationGroup[] rotations)
    {
        for (int i = 0; i < rotations.Length; i++)
        {
            if (i > 0) ImGui.Separator();

            var rotation = IconReplacer.GetChoosedRotation(rotations[i]);

            var canAddButton = Service.ClientState.LocalPlayer != null
                && rotation.JobIDs.Contains((ClassJobID)Service.ClientState.LocalPlayer.ClassJob.Id);

            rotation.Display(rotations[i].rotations, canAddButton);
        }
    }

    private static void DrawRotation(ICustomRotation rotation, bool canAddButton)
    {
        ImGui.Spacing();

        DrawTargetHostileTYpe(rotation);
        DrawSpecialRoleSettings(rotation.Job.GetJobRole(), rotation.JobIDs[0]);

        ImGui.Spacing();

        rotation.Configs.Draw(canAddButton);
    }

    private static void DrawTargetHostileTYpe(ICustomRotation rotation)
    {
        var isAllTargetAsHostile = (int)IconReplacer.GetTargetHostileType(rotation.Job);
        ImGui.SetNextItemWidth(300);
        if (ImGui.Combo(LocalizationManager.RightLang.Configwindow_Params_RightNowTargetToHostileType + $"##HostileType{rotation.GetHashCode()}", ref isAllTargetAsHostile, new string[]
        {
             LocalizationManager.RightLang.Configwindow_Params_TargetToHostileType1,
             LocalizationManager.RightLang.Configwindow_Params_TargetToHostileType2,
             LocalizationManager.RightLang.Configwindow_Params_TargetToHostileType3,
        }, 3))
        {
            Service.Configuration.TargetToHostileTypes[rotation.Job.RowId] = (byte)isAllTargetAsHostile;
            Service.Configuration.Save();
        }
    }

    private static void DrawSpecialRoleSettings(JobRole role, ClassJobID job)
    {
        if (role == JobRole.Healer)
        {
            DrawHealerSettings(job);
        }
        else if (role == JobRole.Tank)
        {
            DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Params_HealthForDyingTank,
                () => ConfigurationHelper.GetHealthForDyingTank(job),
                (value) => Service.Configuration.HealthForDyingTanks[job] = value);
        }
    }

    private static void DrawHealerSettings(ClassJobID job)
    {
        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Param_HealthAreaAbility,
            () => ConfigurationHelper.GetHealAreaAbility(job),
            (value) => Service.Configuration.HealthAreaAbilities[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Param_HealthAreaSpell,
            () => ConfigurationHelper.GetHealAreafSpell(job),
            (value) => Service.Configuration.HealthAreafSpells[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Params_HealingOfTimeSubtractArea,
            () => ConfigurationHelper.GetHealingOfTimeSubtractArea(job),
            (value) => Service.Configuration.HealingOfTimeSubtractAreas[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Param_HealthSingleAbility,
            () => ConfigurationHelper.GetHealSingleAbility(job),
            (value) => Service.Configuration.HealthSingleAbilities[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Param_HealthSingleSpell,
            () => ConfigurationHelper.GetHealSingleSpell(job),
            (value) => Service.Configuration.HealthSingleSpells[job] = value);

        DrawDragFloat(job, LocalizationManager.RightLang.Configwindow_Params_HealingOfTimeSubtractSingle,
            () => ConfigurationHelper.GetHealingOfTimeSubtractSingle(job),
            (value) => Service.Configuration.HealingOfTimeSubtractSingles[job] = value);
    }

    private static void DrawDragFloat(ClassJobID job, string desc, Func<float> getValue, Action<float> setValue)
    {
        const float speed = 0.005f;

        if (getValue == null || setValue == null) return;

        var value = getValue();
        ImGui.SetNextItemWidth(DRAG_NUMBER_WIDTH);
        if (ImGui.DragFloat($"{desc}##{job}{desc}", ref value, speed, 0, 1))
        {
            setValue(value);
            Service.Configuration.Save();
        }
    }
}