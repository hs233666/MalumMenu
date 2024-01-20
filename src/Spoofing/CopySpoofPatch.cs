﻿using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System;
using BepInEx;
using UnityEngine;

namespace MalumMenu;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
public static class CopyFriendCode_PlayerPhysics_LateUpdate_Postfix
{
    //Postfix patch of PlayerPhysics.LateUpdate to open player pick menu to copy a player's friend code
    public static bool isActive;
    public static void Postfix(PlayerPhysics __instance){
        if (CheatToggles.copyPlayerFC){

            if (!isActive){

                //Close any player pick menus already open & their cheats
                if (Utils_PlayerPickMenu.playerpickMenu != null){
                    Utils_PlayerPickMenu.playerpickMenu.Close();
                    CheatToggles.DisablePPMCheats("copyPlayerFC");
                }

                List<GameData.PlayerInfo> playerDataList = new List<GameData.PlayerInfo>();

                //All players are saved to playerList apart from LocalPlayer
                foreach (var player in PlayerControl.AllPlayerControls){
                    if (!player.AmOwner){
                        playerDataList.Add(player.Data);
                    }
                }

                //New player pick menu made for copying a player's friend code to clipboard
                Utils_PlayerPickMenu.openPlayerPickMenu(playerDataList, (Action) (() =>
                {
                    GUIUtility.systemCopyBuffer = AmongUsClient.Instance.GetClientFromCharacter(Utils_PlayerPickMenu.targetPlayerData.Object).FriendCode;
                
                    Utils.showPopup($"\n{Utils_PlayerPickMenu.targetPlayerData.PlayerName}'s friend code\nhas been copied to your clipboard");
                }));

                isActive = true;
            }

            //Deactivate cheat if menu is closed
            if (Utils_PlayerPickMenu.playerpickMenu == null){
                CheatToggles.copyPlayerFC = false;
            }

        }else{
            if (isActive){
                isActive = false;
            }
        }
    }
}