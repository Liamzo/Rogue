using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Starting Equipment", menuName = "Inventory/New Starting Equipment")]
public class StartingEquipment : ScriptableObject {
    public List<Equipment> equipment;
}