using System;
using System.Collections.Generic;
using UnityEngine;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;

            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);

            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {
            //напиши реализацию не меняя сигнатуру функции
            //throw new NotImplementedException();
            Dictionary<char, int> letterCountsCommon = new Dictionary<char, int>();//Словарь для всех букв и их количества
            Dictionary<char, int> letterCountsLocal = new Dictionary<char, int>();//Словарь для букв текущего разбираемого слова

            // Перебор слов и учет букв
            foreach (string word in words)
            {
                letterCountsLocal.Clear();//Очищаем перед работой
                
                foreach (char letter in word)
                {
                    if (letterCountsLocal.ContainsKey(letter)) //Разбираем каждую букву в слове и считаем их количество
                    {
                        letterCountsLocal[letter]++;
                    }
                    else
                    {
                        letterCountsLocal[letter] = 1;
                    }
                }

                foreach (var elem in letterCountsLocal)//Перебираем подсчитанные буквы в слове, для сравнения с уже имеющимися
                {
                    if (letterCountsCommon.ContainsKey(elem.Key)) 
                    {
                        if (letterCountsCommon[elem.Key] < elem.Value) //Если в новом слове буква повторяется больше раз чем в общем списке, то заменяем значение
                            letterCountsCommon[elem.Key] = elem.Value;
                    }
                    else
                    {
                        letterCountsCommon.Add(elem.Key, elem.Value);//Если буквы ещё нет в общем списке, то вносим её
                    }
                }
            }

            //Создание списка букв на вывод
            List<char> letterList = new List<char>();
            foreach (var kvp in letterCountsCommon)
            {
                //Заносим букву столько раз, сколько раз она повторяется в словаре
                for (int i = 0; i<kvp.Value; i++)
                    letterList.Add(kvp.Key);
            }
            //Перемешиваем список
            Shuffle(letterList);
            return letterList;
        }

        public void Shuffle<T>(IList<T> list) //Простой метод перемешивания списков
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}