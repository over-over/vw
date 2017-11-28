using System.Collections;
using System.IO;
using UnityEngine;
using System;
using TMPro;

public class HologramManager : MonoBehaviour {

    public GameObject prefab;
    public GameObject descPrefab;
    public Material mat;
    [SerializeField]
    private Vector3[] positions = {
        new Vector3(-1.5f, 0, -1f),         //Position 0
        new Vector3(0f, 0, 1.5f),           //Position 1
        new Vector3(1.5f, 0, -1f)           //Position 2              
    };
    private Vector3[] forwards = new Vector3[3];

    // Use this for initialization
	void Awake () {

        //Пример записи
        //LevelsContainer lc = new LevelsContainer();
        //HologramNode[] nodes = new HologramNode[2];
        //HologramLevel[] levels = new HologramLevel[2];
        //lc.Levels = levels;

        //Debug.Log(nodes[0]);

        //nodes[0] = new HologramNode();
        //nodes[0].Position = 1;
        //nodes[0].Name = "Test";
        //nodes[0].Description = "Another test";
        //nodes[0].Connections = new int[] { 1, 2 };

        //nodes[1] = new HologramNode();
        //nodes[1].Position = 2;
        //nodes[1].Name = "Test2";
        //nodes[1].Description = "Another test2";
        //nodes[1].Connections = new int[] { 1, 2 };

        //levels[0] = new HologramLevel();
        //levels[0].id = 0;
        //levels[0].name = "Мир чего-то";
        //levels[0].Nodes = nodes;

        //HologramData.Save(Path.Combine(Application.dataPath, "Saves/test.xml"), lc);
        //var levelsCollection = HologramData.Load(Path.Combine(Application.dataPath, "test.xml"));

        Debug.Log(Path.Combine(Application.dataPath, "Saves/test.xml"));
        if (File.Exists(Path.Combine(Application.dataPath, "Saves/test.xml")))
        {
            var xmlData = HologramData.Load(Path.Combine(Application.dataPath, "Saves/test.xml"));
            CalculateForwards();
            HologramBuild(xmlData);
        }
        else {
            Debug.Log("No file");
        }

	}

    public void HologramBuild(LevelsContainer data) {

        for (int i = 0; i < data.Levels.Length; i++)
        {
            Debug.Log(transform.position);
            //Создаём gameObject "уровня"
            GameObject levelGo = new GameObject();
            //Высота между уровнями
            float height = 1.5f;
            float offsetY = (data.Levels.Length - 0.5f - i) * height;
            levelGo.transform.parent = transform;
            levelGo.transform.name = "Level_" + i;
            levelGo.transform.localPosition = new Vector3(0, offsetY, 0);

            if (data.Levels[i].name != null) {
                CreateLevelDescription(data.Levels[i].name, levelGo.transform);
            }
            if (data.Levels[i].Nodes.Length > 1) {
                CreateTriangleLevel(levelGo.transform);
            }

            for (int j = 0; j < data.Levels[i].Nodes.Length; j++) {
                //Создаём gameObject шара и текста
                GameObject nodeGo = Instantiate(prefab);
                nodeGo.transform.parent = levelGo.transform;

                Transform text = nodeGo.transform.GetChild(1);
                text.GetComponent<TMP_Text>().text = data.Levels[i].Nodes[j].Name;
                CurveText(0.5f, text);
                nodeGo.transform.forward = forwards[data.Levels[i].Nodes[j].Position];
                nodeGo.transform.localPosition = positions[data.Levels[i].Nodes[j].Position];

                if (data.Levels[i].Nodes[j].Connections != null)
                {
                    CreateTriangleConnection(data.Levels[i].Nodes[j].Position, data.Levels[i].Nodes[j].Connections[0], data.Levels[i].Nodes[j].Connections[1], height, levelGo.transform);
                }
                if (data.Levels[i].Nodes[j].Description != null) {
                    CreateTriangleDescription(
                        data.Levels[i].Nodes[j].Description,
                        data.Levels[i].Nodes[j].Position,
                        data.Levels[i].Nodes[j].Connections[0],
                        data.Levels[i].Nodes[j].Connections[1],
                        height,
                        levelGo.transform);
                }
            }
        }
    }

