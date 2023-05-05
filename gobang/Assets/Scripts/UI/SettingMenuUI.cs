using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenuUI : MonoBehaviour
{





    public void Set_Screen_Size(int index) {

        switch (index)
        {
            case 0:
                Screen.SetResolution(400, 300, false);
                break;
            case 1:
                Screen.SetResolution(600, 475, false);
                break;
            case 2:
                Screen.SetResolution(800, 600, false);
                break;
            case 3:
                Screen.SetResolution(1024, 768, false);
                break;
            case 4:
                Screen.SetResolution(1600, 1200, false);
                break;
            case 5:
                Screen.SetResolution(2024, 1536, false);
                break;

        }
    
    }

    public void Set_IP_Address(int index)
    {

        switch (index)
        {
            case 0:
                PlayerLocalData.SetTargetIP("127.0.0.1");
                break;
            case 1:
                PlayerLocalData.SetTargetIP("58.153.224.180");
                break;

        }

    }
}
