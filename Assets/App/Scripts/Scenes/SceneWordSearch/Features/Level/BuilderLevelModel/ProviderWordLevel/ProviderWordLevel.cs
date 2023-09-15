using System;
using System.IO;
using System.Linq;
using UnityEngine;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        public LevelInfo LoadLevelData(int levelIndex)
        {
            //напиши реализацию не меняя сигнатуру функции
            string path = @"Assets\\App\\Resources\\WordSearch\\Levels\\" + levelIndex + ".json"; //Прокладываем путь до файла
            string jsonText = File.ReadAllText(path); //Считываем содержимое файла
            LevelInfo words = new LevelInfo();
            words = JsonUtility.FromJson<LevelInfo>(jsonText); //Заполняем свежесозданный объект считанными данными
            return words;
            //throw new NotImplementedException();
        }
    }
}


