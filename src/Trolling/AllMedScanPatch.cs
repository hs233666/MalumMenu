﻿using HarmonyLib;
using Hazel;
namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class AllMedScan_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to play the MedBay scan animation for all players
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        try{
            if (CheatToggles.allMedScan != isActive)
            {
                var HostData = AmongUsClient.Instance.GetHost();
                if (HostData != null && !HostData.Character.Data.Disconnected){
                    foreach (var sender in PlayerControl.AllPlayerControls)
                    {
                        foreach (var recipient in PlayerControl.AllPlayerControls)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(sender.NetId, (byte)RpcCalls.SetScanner, SendOption.None, AmongUsClient.Instance.GetClientIdFromCharacter(recipient));
                            writer.Write(CheatToggles.allMedScan);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                        }
                    }
                }

                isActive = CheatToggles.allMedScan;
            }
        }catch{}
    }
}