﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMovement : MonoBehaviour {

    [HideInInspector]
    public Rigidbody rb;
    private Collider _collider;
    private MeshRenderer mr;

    public float selfGravityY = 5;
    public float slowGravityY = 3;
    public float hoverHeightTreshhold = 1f;
    private Vector3 _activeGravity;

    public Material defMaterial;
    public Material highMaterial;
    public Material actMaterial;
    public Material stopMaterial;

    public enum GravityMode
    {
        ERROR = 0,
        World = 1,
        Self = 2,
        Player = 3,
        Stop = 4,
        Slow = 5
    }

    public GravityMode currentMode;
    
	private void Start ()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();

        Collider[] cols = GetComponents<Collider>();
        foreach (Collider col in cols)
        {
            if (col.isTrigger == false)
                _collider = col;
        }

        _activeGravity = Vector3.zero;
        SetGravityMode(GravityMode.World);
    }

    private void FixedUpdate()
    {
        if (currentMode == GravityMode.World)
            return;

        rb.AddForce(_activeGravity * rb.mass * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    public void SetGravityMode(GravityMode mode)
    {
        if (currentMode == mode)
            return;

        currentMode = mode;
        rb.useGravity = true;

        if(currentMode == GravityMode.World)
        {
            mr.material = defMaterial;
        }
        else if(currentMode == GravityMode.Self)
        {
            mr.material = highMaterial;
            _activeGravity.y = selfGravityY;
            rb.useGravity = false;
        }
        else if(currentMode == GravityMode.Player)
        {
            mr.material = actMaterial;
            _activeGravity.y = 0f;
            rb.useGravity = false;
        }
        else if(currentMode == GravityMode.Stop)
        {
            mr.material = stopMaterial;
            _activeGravity.y = 0f;
            rb.useGravity = false;
        }
        else if(currentMode == GravityMode.Slow)
        {
            mr.material = defMaterial;
            _activeGravity.y = slowGravityY;
        }
        else
        {
            mr.material = null;
        }
    }

    public void Move(Vector3 movement)
    {
        RaycastHit hit;

        if (_collider is BoxCollider)
        {
            if (Physics.BoxCast(transform.position, transform.localScale * 0.5f, movement.normalized, out hit, transform.rotation, movement.magnitude))
                return;
        }
        else if (_collider is SphereCollider)
        {
            if (Physics.SphereCast(transform.position, transform.localScale.x * 0.5f, movement.normalized, out hit, movement.magnitude))
                return;
        }
        else if (_collider is CapsuleCollider)
        {
            Vector3 point1 = transform.position - transform.up * 0.5f * transform.localScale.y;
            Vector3 point2 = transform.position + transform.up * 0.5f * transform.localScale.y;

            if (Physics.CapsuleCast(point1, point2, transform.localScale.x * 0.5f, movement.normalized, movement.magnitude))
                return;
        }

        transform.position += movement;
    }

    public GravityMode GetGravityMode()
    {
        return currentMode;
    }
}
