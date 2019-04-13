using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.UI
{
    public abstract class StatUi : MonoBehaviour
    {
        // Bottom bar
        public Image fill;
        public TextMeshProUGUI valueText;
        public RectTransform overheadFill;
        
        protected void UpdateUI(float value)
        {
            var sizeY = overheadFill.sizeDelta.y;
            overheadFill.sizeDelta = new Vector2(sizeY * value * 10, sizeY);
            fill.fillAmount = value;
            valueText.text = value.ToString("0%");
        }
    }
}