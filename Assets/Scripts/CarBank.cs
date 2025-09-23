using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataBank/CarBank")]
public class CarBank : ScriptableObject {
	[SerializeField] private CarTypeEntry[] _carEntries;
	private Dictionary<CarType, Car> _carDictionary = new Dictionary<CarType, Car>();

	[SerializeField] private ObstacleEntry[] _obstacleEntries;


	public void Init() {
		foreach (CarTypeEntry ce in this._carEntries) {
			this._carDictionary[ce.carType] = ce.prefab;
		}
	}

	public Car GetCarByType(CarType carType) {
		return this._carDictionary[carType];
	}

	public Transform GetObstacleByName(string name) {
		foreach (ObstacleEntry oe in this._obstacleEntries) {
			if (name.Contains(oe.obstacleName))
				return oe.prefab;
		}
		return null;
	}
}

[System.Serializable]
public class CarTypeEntry {
	public CarType carType;
	public Car prefab;
}

[System.Serializable]
public class ObstacleEntry{
	public string obstacleName;
	public Transform prefab;
}
