using System;
using UnityEngine;
using UnityEngine.UI;

public class TeamScore : MonoBehaviour
{
    public Text teamText;
    public Text score;
    public Image image;
    public TeamController teamController;

    private void Update()
    {
        teamText.text = $"{Enum.GetNames(typeof(Team))[(int)teamController.team]} Team"; 
        score.text = $"{teamController.score} Points";
        image.color = GameManager.Instance.GetColorFromTeam(teamController.team);
    }
}
