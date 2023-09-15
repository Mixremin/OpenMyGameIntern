using UnityEngine;
using System;
using System.IO;
using System.Linq;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {

        //ОБРАБОТАТЬ
        //1.Повторяющиеся индексы ++
        //2.Длина слова и количество индексов отличается ++
        //3.Индекс слишком большой ++
        //4.Ни один из уровней не подходит (  throw new Exception() ) - Когда доходит до конца файла, исключение выбивается само, так что вмешательство не потребовалось
        static string prevLevel;
        static string currentLevel;

        public GridFillWords LoadModel(int index)
        {
            //напиши реализацию не меняя сигнатуру функции
            string[] levelData = null;
            string[] letterPos = null;

            string longWord = "";
            string[] longLettersPos = { };
            string[] newPosWord;

            bool levelFound = true;
            int tempIndex = index;
            int sizeXY = 0;
            
            //Определяем пригодность уровня
            do {
                //Очищаем переменные(нужно для следующих итераций) 
                levelFound = true;
                longWord = "";
                longLettersPos = new string[0];

                prevLevel = currentLevel; //Запоминаем прошлый уровень, чтобы они не повторялись
                string path = @"Assets\\App\\Resources\\Fillwords\\pack_0.txt";
                currentLevel = File.ReadLines(path).Skip(index - 1).First(); //Открываем документ с уровнями и ищем строку соответствующую номеру уровня
               
                //Если уровень повторился, то сразу переходим к следующему
                if (currentLevel.Equals(prevLevel) && levelFound)
                {
                    levelFound = false;
                    index++;
                    continue;
                }

                levelData = currentLevel.Split(" ");//Разделяем строку на номер слова и порядок букв
                //Приготовления для позиционирования букв
                for (int c = 0 ;c<levelData.Length;c++)
                {
                    if (c % 2 == 0)
                    {
                        string word = File.ReadLines(@"Assets\\App\\Resources\\Fillwords\\words_list.txt").Skip(int.Parse(levelData[c])).First();//Ищем в файле словоря нужное слово
                        longWord += word; //Все слова заносятся в одну строку без разделителей

                        letterPos = levelData[c + 1].Split(";");//Порядок букв также отдельно разбиваем
                        longLettersPos = longLettersPos.Concat(letterPos).ToArray(); //Все позиции для букв запоминаются в одном массиве

                        if (word.Length != letterPos.Length)
                        {
                            levelFound = false;
                            if (tempIndex == index) index++;
                            break; //Проверка не отличается ли количество букв в слове, от количества мест для букв
                        }
                    }
                }

                //Проверка индексов
                for (int c1 = 0; c1 < longLettersPos.Length; c1++)
                {
                    if (int.Parse(longLettersPos[c1]) > longLettersPos.Length)
                    {
                        levelFound = false;
                        if (tempIndex == index) index++;
                        break; 
                    }
                }

                //Заранее рассчитываем размерность квадрата
                sizeXY = (int) Math.Sqrt(longLettersPos.Length); 
                
                //Если буквы нельзя расположить в квадрате или имеются повторяющиеся индексы, то уровень будет пропущен
                if (sizeXY * sizeXY != longLettersPos.Length
                     || longLettersPos.Length != longLettersPos.Distinct().Count())
                {
                    levelFound = false;
                    if (tempIndex == index) index++;
                } 
            } while (!levelFound);

            newPosWord = new string[longLettersPos.Length]; //Заранее выделяем память для нового расположения букв
            for (int countPos = 0; countPos < longLettersPos.Length; countPos++) {
                newPosWord[int.Parse(longLettersPos[countPos])] = char.ToString(longWord[countPos]);  //Буквы расставляются в соответствие с индексами уровня в строку   
            }

            //Создание сетки
            Vector2Int size = new Vector2Int(sizeXY, sizeXY);
            GridFillWords mainGrid = new GridFillWords(size);

            //Сетка заполняется последовательно буквами
            int countFinPos = 0;
            for(int x = 0; x<size.x; x++)
                for (int y = 0; y < size.y; y++)
                {
                    mainGrid.Set(x, y, new CharGridModel(char.Parse(newPosWord[countFinPos])));
                    countFinPos++;
                }

            //for (int i=0; i < longLettersPos.Length; i++)
            //{
            //    mainGrid.Set(pointX, pointY, new CharGridModel(word[int.Parse(longLettersPos[i])]));//Ставим букву в клетку

            //    if (pointY < size.y-1)//Если прошлись ещё не по всем столбцам
            //    {
            //        pointY++;//Идём на следующую клетку
            //    } else//Если столбцы кончились
            //    {
            //        pointY=0;//Возвращаемся в начало
            //        pointX++;//Меняем строку(Здесь не нужно обнуление строк, как в случае со столбцами, так как цикл закончится в тот же момент когда кончатся клетки)
            //    }              
            //}       
            return mainGrid;
            //throw new NotImplementedException();
        }

        //private GridFillWords setForTest()
        //{
        //    Vector2Int test = new Vector2Int();
        //    test.x = 2;
        //    test.y = 2;
        //    GridFillWords testGrid = new GridFillWords(test);
        //    testGrid.Set(0, 0, new CharGridModel('л'));
        //    testGrid.Set(0, 1, new CharGridModel('е'));
        //    testGrid.Set(1, 0, new CharGridModel('т'));
        //    testGrid.Set(1, 1, new CharGridModel('о'));
        //    return testGrid;
        //}
    }
}