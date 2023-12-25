using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Advent2023.UI
{
    public class ResultUi : MonoBehaviour
    {
        [SerializeField] private TMP_Text _elapsedTimeText;
        [SerializeField] private TMP_Text _resultText;
        private readonly string _elapsedTimeString = "Elapsed time ";
        private readonly string _resultString = "Result ";

        public static ResultUi Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void ShowResult(long elapsedTime, int result)
        {
            _elapsedTimeText.SetText(_elapsedTimeString + elapsedTime);
            _resultText.SetText(_resultString + result);
        }
    }
}