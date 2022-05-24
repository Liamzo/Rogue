using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatsUI : MonoBehaviour
{
    public Transform targetParent;

    public Slider enemyGrit;
    public Slider enemyGrace;
    public Text gritNum;
    public Text graceNum;
    public Text enemyName;

    public PlayerController player;
    public UnitStats enemyStats;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        player.TargetUnitChange += UpdateEnemy;

        enemyStats = null;

        enemyGrit = targetParent.Find("GritSlider").GetComponent<Slider>();
        enemyGrace = targetParent.Find("GraceSlider").GetComponent<Slider>();

        gritNum = enemyGrit.gameObject.transform.Find("GritNum").GetComponent<Text>();
        graceNum = enemyGrace.gameObject.transform.Find("GraceNum").GetComponent<Text>();

        enemyName = targetParent.Find("EnemyName").GetComponent<Text>();

        enemyGrit.maxValue = 0;
        enemyGrace.maxValue = 0;

        targetParent.gameObject.SetActive(false);
    }

    public void UpdateVisuals () {
        if (enemyStats == null) {
            return;
        }
        if (enemyStats.currentGrit <= 0) {
            enemyStats.OnUIChange -= UpdateVisuals;

            enemyStats = null;
            targetParent.gameObject.SetActive(false);
            return;
        }

        enemyGrit.maxValue = enemyStats.stats[(int)Stats.Grit].GetValue();
        enemyGrace.maxValue = enemyStats.stats[(int)Stats.Grace].GetValue();

        enemyGrit.value = enemyStats.currentGrit;

        // Situations were Grace can be above max
        enemyGrace.value = enemyStats.currentGrace;

        gritNum.text = enemyGrit.value + " / " + enemyGrit.maxValue;
        graceNum.text = enemyStats.currentGrace + " / " + enemyGrace.maxValue;

        enemyName.text = enemyStats.unitName;
    }

    public void UpdateEnemy () {
        if (player.targetUnit == null) {
            if (enemyStats != null) {
                enemyStats.OnUIChange -= UpdateVisuals;
            }

            enemyStats = null;
            targetParent.gameObject.SetActive(false);
            return;
        }

        enemyStats = player.targetUnit.unitStats;

        enemyStats.OnUIChange += UpdateVisuals;

        enemyGrit.maxValue = enemyStats.stats[(int)Stats.Grit].GetValue();
        enemyGrace.maxValue = enemyStats.stats[(int)Stats.Grace].GetValue();

        UpdateVisuals();

        targetParent.gameObject.SetActive(true);
    }
}
