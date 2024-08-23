using UnityEngine;
using TMPro;
using System.Collections;

public class DisplayText : MonoBehaviour
{
    public TextMeshProUGUI exerciseText; // 确认这是TextMeshProUGUI类型字段
    public TextMeshProUGUI incorrectText;

    private string introduction ="<b> Correct Steps:</b>\n" +
        "1. <b>Starting Position:</b>\n" +
        "   - Stand with your feet shoulder-width apart and your body in a natural, upright position.\n" +
        "   - Let your arms hang naturally and relax your shoulders.\n\n" +
        "2. <b>Begin the Curl:</b>\n" +
        "   - Stabilize your shoulder and slowly bend your elbow, raising your forearm.\n" +
        "   - Keep your upper arm stationary and only move your forearm, with your palm facing towards you.\n\n" +
        "3. <b>Peak Position:</b>\n" +
        "   - Hold the curl at the highest point for 1-2 seconds.\n" +
        "   - Ensure that your elbow does not exceed a 90-degree angle to prevent overextension.\n\n" +
        "4. <b>Return to Start:</b>\n" +
        "   - Slowly lower your arm back to the starting position.\n" +
        "   - Maintain control over the movement to avoid dropping your arm abruptly.\n\n" +
        "5. <b>Repeat the Exercise:</b>\n" +
        "   - Repeat the above movement 10-15 times based on your fitness level.\n" +
        "   - Perform 2-3 sets, resting 30 seconds to 1 minute between each set.\n\n";
    private string incorrect = "<b>Incorrect Postures and Their Impacts:</b>\n" +
        "1. <b>Unstable Shoulders:</b>\n" +
        "   - If your shoulders lift along with your arms during the curl, it can cause shoulder muscle strain and fatigue.\n\n" +
        "2. <b>Overly Bent Wrist:</b>\n" +
        "   - Excessive bending of the wrist during the exercise may lead to wrist pain or injury.\n\n" +
        "3. <b>Overextended Elbows:</b>\n" +
        "   - If the elbow exceeds a 90-degree angle at the peak of the curl, it can result in elbow joint damage and inflammation.\n\n" +
        "4. <b>Excessive Speed:</b>\n" +
        "   - Performing the exercise too quickly can lead to muscle strain and reduce the effectiveness of the workout.\n\n" +
        "5. <b>Asymmetrical Posture:</b>\n" +
        "   - Inconsistent movement between both arms can cause muscle imbalance and affect overall body posture.\n\n";

    void Start()
    {
        if (exerciseText != null)
        {
            StartCoroutine(DisplayLines());
        }
        if (incorrectText != null)
        {
            StartCoroutine(DisplayLines1());
        }
        else
        {
            Debug.LogError("exerciseText is not assigned in the Inspector");
        }
    }

    IEnumerator DisplayLines()
    {
        string[] lines = introduction.Split('\n');
        exerciseText.text = "";

        foreach (string line in lines)
        {
            exerciseText.text += line + "\n";
            yield return new WaitForSeconds(0.5f); // 每行显示的间隔时间
        }
    }

    IEnumerator DisplayLines1()
    {
        string[] lines = incorrect.Split('\n');
        incorrectText.text = "";

        foreach (string line in lines)
        {
            incorrectText.text += line + "\n";
            yield return new WaitForSeconds(0.5f); // 每行显示的间隔时间
        }
    }
}
