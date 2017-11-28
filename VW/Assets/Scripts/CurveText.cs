using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurveText : MonoBehaviour {

    private TMP_Text text;
    public float radius = 5f;
    
    // Use this for initialization
	void Start () {
        text = transform.GetComponent<TMP_Text>();
        TMP_TextInfo textInfo = text.textInfo;
        Debug.Log(textInfo.characterCount);

        text.ForceMeshUpdate();

        //Находим угол поворота каждой буквы
        int vIndex = textInfo.characterInfo[0].vertexIndex;
        int mIndex = textInfo.characterInfo[0].materialReferenceIndex;
        Vector3[] verts = textInfo.meshInfo[mIndex].vertices;
        float angle = Mathf.Abs(verts[vIndex + 0].x - verts[vIndex + 2].x) / radius;

        float startAng = textInfo.characterCount * angle / 2;
        Debug.Log(startAng);

        for (int i = 0; i < textInfo.characterCount; i++) {
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            float x0 = transform.position.x + radius * Mathf.Cos(-startAng + 1.5708f + angle * i);
            float z0 = transform.position.z + radius * Mathf.Sin(-startAng + 1.5708f + angle * i);
            float x1 = transform.position.x + radius * Mathf.Cos(-startAng + 1.5708f + angle * (i + 1));
            float z1 = transform.position.z + radius * Mathf.Sin(-startAng + 1.5708f + angle * (i + 1));

            vertices[vertexIndex + 0] = new Vector3(x0, vertices[vertexIndex + 0].y, z0);
            vertices[vertexIndex + 1] = new Vector3(x0, vertices[vertexIndex + 1].y, z0);
            vertices[vertexIndex + 2] = new Vector3(x1, vertices[vertexIndex + 2].y, z1);
            vertices[vertexIndex + 3] = new Vector3(x1, vertices[vertexIndex + 3].y, z1);
        }

        text.UpdateVertexData();

	}
	
}
