using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class AutoSortingContainer : MonoBehaviour
{
    public static Dictionary<Car,float> LockValue = new Dictionary<Car,float>();
    
    //------------------ Variables ------------------
    [SerializeField] private SplineContainer _spline;
    [SerializeField] private TextMeshProUGUI _tNB;
    [SerializeField] private int _maxObjContainer = 0;

    private Coroutine _routine;
    private Board _board;
    
    private List<Car> _listObjContainer = new List<Car>();
    //------------------ AutoProperties -------------
    //------------------ Getter/Setter --------------
    public bool canAddObjContainer => nbCar < this._maxObjContainer;
    public int nbCar = 0;
    public bool isEmpty => this._listObjContainer.Count == 0;
    //------------------ Unity Void -----------------
    //------------------ Void -----------------------
    public void Init(Board board) {
        this._board = board;
    }
    
    public void AddObjContainer(Car car, bool inverse)
    {
        if(inverse)
            car.AddToSplineBackward(this._spline);
        else 
            car.AddToSplineForward(this._spline);
    }
    
    public void AddCart(Car car)
    {
        this._listObjContainer.Add(car);
        this.UpdateCartNb();
        if (this._listObjContainer.Count == _maxObjContainer)
        {
            this._board.CheckLose();
        }
    }
    public void RemoveCart(Car car)
    {
        this._listObjContainer.Remove(car);
        this.UpdateCartNb();
        if (this._listObjContainer.Count < _maxObjContainer)
        {
            this._board.CheckLose();
        }
    }

    public void UpdateCartNb()
    {
        nbCar = 0;
        foreach (Car car in this._listObjContainer)
        {
            if (car.IsFull() == false) nbCar++;
        }
        this._tNB.text = $"{nbCar}/{this._maxObjContainer}";
    }
    
}
