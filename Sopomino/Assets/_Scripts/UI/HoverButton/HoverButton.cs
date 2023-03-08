using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(HoverInfo))]
    public class HoverButton : Button
    {
        private HoverInfo _hoverText;

        protected override void Start()
        {
            base.Start();
            _hoverText = GetComponent<HoverInfo>();

            onClick.AddListener(() =>
            {
                HoverManager.Instance.chooseDescription = _hoverText.description;
                HoverManager.Instance.chooseTitle = _hoverText.title;
            });
        }

        public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            HoverManager.Instance.HoverEnter(_hoverText.description);
        }

        public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            HoverManager.Instance.HoverExit();
        }
    }
}
