using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.UI
{
    public abstract class UpdateStat : MonoBehaviour, IPunObservable
    {
        // Bottom bar
        public Image Fill;
        public RectTransform OverheadFill;
        
        protected void UpdateUI(float value)
        {
            if (OverheadFill)
            {
                var sizeY = OverheadFill.sizeDelta.y;
                OverheadFill.sizeDelta = new Vector2(sizeY * value * 10, sizeY);
            }

            Fill.fillAmount = value;
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Synchronize
            if (stream.IsWriting)
            {
                stream.SendNext(OverheadFill.sizeDelta);
                stream.SendNext(Fill.fillAmount);
            }
            else
            {
                OverheadFill.sizeDelta = (Vector2) stream.ReceiveNext();
                Fill.fillAmount = (float) stream.ReceiveNext();
            }
        }
    }
}