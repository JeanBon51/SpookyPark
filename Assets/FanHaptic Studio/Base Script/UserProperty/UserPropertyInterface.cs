using UnityEngine;
using UnityEngine.Events;

public class UserPropertyInterface
{
	private const string KeyFirstLaunch = "User-FirstLaunch";
	public static string UserID => SystemInfo.deviceUniqueIdentifier;

    public static bool FirstLaunch {
        get
        {
            if (SaveDataJsonInterface.Exist<bool>(KeyFirstLaunch))
            {
                return SaveDataJsonInterface.GetBool(KeyFirstLaunch);
            }
            else
            {
                return false;
            }
        }
        set
        {
            SaveDataJsonInterface.SetBool(KeyFirstLaunch,value);
        }
    }
}
