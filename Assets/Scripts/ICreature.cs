using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICreature
{
    // Start is called before the first frame update
    float Score();
    NeuroNet net { get; set; }
    void CreateNeuro();
    GameObject Respawn(); //возвращает новый объект
}
