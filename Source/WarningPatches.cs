using System;
using System.Linq.Expressions;
using System.Reflection;
using HarmonyLib;
using PrehistoricKingdom;
using PrehistoricKingdom.Modular;

namespace WarningRemovals
{
    internal static class WarningPatches
    {
        private static bool IsSandbox()
        {
            return Game.HasInstance &&
                   SimManager.simSettings.gameMode ==
                       SimManager.GameMode.Sandbox;
        }

        [HarmonyPatch]
        internal static class MissingBuildingPatch
        {
            private static readonly Type IssueType =
                AccessTools.TypeByName(
                    "PrehistoricKingdom.Management.ParkIssues." +
                    "MissingCriticalModuleParkIssue")
                ?? throw new TypeLoadException(
                    "MissingCriticalModuleParkIssue was not found.");

            // Create once; no repeated reflection during warning checks.
            private static readonly Func<object, ModuleType>
                GetRequiredModule = CreateModuleGetter();

            private static MethodBase TargetMethod()
            {
                return AccessTools.DeclaredMethod(
                    IssueType, "IsPresent")
                    ?? throw new MissingMethodException(
                        IssueType.FullName, "IsPresent");
            }

            private static Func<object, ModuleType> CreateModuleGetter()
            {
                MethodInfo getter = AccessTools.PropertyGetter(
                    IssueType, "RequiredModuleType")
                    ?? throw new MissingMethodException(
                        IssueType.FullName, "get_RequiredModuleType");

                ParameterExpression instance =
                    Expression.Parameter(typeof(object), "instance");

                return Expression.Lambda<Func<object, ModuleType>>(
                    Expression.Call(
                        Expression.Convert(instance, IssueType),
                        getter),
                    instance).Compile();
            }

            private static bool Prefix(
                object __instance,
                ref bool __result)
            {
                if (!IsSandbox())
                    return true;

                ModuleType required = GetRequiredModule(__instance);

                if (required != ModuleType.ModuleStaffCenter &&
                    required != ModuleType.ModuleLoadingBay &&
                    required != ModuleType.ModuleVisitorSpawner)
                {
                    return true;
                }

                __result = false;
                return false;
            }
        }

        [HarmonyPatch]
        internal static class EnclosureStaffAccessPatch
        {
            private static MethodBase TargetMethod()
            {
                Type issueType = AccessTools.TypeByName(
                    "PrehistoricKingdom.Management.ParkIssues." +
                    "HabitatInaccessibleStaffParkIssue")
                    ?? throw new TypeLoadException(
                        "HabitatInaccessibleStaffParkIssue was not found.");

                return AccessTools.DeclaredMethod(
                    issueType, "IsPresent")
                    ?? throw new MissingMethodException(
                        issueType.FullName, "IsPresent");
            }

            private static bool Prefix(ref bool __result)
            {
                if (!IsSandbox())
                    return true;

                __result = false;
                return false;
            }
        }
    }
}
