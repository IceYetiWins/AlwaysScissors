using HutongGames.PlayMaker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using UnityModManagerNet;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.Runtime.Remoting.Messaging;
using System.Collections;

namespace AlwaysScissors
{
    static class Main
    {
        public static bool enabled;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            modEntry.OnToggle = OnToggle;

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            modEntry.Logger.Log(SceneManager.GetActiveScene().name);

            return true;
        }

        [HarmonyPatch(typeof(IdleAction), "PlayRockPaperScissors")]
        static class IdleAction_PlayRockPaperScissors_Patch
        {
            static bool Prefix(bool ____rpsInProgress, IdleAction __instance)
            {
                if (!Main.enabled)
                    return true;

                ____rpsInProgress = true;

                __instance.Controller.PlayEmote("Scissors");

                var setRockPaperScissorsMethod = typeof(IdleAction).GetMethod("SetRockPapersScissors", BindingFlags.NonPublic | BindingFlags.Instance);

                __instance.Controller.StartActionCoroutine((IEnumerator)setRockPaperScissorsMethod.Invoke(__instance, new object[] { 2 }));

                return false;
            }
        }
    }
}
