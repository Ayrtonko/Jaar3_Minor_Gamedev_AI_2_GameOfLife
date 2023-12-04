using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Renderer myRenderer;
    private Color aliveColor;
    private Color deadColor;
    private Color hoverColor;
    public int xPos;
    public int yPos;
    public bool alive = false;
    public bool nextAliveState = false;

    public bool isMouseOver;

    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        SetMyColor();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAlive();
    }

    void UpdateAlive()
    {
        if (!alive && !isMouseOver)
        {
            myRenderer.material.color = deadColor;
        }

        else if (alive && !isMouseOver)
        {
            myRenderer.material.color = aliveColor;
        }

        else if (alive || !alive && isMouseOver)
        {
            myRenderer.material.color = hoverColor;
        }
    }

    void AliveToggle()
    {
        this.alive = !alive;
    }

    private void OnMouseOver()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }

    private void OnMouseDown()
    {
        AliveToggle();
    }

    void SetMyColor()
    {
        if ((xPos + yPos) % 2 == 0)
        {
            deadColor = Color.Lerp(Color.white, Color.grey, 0.5f);
        }
        else
        {
            deadColor = Color.white;
        }

        myRenderer.material.color = deadColor;
        hoverColor = Color.green;
        aliveColor = Color.black;
    }
}