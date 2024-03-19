using Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    //to/do this should be abstracted to some sort of IActionButton or whatever #prototyp
    public class ActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        public GameObject nameObj;
        public BuildingActionExec actionExec;
        private BuildingAction _action;

        private void Start() {
            _action = actionExec.action;
            nameObj.GetComponent<TextMeshProUGUI>().text = _action.Name;
            nameObj.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            nameObj.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData) {
            nameObj.SetActive(false);
        }
    }
}