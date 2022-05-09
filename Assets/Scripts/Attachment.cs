using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Attachment : MonoBehaviour
{
    //[SerializeField] Transform AttachmentPoint;
    [SerializeField] GameObject AttachmentObject;
    [SerializeField] bool SnapToPoint;
    [SerializeField] float SnapDistance;
    [SerializeField] string AttachmentTag;
    [SerializeField] Material Highlight_Mat;
    [SerializeField] Text State;
    bool Attached = true;
    bool Highlighted;//if mouse is over the object
    bool selected;
    Vector3 AttachmentOffset;
    Vector3 MouseOffset;
    Material BaseMat;
    Vector3 IntMouse;
    float ZPlane;//only what to move the X and Y 
    
    public GameObject SetAttachmentObject
    {
        get { return AttachmentObject; }
        set { AttachmentObject = value; }
    }
    public bool SetSnapToPoint
    {
        get { return SnapToPoint; }
        set { SnapToPoint = value; }
    }
    public float SetSnapDistance
    {
        get { return SnapDistance; }
        set { SnapDistance = value; }
    }
    public string SetAttachmentTag
    {
        get { return AttachmentTag; }
        set { AttachmentTag = value; }
    }
    public Material SetHighlightMaterial
    {
        get { return Highlight_Mat; }
        set { Highlight_Mat = value; }
    }

    void Start()
    {
        AttachmentOffset = AttachmentObject.transform.position - this.transform.position;
        BaseMat = AttachmentObject.GetComponent<Renderer>().material;
        ZPlane = AttachmentObject.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100) && !Highlighted && !selected)
        {
            if (hit.transform.tag == AttachmentTag && CheckName(hit.transform.name))//if the mouse is over a selectable object check its name.
            {
                Highlighted = true;
                UpdateAttachedMat(AttachmentObject,Highlight_Mat);
            }
            
        }
        else if(!Physics.Raycast(ray, out hit, 100) && Highlighted && !selected)
        {
            UpdateAttachedMat(AttachmentObject, BaseMat);
            Highlighted = false;
            selected = false;
        }
        if (Input.GetMouseButtonDown(0) && Highlighted)//check if mouse is over the object when mouse is pressed.
        {
            selected = true;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 1f;
            IntMouse = Camera.main.ScreenToWorldPoint(mousePos);
            MouseOffset = IntMouse - AttachmentObject.transform.position;
        }
        else if (selected && Input.GetMouseButton(0))
        {
            if (Attached)
            {
                if (Vector3.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.ScreenToWorldPoint(IntMouse)) > SnapDistance)
                {
                    Attached = false;
                    State.text = "Detached";
                }
            }
            else
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 1f;
                Vector3 NewPos = (Camera.main.ScreenToWorldPoint(mousePos) - MouseOffset);
                NewPos.z = ZPlane;//make sure it stay on the same zplane
                AttachmentObject.transform.position = NewPos;
            }
        }
        else if (Input.GetMouseButtonUp(0) && Highlighted)//the object is no longer selected but it is still highlighted
        {
            selected = false;
        }
    }
    public bool CheckName(string NameToCheck)//this still can be approved appon.
    {
        string[] subStrings = NameToCheck.Split('_');
        string[] AttachedNames = AttachmentObject.name.Split('_');

        foreach (string str in subStrings)
        {
            if (AttachedNames[(AttachedNames.Length) - 1] == str)//check the last word of the attached object with all the words in the strings.
            {
                return true;
            }
        }
        return false;
    }
    void UpdateAttachedMat(GameObject Obj, Material Mat)
    {
        if (Obj.transform.childCount > 0)//if the game object has childern then go to each child and change the Mat
        {
            for (int i = 0; i < Obj.transform.childCount; i++)
            {
                UpdateAttachedMat(Obj.transform.GetChild(i).gameObject,Mat);
            }
        }
        Renderer ObjRend = Obj.GetComponent<Renderer>();//Get the renderer
        if (ObjRend != null)
        {
            ObjRend.material = Mat;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Vector3.Distance(other.transform.position,this.transform.position) <= SnapDistance && !Attached && !selected) {//if the object is in snapping distance and it no longer selected the snap the object to the point.
            Attached = true;
            State.text = "Attached";
            Debug.Log("Should snap");
            AttachmentObject.transform.position = other.transform.position + AttachmentOffset;
        }
    }
}
