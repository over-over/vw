using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class HologramData {

    public static void Save(string path, LevelsContainer data)
    {
        if (!Directory.Exists(Path.Combine(Application.dataPath, "Saves"))) Directory.CreateDirectory(Path.Combine(Application.dataPath,"Saves"));
        var serializer = new XmlSerializer(typeof(LevelsContainer));
        //var stream = new FileStream(path, FileMode.Create);
        var stream = new StreamWriter(path, false, System.Text.Encoding.UTF8);
        serializer.Serialize(stream, data);
        stream.Close();
    }

    public static LevelsContainer Load(string path)
    {
        var serializer = new XmlSerializer(typeof(LevelsContainer));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as LevelsContainer;
        }
    }
}

[XmlRoot("LevelsCollection")]
public class LevelsContainer
{
    [XmlArray("Levels")]
    [XmlArrayItem("Level")]
    public HologramLevel[] Levels;
}

[XmlType("Level")]
public class HologramLevel
{
    [XmlAttribute("id")]
    public int id;
    [XmlAttribute("name")]
    public string name;
    [XmlArray("Nodes")]
    [XmlArrayItem("Node")]
    public HologramNode[] Nodes;
}

public class HologramNode
{

    public int Position;
    public string Name;
    public string Description;
    [XmlArray("Connections")]
    [XmlArrayItem("Link")]
    public int[] Connections;

}