using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CHECKER : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TextObject;


    // Update is called once per frame
    void Update()
    {
        TextObject.text = TurnManager.Instance.currentState.ToString();
        
    }
}
