using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.UI
{
    [RequireComponent(typeof(CastBehaviour))]
    public class UpdateSkill : MonoBehaviour, IPunObservable
    {
        // Skill bar
        public GameObject SkillBar;
        public GameObject SkillPlaceHolderPrefab;
        private List<GameObject> skills = new List<GameObject>();

        // Start is called before the first frame update
        private void Start()
        {
            foreach (var spell in GetComponent<CastBehaviour>().CharacterData.Spells)
            {
                // Instanciate the prefab the prefab which has an image component + image background for cooldown
                skills.Add(Instantiate(SkillPlaceHolderPrefab, SkillBar.transform));
                
                // Set the right icon
                skills.Last().transform.GetChild(0).GetComponent<Image>().sprite = spell.Icon;
            }
            
            GetComponent<CastBehaviour>().OnSpellThrown.AddListener(UpdateUI);
        }
        
        /// <summary>
        /// Callback when a spell is thrown, receive the spell index and his cooldown
        /// Will update background image to show that spell in cooldown
        /// </summary>
        /// <param name="spell">spell index</param>
        /// <param name="cooldown">spell cooldown</param>
        private void UpdateUI(int spell, float cooldown)
        {
            StartCoroutine(SkillCooldown(skills[spell].transform.GetChild(1).GetComponent<Image>(), cooldown));
        }

        /// <summary>
        /// Update the background fill of the spell over time
        /// </summary>
        /// <param name="image"></param>
        /// <param name="cooldown"></param>
        /// <returns></returns>
        private IEnumerator SkillCooldown(Image image, float cooldown)
        {
            var i = cooldown;
            image.fillAmount = 1;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                image.fillAmount = i / cooldown;
                i -= Time.deltaTime;
            }
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Can we sync spells cooldown ?
            if (stream.IsWriting)
            {
            }
            else
            {
            }
        }
    }
}