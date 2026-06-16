using System.Collections;
using UnityEngine;
using MelonLoader;
using GHPC;
using GHPC.State;
using GHPC.Vehicle;
using GHPC.Weapons;

[assembly: MelonInfo(typeof(AccurateBMP2Mod.AccurateBMP2), "Accurate_BMP2", "1.0.0", "QwertyRyo")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace AccurateBMP2Mod
{
    public class AccurateBMP2 : MelonMod
    {
        private static GameObject gameManager;
        static bool activeScene = false;

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "MainMenu2_Scene" || sceneName == "t64_menu" || sceneName == "MainMenu2-1_Scene")
            {
                activeScene = false;
                return;
            }

            gameManager = GameObject.Find("_APP_GHPC_");
            if (gameManager == null) { return; }
            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(FixBMP2Deviation), GameStatePriority.Medium);
        }

        public IEnumerator FixBMP2Deviation(GameState _)
        {
            if (activeScene == true) { yield break; }
            activeScene = true;

            Vehicle[] list = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);

            foreach (var vehicle in list)
            {
                if (vehicle.UniqueName != "BMP2_SA" && vehicle.UniqueName != "BMP2") { continue; }

                WeaponsManager wm = vehicle.GetComponent<WeaponsManager>();
                if (wm == null) { MelonLogger.Error($"{vehicle.UniqueName}: WeaponsManager not found"); continue; }
                if (wm.Weapons == null || wm.Weapons.Length == 0) { MelonLogger.Error($"{vehicle.UniqueName}: Weapons array is empty"); continue; }

                WeaponSystem weapon = wm.Weapons[0].Weapon;
                if (weapon == null) { MelonLogger.Error($"{vehicle.UniqueName}: Weapons[0].Weapon is null"); continue; }

                weapon.BaseDeviationAngle = 0.065f;
                MelonLogger.Msg($"{vehicle.UniqueName}: BaseDeviationAngle set to 0.065");
            }

            yield break;
        }
    }
}
