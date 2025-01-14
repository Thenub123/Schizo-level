using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Menu : MonoBehaviour
{
    
    public GameObject[] menus;
    public int currentMenu;

    public GameObject[] backgrounds;
    public int currentBackground;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextMenu(int next) {
        IEnumerator menuTimer = MenuTimer(next);
        StartCoroutine(menuTimer);
    }

    private IEnumerator MenuTimer(int next)
    {
        yield return new WaitForSeconds(0.5f);
        menus[currentMenu].SetActive(false);
        currentMenu = next;
        menus[currentMenu].SetActive(true);
    }

    public void NextArea(int next) {
        IEnumerator areaTimer = AreaTimer(next);
        StartCoroutine(areaTimer);
    }

    private IEnumerator AreaTimer(int next)
    {
        yield return new WaitForSeconds(0.5f);
        backgrounds[currentBackground].SetActive(false);
        currentBackground = next;
        backgrounds[currentBackground].SetActive(true);
    }
}
