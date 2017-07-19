using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class EventTriggerEx : 
	MonoBehaviour,
	IPointerEnterHandler,
	IPointerExitHandler,
	IPointerDownHandler,
	IPointerUpHandler,
	IPointerClickHandler,
	IInitializePotentialDragHandler,
	IBeginDragHandler,
	IDragHandler,
	IEndDragHandler,
	IDropHandler,
	IScrollHandler,
	IUpdateSelectedHandler,
	ISelectHandler,
	IDeselectHandler,
	IMoveHandler,
	ISubmitHandler,
	ICancelHandler
{
	public List<MonoBehaviour> m_delegates = new List<MonoBehaviour>();

	#region private methods

	#endregion
	void ExcuteMethod(Type _interfaceType, string _methodName, BaseEventData _eventData)
	{
		for (int idx = 0; idx < m_delegates.Count; idx++)
		{
			Type[] interfaces = m_delegates[idx].GetType().GetInterfaces();
			foreach (Type interfaceIt in interfaces)
			{
				if (interfaceIt == _interfaceType)
				{
					MethodInfo[] temp = interfaceIt.GetMethods();

					foreach (MethodInfo info in temp)
					{
						if (info.Name == _methodName)
						{
							BaseEventData[] paramTemp = new BaseEventData[1];
							paramTemp.SetValue(_eventData, 0);
							info.Invoke(m_delegates[idx], paramTemp);
						}
					}
				}
			}
		}
	}

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		ExcuteMethod(typeof(IPointerEnterHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		ExcuteMethod(typeof(IPointerExitHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		ExcuteMethod(typeof(IDragHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnDrop(PointerEventData eventData)
	{
		ExcuteMethod(typeof(IDropHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		ExcuteMethod(typeof(IPointerDownHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		ExcuteMethod(typeof(IPointerUpHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		ExcuteMethod(typeof(IPointerClickHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnSelect(BaseEventData eventData)
	{
		ExcuteMethod(typeof(ISelectHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnDeselect(BaseEventData eventData)
	{
		ExcuteMethod(typeof(IDeselectHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnScroll(PointerEventData eventData)
	{
		ExcuteMethod(typeof(IScrollHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnMove(AxisEventData eventData)
	{
		ExcuteMethod(typeof(IMoveHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnUpdateSelected(BaseEventData eventData)
	{
		ExcuteMethod(typeof(IUpdateSelectedHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnInitializePotentialDrag(PointerEventData eventData)
	{
		ExcuteMethod(typeof(IInitializePotentialDragHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		ExcuteMethod(typeof(IBeginDragHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnEndDrag(PointerEventData eventData)
	{
		ExcuteMethod(typeof(IEndDragHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnSubmit(BaseEventData eventData)
	{
		ExcuteMethod(typeof(ISubmitHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}

	public virtual void OnCancel(BaseEventData eventData)
	{
		ExcuteMethod(typeof(ICancelHandler), MethodBase.GetCurrentMethod().Name, eventData);
	}
}
