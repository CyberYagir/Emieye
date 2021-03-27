using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool over;


    public void Update()
    {
        if (over)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.1f, 1.2f, 1.2f), 3 * Time.deltaTime);
        }
        else
        {

            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, 1f, 1f), 3 * Time.deltaTime);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        over = false;
    }
}
