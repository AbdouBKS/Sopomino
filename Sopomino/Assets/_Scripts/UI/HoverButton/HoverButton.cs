using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(HoverText))]
    public class HoverButton : Button
    {
        private HoverText _hoverText;

        protected override void Start()
        {
            base.Start();
            _hoverText = GetComponent<HoverText>();
        }

        public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            HoverManager.Instance.SetHoverText(_hoverText.Text);
        }

        public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            HoverManager.Instance.SetHoverText("");
        }
    }
}
