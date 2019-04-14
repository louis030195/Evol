using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Ability;
using Evol.Game.Player;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

namespace Evol.Game.UI
{
    public class AbilityUI : MonoBehaviour, IPunObservable
    {
        // Skill bar
        public GameObject skillBar;
        public GameObject skillPlaceholderPrefab;
        private List<GameObject> abilities = new List<GameObject>();

        public GameObject skillInformationPanel;
        public TextMeshProUGUI skillInformationText;

        // Start is called before the first frame update
        private void Start()
        {
            if (GetComponentInParent<PlayerManager>() == null)
            {
                throw new UnityException($"UpdateSkill should be in a child of a PlayerManager & CastBehaviour component");
            }
            
            var i = 1;
            foreach (var ability in GetComponentInParent<PlayerManager>().characterData.abilities)
            {
                // Instanciate the prefab the prefab which has an image component + image background for cooldown
                abilities.Add(Instantiate(skillPlaceholderPrefab, skillBar.transform));
                
                // Set the key text
                abilities.Last().transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Key {i}";
                
                // Set the right icon
                abilities.Last().transform.GetChild(1).GetComponent<Image>().sprite = 
                    ability.icon;
                var trigger = abilities.Last().AddComponent<EventTrigger>();
                var entryPointerEnter = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
                entryPointerEnter.callback.AddListener((data) =>
                {
                    OnPointerEnterDelegate(data as PointerEventData, ability);
                } );
                var entryPointerExit = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
                entryPointerExit.callback.AddListener( ( data ) => { OnPointerExitDelegate(data as PointerEventData); } );
                trigger.triggers.Add( entryPointerEnter );
                trigger.triggers.Add(entryPointerExit);
                
                i++;
            }
            
            GetComponentInParent<CastBehaviour>().onSpellThrown.AddListener(UpdateUI);
        }
        
        private void OnPointerEnterDelegate( PointerEventData data, AbilityData ability )
        {
            // Only show the panel when cursor is visible
            if (!Cursor.visible) return;
            skillInformationPanel.SetActive(true);
            // Show the panel a little above the mouse
            skillInformationPanel.transform.position = new Vector3(Input.mousePosition.x * 0.8f, Input.mousePosition.y * 1.2f, Input.mousePosition.z);
            skillInformationText.text = ability.stat.ToString();
        }
        
        private void OnPointerExitDelegate( PointerEventData data )
        {
            skillInformationPanel.SetActive(false);
        }
        
        /// <summary>
        /// Callback when a spell is thrown, receive the spell index and his cooldown
        /// Will update background image to show that spell in cooldown
        /// </summary>
        /// <param name="spell">spell index</param>
        /// <param name="cooldown">spell cooldown</param>
        private void UpdateUI(int spell, float cooldown)
        {
            StartCoroutine(SkillCooldown(abilities[spell].transform.GetChild(2).GetComponent<Image>(), cooldown));
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