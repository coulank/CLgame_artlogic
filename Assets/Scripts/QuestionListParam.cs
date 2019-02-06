using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName ="Data/CreateQuestionList")]
public class QuestionListParam : ScriptableObject
{
    public QuestionParam[] QuestionList;
}
