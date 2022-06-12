using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    #region Singleton
    public static Inventory instance;
    
    void Awake () {
        if (instance != null) {
            Debug.LogWarning("More than one inventory");
            return;
        }
        instance = this;
    }
    #endregion
    
    public InventoryUI inventoryUI;

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallBack;

    public int space = 20;

    public List<BaseItem> items = new List<BaseItem>();

    public BaseItem usedItem = null;

    public bool Add (BaseItem item) {
        if (!item.item.isDefaultItem) {
            if (items.Count < space) {
                items.Add(item);

                if (onItemChangedCallBack != null) {
                    onItemChangedCallBack.Invoke();
                }
                return true;
            }
            Logger.instance.AddLog("Not enough room in inventory");
        }
        return false;
    }

    public void Remove (BaseItem item) {
        items.Remove(item);

        if (onItemChangedCallBack != null) {
            onItemChangedCallBack.Invoke();
        }
    }

    public void ToggleInventory() {
        inventoryUI.ToggleInventory();
    }

}
