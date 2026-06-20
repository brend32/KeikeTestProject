using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;


namespace UnityEngine.UI.Extensions
{
    public class ScrollRectEx : ScrollRect
    {
        private bool routeToParent = false;

        /// <summary>
        /// Always route initialize potential drag event to parents
        /// </summary>
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.initializePotentialDrag);
            base.OnInitializePotentialDrag(eventData);
        }

        /// <summary>
        /// Drag event
        /// </summary>
        public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (routeToParent)
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.dragHandler);
            else
                base.OnDrag(eventData);
        }

        /// <summary>
        /// Begin drag event
        /// </summary>
        public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (!horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
                routeToParent = true;
            else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
                routeToParent = true;
            else
                routeToParent = false;

            if (routeToParent)
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.beginDragHandler);
            else
                base.OnBeginDrag(eventData);
        }

        /// <summary>
        /// End drag event
        /// </summary>
        public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (routeToParent)
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.endDragHandler);
            else
                base.OnEndDrag(eventData);
            routeToParent = false;
        }
    }
}