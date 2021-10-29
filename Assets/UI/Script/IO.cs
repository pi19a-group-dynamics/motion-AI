using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class IO
{
    //private string path = Config.SavesPath;


    //public void SaveScene(string _saveName, List<List<GameObject>> _branches)
    //{
    //    //Папка для текущего сохранения
    //    DirectoryInfo dirInfo = new DirectoryInfo(path);
    //    if (!dirInfo.Exists){
    //        dirInfo.Create();
    //    }
    //    dirInfo.CreateSubdirectory(_saveName);

    //    Debug.Log(Directory.GetDirectories(dirInfo.Name));
    //    string savePath = path + $"\\{_saveName}"; //Путь к текущему сохранению



    //    //Файлы сейва
    //    for (int i = 0; i < _branches.Count; i++)
    //    {
    //        List<GameObject> branch = _branches[i]; //Ветка

    //        //Папка для текущей ветки
    //        dirInfo = new DirectoryInfo(savePath);
    //        if (dirInfo.Exists)
    //        {
    //            dirInfo.Create();
    //        }
    //        dirInfo.CreateSubdirectory($"branch {i + 1}");
    //        string branchPath = savePath + $"\\branch {i + 1}"; //Путь к текущей ветке


    //        for (int j = 0; j < branch.Count; j++)
    //        {
    //            GameObject creature = branch[j]; //Особь

    //            NeuroNet neuron_net = creature.GetComponent<ICreature>().net;

    //            FileStream stream = File.Create($"{branchPath}\\nuron_net {j + 1}.xml");//Отдельный документ для нейросети
    //            XmlSerializer serializer = new XmlSerializer(typeof(NeuroNet));

    //            serializer.Serialize(stream, neuron_net);
    //            stream.Close();
    //        }
    //    }

    //    //Информация о сохранении
    //    using (FileStream stream = File.Create($"{savePath}\\Info.xml"))
    //    {
    //        SaveInfo info = new SaveInfo();

    //        info.BranchCount = _branches.Count;
    //        info.BranchSize = _branches[0].Count;
    //        info.CratureType = _branches[0][0].GetComponent<ICreature>().GetType().ToString();

    //        XmlSerializer serializer = new XmlSerializer(typeof(SaveInfo));

    //        serializer.Serialize(stream, info);
    //    }

        //Данные для графиков
        //File.Copy(path, newPath, true);
    //}
}
