using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectedAgentUI : MonoBehaviour
{
    public SpectatorController spectatorController;
    public Text text;
    private void Update()
    {
        if(spectatorController.controlledPlayer != null)
        {
            Color32 color32 = spectatorController.controlledPlayer.renderer.material.color;
            string hexString = $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}";

            text.text = $"Controlling <color={hexString}>{spectatorController.controlledPlayer.name}</color>";
        }
        else
        {
            text.text = $"";
        }

    }
}
