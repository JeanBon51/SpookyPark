using System;
using System.Collections;
using System.Collections.Generic;
using AllIn1SpringsToolkit;
using UnityEngine;
using UnityEngine.Events;

public class SpringValueVector3 : SpringComponent
{
    private SpringVector3 _springVector3 = new SpringVector3();

    public UnityEvent<Vector3> OnValueUpdate = new UnityEvent<Vector3>();
    public Vector3 GetValue => this._springVector3.GetCurrentValue();

    private void Update()
    {
        if (initialized)
            OnValueUpdate?.Invoke(this._springVector3.GetCurrentValue());
    }
    public void SetUnifiedForceAndDragEnabled(bool value) => this._springVector3.SetUnifiedForceAndDragEnabled(value);
    public void SetTarget(Vector3 value) => this._springVector3.SetTarget(value);
    public void SetVelocity(Vector3 value) => this._springVector3.SetVelocity(value);
    public void SetVelocity(float value) => this._springVector3.SetVelocity(value);
    public void AddVelocity(Vector3 value) => this._springVector3.AddVelocity(value);
    public void SetForce(float value) => this._springVector3.SetForce(value);
    public void SetForce(Vector3 value) => this._springVector3.SetForce(value);
    public void SetDrag(Vector3 value) => this._springVector3.SetDrag(value);
    public void SetDrag(float value) => this._springVector3.SetDrag(value);
    protected override void SetCurrentValueByDefault()
    {
        this._springVector3.SetCurrentValue(Vector3.one);
    }
    protected override void SetTargetByDefault()
    {
        this._springVector3.SetTarget(Vector3.one);
    }
    protected override void RegisterSprings()
    {
        RegisterSpring(this._springVector3);
    }
    public override bool IsValidSpringComponent()
    {
        return true;
    }
}