    //Находим forward вектора для каждой из точек, чтобы относительно них выравнивать текст
    private void CalculateForwards() {
        for (int i = 0; i < positions.Length; i++) {
            int next = i + 1 < positions.Length ? i + 1 : 0;
            int prev = i - 1 > -1 ? i - 1 : positions.Length - 1;
            Vector3 v1 = positions[next] - positions[i];
            Vector3 v2 = positions[prev] - positions[i];
            Vector3 v3 = (v1 + v2) / 2;
            forwards[i] = -v3;
        }
    }

    private void CurveText(float radius, Transform tr) {
        TMP_Text text = tr.GetComponent<TMP_Text>();
        TMP_TextInfo textInfo = text.textInfo;
        Debug.Log(textInfo.characterCount);

        text.ForceMeshUpdate();

        //Находим угол поворота для каждой буквы, после чего меняем вершины каждой буквы, чтобы они образовывали дугу
        int vIndex = textInfo.characterInfo[0].vertexIndex;
        int mIndex = textInfo.characterInfo[0].materialReferenceIndex;
        Vector3[] verts = textInfo.meshInfo[mIndex].vertices;
        float angle = Mathf.Abs(verts[vIndex + 0].x - verts[vIndex + 2].x) / radius;
        float startAngle = textInfo.characterCount * angle / 2;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            float x0 = radius * Mathf.Cos(-startAngle + 1.5708f + angle * i);
            float z0 = radius * Mathf.Sin(-startAngle + 1.5708f + angle * i);
            float x1 = radius * Mathf.Cos(-startAngle + 1.5708f + angle * (i + 1));
            float z1 = radius * Mathf.Sin(-startAngle + 1.5708f + angle * (i + 1));

            vertices[vertexIndex + 0] = new Vector3(x0, vertices[vertexIndex + 0].y, z0);
            vertices[vertexIndex + 1] = new Vector3(x0, vertices[vertexIndex + 1].y, z0);
            vertices[vertexIndex + 2] = new Vector3(x1, vertices[vertexIndex + 2].y, z1);
            vertices[vertexIndex + 3] = new Vector3(x1, vertices[vertexIndex + 3].y, z1);
        }

