using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    public GameObject[] guns;
    public GameObject[] gunsDisplay;
    private Gun scriptGun;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            // Realiza acciones con cada objeto, por ejemplo, activarlos
            guns[i].SetActive(false);
            gunsDisplay[i].SetActive(false);
            //Gun scriptGunAux = guns[i].GetComponent<Gun>();
            //scriptGunAux.DeselectGun();
        }
        guns[0].SetActive(true);
        gunsDisplay[0].SetActive(true);
        scriptGun = guns[0].GetComponent<Gun>();
        //scriptGun.available = true;
        scriptGun.setAvailable();
        //scriptGun.SelectGun();

    }

    public bool selectGun(int id, bool d)
    {
        if (guns[id].GetComponent<Gun>().available)
        {
            if (id < guns.Length)
            {
                for (int i = 0; i < guns.Length; i++)
                {
                    // Realiza acciones con cada objeto, por ejemplo, activarlos
                    if (i != id)
                    {
                        guns[i].SetActive(false);
                        gunsDisplay[i].SetActive(false);
                        //Gun scriptGunAux = guns[id].GetComponent<Gun>();
                        //scriptGunAux.DeselectGun();
                    }
                }
                guns[id].SetActive(true);
                gunsDisplay[id].SetActive(true);
                scriptGun = guns[id].GetComponent<Gun>();
                scriptGun.setStartDirection(d);
                //scriptGun.SelectGun();
                return true;
            }
        }
        return false;
    }

    public void UnlockGun(int id)
    {
        if (id < guns.Length) guns[id].GetComponent<Gun>().setAvailable();
    }

    public void addBullets(int id, int num)
    {
        if (id < guns.Length) guns[id].GetComponent<Gun>().addBullets(num);
    }
}
