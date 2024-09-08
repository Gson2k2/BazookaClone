using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyGame.Script.CoreGame;
using UnityEditor;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform gunSpawnParent;

    private GameObject _currentGunModel;
    private GameObject _currentGunDropModel;

    private void Start()
    {
        OnGunInit();
    }

    public void OnGunInit()
    {
        OnReset();
        var userData = GameSettingManager.Instance.OnDataLoad();
        var gunID = userData.playerResources.PlayerCurrentGunEquipmentID;
        var gunModel = new GameObject();
        try
        {
            gunModel = Resources.Load<ShopItemData>("ScriptObject/GunShopData")
                .itemDataArr.First(x => x.itemID == gunID).itemModel;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
            gunModel = Resources.Load<ShopItemData>("ScriptObject/GunShopData").itemDataArr.First().itemModel;
        }

        _currentGunModel = Instantiate(gunModel,gunSpawnParent);
        _currentGunDropModel = Instantiate(gunModel, gunSpawnParent);
        
        _currentGunDropModel.AddComponent<Rigidbody>();
        _currentGunDropModel.AddComponent<BoxCollider>();
    }

    void OnReset()
    {
        try
        {
            Destroy(_currentGunModel);
            Destroy(_currentGunDropModel);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }
    
}