        text.UpdateVertexData();
    }

    private void CreateLevelDescription(string name, Transform parent)
    {
        GameObject prefGo = Instantiate(descPrefab, parent);
        TextMesh txt = prefGo.transform.GetChild(0).GetComponent<TextMesh>();
        txt.text = name;
        txt.transform.rotation = new Quaternion(1f, 0, 0, 1f);
        txt.transform.localPosition = new Vector3(0, 0.05f, 0);
    }

    private void CreateTriangleDescription(string name, int p1, int p2, int p3, float height, Transform parent)
    {
        Vector3 cross;
        Vector3 center;
        float angle;

        if (p2 < 0)
        {
            p2 = Math.Abs(p2);
            cross = Vector3.Cross(positions[p3] - positions[p1], (positions[p1] + Vector3.up) - positions[p1]) / 15;
            angle = Vector3.SignedAngle(positions[p1], positions[p3], Vector3.up);
            center = (positions[p1] + positions[p2] + positions[p3] + new Vector3(0,height,0) * 2) / 3;
        }
        else {
            cross = Vector3.Cross(positions[p2] - positions[p1], (positions[p1] + Vector3.up) - positions[p1]) / 15;
            angle = Vector3.SignedAngle(positions[p1], positions[p2], Vector3.up);
            center = (positions[p1] + positions[p2] + positions[p3] + new Vector3(0, height, 0)) / 3;
        }
        if (angle > 0) cross = -cross;

        GameObject prefGo = Instantiate(descPrefab, parent);
        prefGo.transform.GetChild(0).GetComponent<TextMesh>().text = name;
        prefGo.transform.GetChild(0).GetComponent<TextMesh>().transform.localPosition = center - cross;
        prefGo.transform.GetChild(0).GetComponent<TextMesh>().transform.forward = cross;

    }

    private void CreateTriangleLevel(Transform parent)
    {
        Vector3[] vertices = {
            new Vector3 (positions[1].x, 0, positions[1].z),
            new Vector3 (positions[1].x, 0.05f, positions[1].z),
            new Vector3 (positions[0].x, 0.05f, positions[0].z),
            new Vector3 (positions[0].x, 0, positions[0].z),
            new Vector3 (positions[2].x, 0, positions[2].z),
            new Vector3 (positions[2].x, 0.05f, positions[2].z)
        };
        int[] triangles = {
	        1, 3, 0,
	        1, 2, 3,
	        4, 2, 5,
	        4, 3, 2,
	        0, 5, 1,
	        0, 4, 5,
            0, 3, 4,
            1, 5, 2
        };
        GameObject meshGo = new GameObject();
        meshGo.AddComponent<MeshFilter>();
        meshGo.AddComponent<MeshRenderer>();
        Mesh mesh = meshGo.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        MeshRenderer meshRend = meshGo.GetComponent<MeshRenderer>();
        meshRend.material = mat;
        meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        meshGo.transform.name = "connection";
        meshGo.transform.parent = parent;
        meshGo.transform.localPosition = Vector3.zero;
    }

    private void CreateTriangleConnection(int p1, int p2, int p3, float height, Transform parent)
    {
        Vector3 cross;
        float angle;
        int[] triangles;

        //Вершины треугольника
        Vector3[] vertices = {
			new Vector3 (positions[p1].x, 0, positions[p1].z),
			new Vector3 (positions[p1].x, 0, positions[p1].z),
			new Vector3 (positions[p3].x, height, positions[p3].z),
			new Vector3 (positions[p3].x, height, positions[p3].z),
			new Vector3 (0, 0, 0),
			new Vector3 (0, 0, 0)
		};

        //Если p2 = -1, значит вместо соседней вершины будет p1, но на уровень выше
        //Из-за этого надо поменять вершины образующих треугольников, иначе меш будет вывернут наизнанку
        if (p2 < 0)
        {
            p2 = Mathf.Abs(p2);
            vertices[4].y = height;
            vertices[5].y = height;
            cross = Vector3.Cross(positions[p3] - positions[p1], (positions[p1] + Vector3.up) - positions[p1]) / 15;
            angle = Vector3.SignedAngle(positions[p1], positions[p3], Vector3.up);
            triangles = new int[]{
                1, 4, 0,
                1, 5, 4,
                5, 3, 4,
                5, 2, 3,
                0, 2, 1,
                0, 3, 2,
                0, 4, 3,
                1, 2, 5
            };
        }
        else {
            cross = Vector3.Cross(positions[p2] - positions[p1], (positions[p1] + Vector3.up) - positions[p1]) / 15;
            angle = Vector3.SignedAngle(positions[p1], positions[p2], Vector3.up);
            triangles = new int[]{
                1, 3, 0,
                1, 2, 3,
                4, 2, 5,
                4, 3, 2,
                0, 5, 1,
                0, 4, 5,
                0, 3, 4,
                1, 5, 2
            };
        }

        vertices[4].x = positions[p2].x;
        vertices[4].z = positions[p2].z;
        vertices[5].x = positions[p2].x;
        vertices[5].z = positions[p2].z;

        //Смотрим какой угол между точками, чтобы перпендикулярные векторы всегда смотрели наружу
        if (angle < 0)
        {
            cross = -cross;
            vertices[0] += cross;
            vertices[3] += cross;
            vertices[4] += cross;
        }
        else {
            vertices[1] += cross;
            vertices[2] += cross;
            vertices[5] += cross;
        }

        //Без RecalculateNormals у мешей не будет правильного освещения, но и не будет неприятных линий на стыках треугольников, такой вот костыль
        GameObject meshGo = new GameObject();
        meshGo.AddComponent<MeshFilter>();
        meshGo.AddComponent<MeshRenderer>();
        Mesh mesh = meshGo.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.RecalculateNormals();
        
        MeshRenderer meshRend = meshGo.GetComponent<MeshRenderer>();
        meshRend.material = mat;
        meshRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        meshGo.transform.name = "connection";
        meshGo.transform.parent = parent;
        meshGo.transform.localPosition = Vector3.zero;
    }
}