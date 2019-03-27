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
        public bool HideOverheadBar = true;
        public GameObject OverheadBar;
        public RectTransform OverheadFill;


        // Start is called before the first frame update
        protected virtual void Start()
        {
            // We don't want to see our own overhead bar, just others
            if (gameObject.GetPhotonView().IsMine)
            {
                HideOverheadBar = true;
            }
            
            if (HideOverheadBar)
            {
                OverheadBar.gameObject.SetActive(false);
            }
        }
        
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