using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager1 : MonoBehaviour
{
    public static UIManager1 Instance;

    [SerializeField]
    private GameObject wordPanel;

    [SerializeField]
    private GameObject imagePanel;


    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OpenWordPanel()
    {
        wordPanel.SetActive(true);
        imagePanel.SetActive(false);
    }

    public void OpenImagePanel()
    {
        imagePanel.SetActive(true);
        wordPanel.SetActive(false);
    }
}

