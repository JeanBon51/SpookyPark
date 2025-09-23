//[System.Serializable]
//public class MapData {
//    public string name => $"Level {this.id}";
//    public string id;
//    public int splineId;
//	public float cameraFOV;
//	public float[] parkingPos;
//	public CarData[] carDataArr;
//	public ObstacleData[] obstacleDataArr;
//    public SpotData[] spotDataArr;
//}

//[System.Serializable]
//public class CarData {
//    public CarType carType;
//    public ObjType carColor;
//	public float[] localPosition;
//    public float[] localRotation;
//    public PassengersData passengerData;
//}

[System.Serializable]
public class ObstacleData {
    public string name;
	public float[] localPosition;
	public float[] localRotation;
}

[System.Serializable]
public class SpotData {
    public int index;
    public PassengerGroupData passengerData;
}

// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

[System.Serializable]
public class MapData {
	public string name => $"Level {this.id}";
	public string id;
	public int splineId;
	public float cameraFOV;
	public float[] parkingPos;
	public CarData[] carDataArr;
	public ObstacleData[] obstacleDataArr;
	public SpotData[] spotDataArr;
}

[System.Serializable]
public class CarData {
	public CarType carType;
	// public ObjType carColor;
	public float[] localPosition;
	public float[] localRotation;
	public PassengersData passengerData;
	public ObjType[] carPartColors;
	public bool isHidden;
}