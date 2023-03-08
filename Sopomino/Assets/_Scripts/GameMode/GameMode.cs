using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Serialization;


namespace GameMode
{

    [CreateAssetMenu(fileName = "New GameMode", menuName = "GameMode", order = 0)]
    public class GameMode : ScriptableObject
    {
        [Header("Infos")]
        public string gameModeName;
        public string description;
        public int limit;
        public LimitType limitType;

        [Space(10)]

        [TextArea]
        public string endGameTemplateMessage;
        public DataName[] dataToDisplay;

        public enum DataName
        {
            Lines,
            Score,
            Time
        }

        public enum LimitType
        {
            Lines,
            Time,
            Score
        }

        public string GetEndGameMessage()
        {
            var endGameMessage = endGameTemplateMessage;
            var index = 1;
            foreach (var data in dataToDisplay)
            {
                var value = GetDataValue(data);
                var checkIndex = "$" + index.ToString();

                endGameMessage = endGameMessage.Replace(checkIndex, value);
                index++;
            }

            return endGameMessage;

            string GetDataValue(DataName dataName)
            {
                switch (dataName)
                {
                    case DataName.Lines:
                        return ScoreManager.Instance.Lines.ToString();
                    case DataName.Score:
                        return ScoreManager.Instance.Score.ToString();
                    case DataName.Time:
                        return StopWatchManager.Instance.GetTimeToString();
                    default:
                        Debug.LogError($"{dataName} not found");
                        return "";
                }
            }
        }
    }
}
