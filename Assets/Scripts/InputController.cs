using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private Camera mCamera;

    private Dictionary<Collider, IInputElement> mInputElements = new Dictionary<Collider, IInputElement>();
    private HashSet<IInputElement> mMouseOverElements = new HashSet<IInputElement>();

    public void AddTrackingElement(IInputElement inputElement)
    {
        mInputElements.Add(inputElement.MainCollider, inputElement);
    }

    public void RemoveTrackingElement(IInputElement inputElement)
    {
        if (mInputElements.ContainsKey(inputElement.MainCollider))
            mInputElements.Remove(inputElement.MainCollider);
    }

    private void Update()
    {
        ProcessMouse();
    }

    private void ProcessMouse()
    {
        ProcessMouseOver();
        ProcessMouseClicks();

    }

    private void ProcessMouseClicks()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                IInputElement element;
                if (mInputElements.TryGetValue(hitInfo.collider, out element))
                {
                    element.ProcessClick(0);
                }
            }
        }
    }

    private void ProcessMouseOver()
    {
        Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, 100000f);
        List<IInputElement> elements = new List<IInputElement>();

        if (hitInfos.Length != 0)
        {
            IInputElement element;
            foreach (var hitInfo in hitInfos)
            {
                mInputElements.TryGetValue(hitInfo.collider, out element);

                if (element != null)
                {
                    elements.Add(element);
                    if (!mMouseOverElements.Contains(element))
                    {
                        mMouseOverElements.Add(element);
                        element.ProcessMouseOver();
                    }
                }
            }
        }

        var theyLostMouse = mMouseOverElements.Except(elements).ToList();
        foreach (var looser in theyLostMouse)
        {
            looser.ProcessMouseLost();
            mMouseOverElements.Remove(looser);
        }
    }
}
