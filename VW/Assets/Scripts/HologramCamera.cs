using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramCamera : MonoBehaviour {

    public GameObject hologram;
    private int level = 0;
	// Use this for initialization
	void Start () {
        Vector3 newPos = hologram.transform.GetChild(level).transform.position;
        //hologram.GetComponent<HologramManager>().
        Debug.Log("Level y " + newPos.y);
        this.transform.position = new Vector3(this.transform.position.x, 2.5f + newPos.y, this.transform.position.z);
        Debug.Log("Children " + hologram.transform.childCount);
        SelectColor(hologram.transform.GetChild(level + 1), hologram.transform.GetChild(level));
	}

    void Update() {

        if (Input.GetMouseButton(0))
        {
            transform.LookAt(hologram.transform.GetChild(level));
            //transform.RotateAround(hologram.transform.GetChild(level).position, Vector3.up, Input.GetAxis("Mouse X") * 10f);
            transform.RotateAround(hologram.transform.GetChild(level).position, Vector3.left, Input.GetAxis("Mouse Y") * 10f);
        }

        //This is just retarted
        for (int i = 0; i < hologram.transform.childCount; i++) {
            //hologram.transform.GetChild(i).Find("HologramSphere(Clone)").GetChild(1).transform.LookAt(this.transform);
            for (int j = 0; j < hologram.transform.GetChild(i).childCount; j++) {
                if (hologram.transform.GetChild(i).GetChild(j).transform.name == "HologramSphere(Clone)") {
                    hologram.transform.GetChild(i).GetChild(j).GetChild(1).transform.LookAt(2 * hologram.transform.GetChild(i).GetChild(j).GetChild(1).transform.position - this.transform.position);
                }
            }
        }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (level - 1 > -1)
                {
                    SelectColor(hologram.transform.GetChild(level), hologram.transform.GetChild(level - 1));
                    Debug.Log("Renderers: " + hologram.transform.GetChild(level).GetComponentsInChildren<MeshRenderer>().Length);
                    level = level - 1;
                    Vector3 newPos = hologram.transform.GetChild(level).transform.position;
                    this.transform.position = new Vector3(this.transform.position.x, 2.5f + newPos.y, this.transform.position.z);
                }
            }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (level + 1 < hologram.transform.childCount)
            {
                SelectColor(hologram.transform.GetChild(level), hologram.transform.GetChild(level + 1));
                level = level + 1;
                Vector3 newPos = hologram.transform.GetChild(level).transform.position;
                this.transform.position = new Vector3(this.transform.position.x, 2.5f + newPos.y, this.transform.position.z);
            }
        }
    }

    private void SelectColor(Transform prev, Transform next)
    {
        MeshRenderer[] mr = next.GetComponentsInChildren<MeshRenderer>();
        MeshRenderer[] mr2 = prev.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mesh in mr){
            if(mesh.gameObject.name != "name")
            mesh.material.color = new Color(0.5f, 1f, 0.83f, 1f);
        }
        foreach (MeshRenderer mesh in mr2)
        {
            if (mesh.gameObject.name != "name")
            mesh.material.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public void SetAnchor(Vector3 pos) {
        
    }

}
