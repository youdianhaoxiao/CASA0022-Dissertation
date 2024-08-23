using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private GameObject loginPanel;

    [SerializeField]
    private GameObject registrationPanel;

    [SerializeField]
    private GameObject userdataPanel;

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        registrationPanel.SetActive(false);
        userdataPanel.SetActive(false);
    }

    public void OpenRegistrationPanel()
    {
        registrationPanel.SetActive(true);
        loginPanel.SetActive(false);
        userdataPanel.SetActive(false);
    }

    public void OpenUserdataPanel()
    {
        userdataPanel.SetActive(true);
        registrationPanel.SetActive(false);
        loginPanel.SetActive(false);
    }
}
