using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetsController : MonoBehaviour
{
    public Canvas canvas;

    public List<TargetIndicator> targetIndicators = new List<TargetIndicator>();

    public Camera MainCamera;

    public GameObject TargetIndicatorPrefab;
    public GameObject ExitTargetIndicatorPrefab;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (targetIndicators.Count > 0)
        {
            for (int i = 0; i < targetIndicators.Count; i++)
            {
                targetIndicators[i].UpdateTargetIndicator();
            }
        }
    }

    public void AddTargetIndicator(GameObject target)
    {
        if (target.gameObject.tag.Equals("BigItem"))
        {
            TargetIndicator indicator = GameObject.Instantiate(TargetIndicatorPrefab, canvas.transform).GetComponent<TargetIndicator>();
            indicator.InitialiseTargetIndicator(target, MainCamera, canvas);
            targetIndicators.Add(indicator);
        }
        if (target.tag.Equals("Exit"))
        {
            TargetIndicator indicator = GameObject.Instantiate(ExitTargetIndicatorPrefab, canvas.transform).GetComponent<TargetIndicator>();
            indicator.InitialiseTargetIndicator(target, MainCamera, canvas);
            targetIndicators.Add(indicator);
        }
    }
}
