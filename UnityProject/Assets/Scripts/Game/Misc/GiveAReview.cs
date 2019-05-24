using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Evol.Game.Misc
{
    public class GiveAReview : MonoBehaviour
    {
        public GameObject Input;
        public TextMeshProUGUI ButtonText;

        public void OnClick()
        {
            Application.OpenURL("https://forms.gle/8xNrC56X6S71Q7Sh7");
            /*
            if (Input.activeInHierarchy)
            {
                ButtonText.text = $"Thanks !";
                if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("review"))
                    PhotonNetwork.LocalPlayer.CustomProperties["review"] = Input.GetComponent<TMP_InputField>().text;
                else
                    PhotonNetwork.LocalPlayer.CustomProperties.Add("review", Input.GetComponent<TMP_InputField>().text);
            }
            else
            {
                ButtonText.text = $"Send";
            }
            Input.SetActive(!Input.activeInHierarchy);*/
        }
    }
}