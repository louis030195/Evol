using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.UI
{
    public abstract class StatUi : MonoBehaviour, IPunObservable
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
            valueText.text = value > 0 ? value.ToString("0%") : "0%";
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Synchronize life
            if (stream.IsWriting)
            {
                stream.SendNext(overheadFill.sizeDelta);
                stream.SendNext(fill.fillAmount);
                stream.SendNext(valueText.text);
            }
            else
            {
                overheadFill.sizeDelta = (Vector2) stream.ReceiveNext();
                fill.fillAmount = (float) stream.ReceiveNext();
                valueText.text = stream.ReceiveNext().ToString();
            }
        }
    }
}