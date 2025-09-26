using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
	//------------------ Static Variables -----------
	public static Board instance { get; private set; } = null;
	//------------------ Variables ------------------
	[SerializeField] private CarBank _carBank = null;
	[SerializeField] private SplineBank _splineBank = null;
	[SerializeField] private Parking _parking = null;
	[SerializeField] private AutoSortingContainer _autoSorting = null;
	[SerializeField] private Obj _objPrefab;
	private bool _isEnded = false;
	
	private int _carMoveIndex = int.MinValue;
	private Coroutine _loseRoutine = null;
	
	private TutorialPanel _tutorialPanel = null;
	
	//------------------ AutoProperties -------------
	public AutoSortingContainer autoSort { get; set; }
	public bool isInit { get; private set; }
	public List<Car> carList { get; set; } = new List<Car>();
	//------------------ Getter/Setter --------------
	public Parking parking => this._parking;

	//------------------ Unity Void -----------------
	public void Init(int levelIndex, int seed)
	{
		isInit = false;
		instance = this;
		this.autoSort = this.GetComponentInChildren<AutoSortingContainer>();
		this._isEnded = true;
		this.LoadMap(levelIndex, seed);
		this._isEnded = false;
		Spot.spotList.Clear();
		foreach (Spot spot in this.autoSort.GetComponentsInChildren<Spot>(true)) {
			spot.Init(this);
		}

		this._tutorialPanel = UIContainer.GetPanel(TypeMenu.Tutorial) as TutorialPanel;
		if(this._tutorialPanel != null) this._tutorialPanel.TryLaunchTutorial($"Level{seed}");
		isInit = true;
	}

	//--- Load Map ---
	private void LoadMap(int levelIndex, int seed) {
		this.DestroyMap();
		this._carBank.Init();
		MapData mapData = MapDataJsonInterface.GetMap(levelIndex.ToString());
		this._parking.transform.position = new Vector3(mapData.parkingPos[0], mapData.parkingPos[1], mapData.parkingPos[2]);
		CameraContainer.SetCameraFOV(mapData.cameraFOV);

		Dictionary<ObjType, int> colorGroupDict = new Dictionary<ObjType, int>();
		int index = 0;
		foreach (CarData cd in mapData.carDataArr) {
			Car prefab = this._carBank.GetCarByType(cd.carType);
			Car car = Instantiate(prefab, this._parking.carParent);
			car.gameObject.name = $"Car{index}";
			car.Init(this, cd, this._parking.carParent); 
			this.carList.Add(car);
			index++;
		}

		foreach (ObstacleData od in mapData.obstacleDataArr) {
			Transform prefab = this._carBank.GetObstacleByName(od.name);
			Transform obstacle = Instantiate(prefab, this._parking.obstacleParent);

			obstacle.localPosition = new Vector3(od.localPosition[0], od.localPosition[1], od.localPosition[2]);
			obstacle.localRotation = Quaternion.Euler(od.localRotation[0], od.localRotation[1], od.localRotation[2]);
		}

		this.autoSort = Instantiate(this._splineBank.GetSplinePrefabByNames(mapData.splineId), this.transform);

		if (mapData.spotDataArr != null) {
			Spot[] spots = this.autoSort.GetComponentsInChildren<Spot>();
			foreach (SpotData sd in mapData.spotDataArr) {
				for (int i = 0; i < sd.passengerData.number; i++) {
					Obj o = Instantiate(this._objPrefab, this.transform);
					o.Init(sd.passengerData.color, false);
					o.transform.SetParent(spots[sd.index].transform);
					spots[sd.index].currentObj.Add(o);
					o.transform.localPosition = UnityEngine.Random.Range(-1f, 1f) * Vector3.right + UnityEngine.Random.Range(-1f, 1f) * Vector3.forward;
				}
				spots[sd.index].SetType(sd.passengerData.color);
			}
		}

		this.autoSort.Init(this);
	}

	//--- Destroy Map ---
	private void DestroyMap() {
		this.autoSort = this.GetComponentInChildren<AutoSortingContainer>();
		if (this.autoSort != null) DestroyImmediate(this.autoSort.gameObject);
		for (int i = this._parking.carParent.childCount - 1; i > -1; i--) {
			DestroyImmediate(this._parking.carParent.GetChild(i).gameObject);
		}
		for (int i = this._parking.obstacleParent.childCount - 1; i > -1; i--) {
			DestroyImmediate(this._parking.obstacleParent.GetChild(i).gameObject);
		}
		for (int i = this._parking.limitParent.childCount - 1; i > -1; i--) {
			DestroyImmediate(this._parking.limitParent.GetChild(i).gameObject);
		}
	}

	//--- Win Check ---
	public void CheckWin() {
		if(this._isEnded) return;
		if (this.carList.Count == 0 && this.autoSort.isEmpty)
		{
			this._isEnded = true;
			GameContainer.Instance.Win();
		}
	}
	public void CheckLose() {
		if(this._isEnded) return;
		if(Application.isPlaying == false) return;
		if (this._loseRoutine != null) {
			this.StopCoroutine(this._loseRoutine);
		}

		if (this.autoSort.canAddObjContainer == false)
		{
			this._loseRoutine = this.StartCoroutine(this.RoutineCheckLose());
		}
	}

	private IEnumerator RoutineCheckLose()
	{
		yield return new WaitForSeconds(7f);
		if (this._isEnded == false && this.autoSort.canAddObjContainer == false)
		{
			GameContainer.Instance.Lose();
		}
	}

	//--- Traffic flow ---
	public void TryMoveCar(CarPart carPart) {
		bool move = carPart.TryMove(this._carMoveIndex);
		if (move)
		{
			this._carMoveIndex += 1;
		}
	}
}

[System.Serializable]
public class PassengersData {
	public List<PassengerGroupData> passengerGroupes = new List<PassengerGroupData>();
}

[System.Serializable]
public class PassengerGroupData {
	public ObjType color;
	public int number;
}
