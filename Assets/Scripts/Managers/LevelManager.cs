using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class LevelManager : MonoBehaviour
    {
        public LevelData LevelData;

        public LevelDefinition CurrentLevel => _currentLevel;
        private LevelDefinition _currentLevel;
        private int _currentLevelIndex;

        public int CurrentLevelNumber => _currentLevelIndex + 1;

        public void NextLevel() => LoadLevel(++_currentLevelIndex);

        public void RestartLevel() => LoadLevel(_currentLevelIndex);

        public void FirstLevel() => LoadLevel(0);        

        public void LoadLevel(int level)
        {
            _currentLevelIndex = level;
            _currentLevel = LevelData.Levels[level];
            Game.SetState(GameState.LevelStarted);
        }


    }
}

