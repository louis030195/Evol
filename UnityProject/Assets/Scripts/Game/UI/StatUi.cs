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
        [Header("Parameters")]
        public bool hide;
        [Tooltip("Whether to fade in when stat changing then fade out (typically for others bar)")] public bool fade;
        [Tooltip("Time between fade in-out, leave it if not using fade")] public int fadeDuration = 5;
        [Tooltip("Target stat to monitor, LEAVE EMPTY IF ITS MONITORING PARENT COMPONENT")] public string targetTag;

        [Header("Objects references")] 
        public GameObject bar;
        public Image fill;
        public TextMeshProUGUI valueText;
        public RectTransform overheadFill; // TODO: change name

        protected GameObject target; // Which object stat it's referencing (leave empty if it's the parent object)
        
        
        private void Awake()
        {
            target = targetTag.Equals(string.Empty) ? transform.parent.gameObject : GameObject.FindWithTag(targetTag);
            
            if (hide)
            {
                bar.SetActive(false);
                enabled = false;
            }
            
            // Start with hided bar
            if (fade) bar.SetActive(false);
        }

        protected void UpdateUI(float value)
        {
            var sizeY = overheadFill.sizeDelta.y;
            overheadFill.sizeDelta = new Vector2(sizeY * value * 10, sizeY);
            fill.fillAmount = value;
            valueText.text = value > 0 ? value.ToString("0%") : "0%";
            if(fade && !bar.activeInHierarchy) StartCoroutine(FadeInFadeOut());
        }

        private IEnumerator FadeInFadeOut() // TODO: real fade xd ... not on off
        {
            bar.SetActive(true);
            yield return new WaitForSeconds(5f);
            bar.SetActive(false);
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