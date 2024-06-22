using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLossMenuOpenedPublisher : MonoBehaviour
{
    public delegate void OnWinLossMenuOpened(bool isWinLossMenuOpen);
    public static event OnWinLossMenuOpened WinLossMenuOpened;

    public static void NotifyWinLossMenuOpen(bool isWinLossMenuOpen)
    {
        WinLossMenuOpened?.Invoke(isWinLossMenuOpen);
    }


    /* 
        void OnEnable(){
            WinLossMenuOpenedPublisher.WinLossMenuOpened += OnWinLossMenuOpened;
        }
        void OnDisable(){
            WinLossMenuOpenedPublisher.WinLossMenuOpened -= OnWinLossMenuOpened;
        }
        void OnWinLossMenuOpened(bool isWinLossMenuOpen){
            isMenuOpen = isWinLossMenuOpen;
        }
    */
}
