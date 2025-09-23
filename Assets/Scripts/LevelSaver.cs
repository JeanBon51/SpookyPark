
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelSaver : MonoBehaviour {
	[SerializeField, Header("Refs")] private Board _board;
	[SerializeField] private Camera _camera;
	[SerializeField] private CarBank _carBank = null;
	[SerializeField] private SplineBank _splineBank = null;
	[SerializeField] private Obj _objPrefab;

	[SerializeField, Header("Data")] private int _associatedSplineId;
	[SerializeField] private CustomRadomParameter _customRadomParameter;
	[SerializeField, Header("LoadMethods")] private int _levelIndex = 0;

	[PropertySpace(15)]
	[Button]
	private void SaveCurrMap() {
		MapData mapData = new MapData();
		mapData.id = this._levelIndex.ToString();
		mapData.parkingPos = new float[] { this._board.parking.transform.position.x, this._board.parking.transform.position.y, this._board.parking.transform.position.z };
		mapData.splineId = this._associatedSplineId;
		mapData.cameraFOV = this._camera.fieldOfView;

		List<CarData> cdl = new List<CarData>();
		Car[] allCars = this._board.parking.carParent.GetComponentsInChildren<Car>();
		foreach (Car car in allCars) {
			CarData cd = new CarData();
			cd.carType = car.carType;
			cd.localPosition = new float[] { car.transform.localPosition.x, car.transform.localPosition.y, car.transform.localPosition.z };
			cd.localRotation = new float[] { car.transform.localEulerAngles.x, car.transform.localEulerAngles.y, car.transform.localEulerAngles.z };
			cd.passengerData = car.forcePassengerData;
			cd.carPartColors = this.GetCarPartsColors(car);
			cd.isHidden = car.isHidden;
			cdl.Add(cd);
		}
		mapData.carDataArr = cdl.ToArray();

		List<ObstacleData> odl = new List<ObstacleData>();
		List<Transform> allObstacles = this._board.parking.obstacleParent.GetComponentsInChildren<Transform>().ToList();
		allObstacles.Remove(this._board.parking.obstacleParent);
		foreach (Transform obstacle in allObstacles) {
			ObstacleData od = new ObstacleData();
			od.name = obstacle.gameObject.name;
			od.localPosition = new float[] { obstacle.transform.localPosition.x, obstacle.transform.localPosition.y, obstacle.transform.localPosition.z };
			od.localRotation = new float[] { obstacle.transform.localEulerAngles.x, obstacle.transform.localEulerAngles.y, obstacle.transform.localEulerAngles.z };
			odl.Add(od);
		}
		mapData.obstacleDataArr = odl.ToArray();

		List<SpotData> spotDataList = new List<SpotData>();
		AutoSortingContainer autoSort = this._board.GetComponentInChildren<AutoSortingContainer>();
		if (autoSort != null) {
			Spot[] spots = autoSort.GetComponentsInChildren<Spot>();

			for (int i = 0; i < spots.Length; i++) {
				if (spots[i].forcePassenger.color == ObjType.None) continue;
				SpotData sd = new SpotData();
				sd.index = i;
				sd.passengerData = spots[i].forcePassenger;
				spotDataList.Add(sd);
			}
		}

		if (spotDataList.Count > 0) mapData.spotDataArr = spotDataList.ToArray();
		else mapData.spotDataArr = null;

		MapDataJsonInterface.SetMap(mapData);
		Debug.Log($"Map {this._levelIndex} Saved");
	}

	private ObjType[] GetCarPartsColors(Car car) {
		List<ObjType> result = new List<ObjType>();
		foreach (CarPart cp in car.carParts) {
			result.Add(cp.eColor);
		}
		return result.ToArray();
	}

	[PropertySpace(15)]
	[Button]
	private void GetMap() {
		this.DestroyMap();
		this._carBank.Init();
		MapData mapData = MapDataJsonInterface.GetMap(this._levelIndex.ToString());
		this._board.parking.transform.position = new Vector3(mapData.parkingPos[0], mapData.parkingPos[1], mapData.parkingPos[2]);
		this._associatedSplineId = mapData.splineId;
		this._camera.fieldOfView = mapData.cameraFOV;

#if UNITY_EDITOR
		ConvertToPrefabInstanceSettings settings = new ConvertToPrefabInstanceSettings();
#endif
		int index = 0;
		foreach (CarData cd in mapData.carDataArr) {
			Car prefab = this._carBank.GetCarByType(cd.carType);
			Car car = Instantiate(prefab, this._board.parking.carParent);
			car.gameObject.name = $"Car{index}";
#if UNITY_EDITOR
			PrefabUtility.ConvertToPrefabInstance(car.gameObject, prefab.gameObject, settings, InteractionMode.AutomatedAction);
#endif
			car.LDInit(this._board, cd, this._board.parking.carParent);
			index++;
		}

		foreach (ObstacleData od in mapData.obstacleDataArr) {
			Transform prefab = this._carBank.GetObstacleByName(od.name);
			Transform obstacle = Instantiate(prefab, this._board.parking.obstacleParent);
#if UNITY_EDITOR
			PrefabUtility.ConvertToPrefabInstance(obstacle.gameObject, prefab.gameObject, settings, InteractionMode.AutomatedAction);
#endif

			obstacle.localPosition = new Vector3(od.localPosition[0], od.localPosition[1], od.localPosition[2]);
			obstacle.localRotation = Quaternion.Euler(od.localRotation[0], od.localRotation[1], od.localRotation[2]);
		}

		AutoSortingContainer autoSortPrefab = this._splineBank.GetSplinePrefabByNames(mapData.splineId);
		if (autoSortPrefab) {
			this._board.autoSort = Instantiate(this._splineBank.GetSplinePrefabByNames(mapData.splineId), this._board.transform);
			if (mapData.spotDataArr != null) {
				Spot[] spots = this._board.autoSort.GetComponentsInChildren<Spot>();
				foreach (SpotData sd in mapData.spotDataArr) {
					for (int i = 0; i < sd.passengerData.number; i++) {
						Obj o = Instantiate(this._objPrefab, this.transform);
						o.Init(sd.passengerData.color, false);
						o.transform.SetParent(spots[sd.index].transform);
						spots[sd.index].currentObj.Add(o);
						o.transform.localPosition = UnityEngine.Random.Range(-1f, 1f) * Vector3.right + UnityEngine.Random.Range(-1f, 1f) * Vector3.forward;
					}
					spots[sd.index].forcePassenger = sd.passengerData;
					spots[sd.index].SetType(sd.passengerData.color, false);
				}
			}
		}

		Debug.Log($"Map {this._levelIndex} Loaded");
	}

	[PropertySpace(15)]
	[Button]
	private void DestroyMap() {
		AutoSortingContainer autoSort = this._board.GetComponentInChildren<AutoSortingContainer>();
		if (autoSort != null) DestroyImmediate(autoSort.gameObject);

		for (int i = this._board.parking.carParent.childCount - 1; i > -1; i--) {
			DestroyImmediate(this._board.parking.carParent.GetChild(i).gameObject);
		}
		for (int i = this._board.parking.obstacleParent.childCount - 1; i > -1; i--) {
			DestroyImmediate(this._board.parking.obstacleParent.GetChild(i).gameObject);
		}
		for (int i = this._board.parking.limitParent.childCount - 1; i > -1; i--) {
			DestroyImmediate(this._board.parking.limitParent.GetChild(i).gameObject);
		}
	}

	//[PropertySpace(15)]
	//[Button]
	//private void SetPassengerData() {
	//	Car[] cars = this.GetComponentsInChildren<Car>();

	//	Dictionary<ObjType, int> colorGroupDict = new Dictionary<ObjType, int>();
	//	List<CarPartInfo> cil = new List<CarPartInfo>();
	//	foreach (Car car in cars) {
	//		foreach (CarPart cp in car.carParts) {
	//			if (colorGroupDict.ContainsKey(cp.eColor) == false) colorGroupDict.Add(cp.eColor, car.maxObj);
	//			else colorGroupDict[cp.eColor] += 2;

	//			cil.Add(new CarPartInfo(cp, cp.eColor));
	//		}
	//	}

	//	AutoSortingContainer autoSort = this._board.GetComponentInChildren<AutoSortingContainer>();
	//	if (autoSort != null) {
	//		Spot[] spots = autoSort.GetComponentsInChildren<Spot>();
	//		foreach (Spot spot in spots) {
	//			if (spot.forcePassenger.color == ObjType.None) continue;
	//			if (colorGroupDict.ContainsKey(spot.forcePassenger.color) == false) colorGroupDict.Add(spot.forcePassenger.color, spot.forcePassenger.number);
	//			else colorGroupDict[spot.forcePassenger.color] += spot.forcePassenger.number;
	//		}
	//	}

	//	for (int i = 0; i < cil.Count; i++) {
	//		PassengersData passengerData = new PassengersData();

	//		int maxPlace = cil[i].carPartRef.maxObj;
	//		int numberOfGroup = (int)cil[i].carType + 1;

	//		passengerData.passengerGroupes = this.GetPassengerInfos(cil[i].carColor, maxPlace, numberOfGroup, colorGroupDict);

	//		cil[i].carRef.SetPassengerForceData(passengerData);
	//	}

	//	List<ObjType> toRemove = new List<ObjType>();
	//	foreach (KeyValuePair<ObjType, int> pair in colorGroupDict) {
	//		if(pair.Value <= 0)
	//			toRemove.Add(pair.Key);
	//	}

	//	foreach (ObjType type in toRemove) {
	//		colorGroupDict.Remove(type);
	//	}

	//	if (colorGroupDict.Keys.Count > 0) {
	//		Debug.Log("**********************************************************");
	//		Debug.Log("remaining Passenger:");
	//		foreach (KeyValuePair<ObjType, int> pair in colorGroupDict) {
	//			Debug.Log($"{pair.Key} => {pair.Value}");
	//		}
	//		Debug.Log("**********************************************************");
	//	}
	//}

	//public List<PassengerGroupData> GetPassengerInfos(ObjType carColor, int maxPlaces, int gNum, Dictionary<ObjType, int> dictionary) {
	//	List<PassengerGroupData> result = new List<PassengerGroupData>();

	//	int toRemove = 0;
	//	int gSize = Mathf.CeilToInt((float)maxPlaces / gNum);
	//	List<ObjType> types = new List<ObjType>();

	//	if (this._customRadomParameter != null && this._customRadomParameter.ownColorWeight != 0 && this._customRadomParameter.otherColorWeight != 0) {
	//		foreach (ObjType type in dictionary.Keys) {
	//			for (int i = 0; i < (type == carColor ? this._customRadomParameter.ownColorWeight : this._customRadomParameter.otherColorWeight); i++) {
	//				types.Add(type);
	//			}
	//		}
	//		types.Shuffle();

	//		for (int i = 0; i < gNum; i++) {
	//			toRemove = gSize;
	//			ObjType color = types[UnityEngine.Random.Range(0, types.Count)];
	//			if (toRemove > maxPlaces) toRemove = maxPlaces;
	//			if (dictionary[color] < toRemove) toRemove = dictionary[color];
	//			if (toRemove > 0) {
	//				PassengerGroupData pg = new PassengerGroupData();
	//				pg.color = color;
	//				pg.number = toRemove;
	//				dictionary[color] -= toRemove;
	//				maxPlaces -= toRemove;
	//				result.Add(pg);
	//			}
	//			types = types.Where(x => x != color).ToList();
	//		}
	//	}
	//	else {
	//		types = dictionary.Keys.ToList();
	//		types.Remove(carColor);
	//		types.Add(carColor);
	//		foreach (ObjType color in types) {
	//			toRemove = gSize;
	//			if (toRemove > maxPlaces) toRemove = maxPlaces;
	//			if (dictionary[color] < toRemove) toRemove = dictionary[color];
	//			if (toRemove > 0) {
	//				PassengerGroupData pg = new PassengerGroupData();
	//				pg.color = color;
	//				pg.number = toRemove;
	//				dictionary[color] -= toRemove;
	//				maxPlaces -= toRemove;
	//				result.Add(pg);
	//			}
	//			toRemove = gSize - toRemove;
	//		}
	//	}



	//	return result;
	//}
	
	[Button]
	private void CheckPassengerAttribution() {
		Car[] cars = this.GetComponentsInChildren<Car>();

		Dictionary<ObjType, int> neededPassengerDict = new Dictionary<ObjType, int>();
		Dictionary<ObjType, int> actualPassengerDict = new Dictionary<ObjType, int>();

		foreach (Car car in cars) {
			foreach (CarPart cp in car.carParts) {
				if (neededPassengerDict.ContainsKey(cp.eColor) == false) neededPassengerDict.Add(cp.eColor, 2);
				else neededPassengerDict[cp.eColor] += 2;
			}

			foreach (PassengerGroupData pg in car.forcePassengerData.passengerGroupes) {
				if (actualPassengerDict.ContainsKey(pg.color) == false) actualPassengerDict.Add(pg.color, pg.number);
				else actualPassengerDict[pg.color] += pg.number;
			}
		}

		AutoSortingContainer autoSort = this._board.GetComponentInChildren<AutoSortingContainer>();
		if (autoSort != null) {
			Spot[] spots = autoSort.GetComponentsInChildren<Spot>();
			foreach (Spot spot in spots) {
				if (spot.forcePassenger.color == ObjType.None) continue;
				if (actualPassengerDict.ContainsKey(spot.forcePassenger.color) == false) actualPassengerDict.Add(spot.forcePassenger.color, spot.forcePassenger.number);
				else actualPassengerDict[spot.forcePassenger.color] += spot.forcePassenger.number;
			}
		}

		Debug.Log("**********************************************************");
		bool noPB = true;
		foreach (ObjType type in neededPassengerDict.Keys) {
			if (actualPassengerDict.ContainsKey(type) == false) {
				Debug.Log($"Missing {neededPassengerDict[type]} of color {type}"); 
				noPB = false;
				continue;
			}
			int remaining = neededPassengerDict[type] - actualPassengerDict[type];
			if (remaining != 0) {
				Debug.Log($"{type} = {remaining}");
				noPB = false;
			}
		}
		if(noPB) Debug.Log("Everything is correct");
		Debug.Log("**********************************************************");
	}

	[PropertySpace(15)]
	[Button]
	private void Refresh() {
#if UNITY_EDITOR
		AssetDatabase.Refresh();
#endif
	}

	//[PropertySpace(15)]
	//[Button]
	//private void ChangeData() {
	//	List<MapData> oldMapDataList = new List<MapData>();
	//	List<NewMapData> newMapDataList = new List<NewMapData>();
	//	for (int i = 0; i < 30; i++) {
	//		MapData omd = MapDataJsonInterface.GetMap(i.ToString());
	//		NewMapData nmd = new NewMapData();
	//		nmd.id = omd.id;
	//		nmd.splineId = omd.splineId;
	//		nmd.cameraFOV = omd.cameraFOV;
	//		nmd.parkingPos = omd.parkingPos;
	//		nmd.obstacleDataArr = omd.obstacleDataArr;
	//		nmd.spotDataArr = omd.spotDataArr;



	//		List<NewCarData> ncdl = new List<NewCarData>();
	//		foreach (CarData cd in omd.carDataArr) {
	//			NewCarData ncd = new NewCarData();

	//			ncd.carType = cd.carType;
	//			ncd.carColor = cd.carColor;
	//			ncd.localPosition = cd.localPosition;
	//			ncd.localRotation = cd.localRotation;
	//			ncd.passengerData = cd.passengerData;

	//			List<ObjType> cpc = new List<ObjType>();
	//			int length = (int)ncd.carType + 1;

	//			for (int a = 0; a < length; a++) {
	//				cpc.Add(ncd.carColor);
	//			}
	//			ncd.carPartColors = cpc.ToArray();
	//			ncdl.Add(ncd);
	//		}

	//		nmd.carDataArr = ncdl.ToArray();
	//		newMapDataList.Add(nmd);
	//	}

	//	MapDataJsonInterface.SetMaps(newMapDataList.ToArray());
	//}
}

public class CarPartInfo {
	public CarPart carPartRef;
	public ObjType carColor;

	public CarPartInfo(CarPart carPartRef, ObjType carColor) {
		this.carPartRef = carPartRef;
		this.carColor = carColor;
	}
}

[System.Serializable]
public class CustomRadomParameter {
	public int ownColorWeight;
	public int otherColorWeight;
}