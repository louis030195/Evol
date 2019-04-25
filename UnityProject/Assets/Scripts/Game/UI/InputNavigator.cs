using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Evol.Game.UI
{
    public class InputNavigator : MonoBehaviour
    {
        EventSystem system;

        private void Start()
        {
            system = EventSystem.current;// EventSystemManager.currentSystem;
     
        }
        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                var next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
         
                if (next != null)
                {
             
                    var inputfield = next.GetComponent<TMP_InputField>();
                    if (inputfield != null)
                        inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret
                    
                    system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
                }
                else Debug.Log($"next nagivation element not found");
         
            }
        }
    }
}