using System;
using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    {
        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {   //ChessUnitType(Енамчик)

            List<Vector2Int> FromTo = new List<Vector2Int>();

            int[] dx;
            int[] dy; //Эти два массива будут представлять собой возможное смещение фигуры(На сколько по оси её можно сдвинуть)
            switch (unit)
            {
                //0 - Пешка
                case ChessUnitType.Pon:
                    dx = new int[]{ 0, 0 };
                    dy = new int[] { 1, -1 }; //Пешка может двигаться как вперёд так и назад
                    return BFSFinder(from , to, grid, dx, dy);
                    break;
                //1 - Король
                case ChessUnitType.King:
                    dx = new int[] { 0, 0, -1, -1, -1, 1, 1, 1 };
                     dy = new int[] { 1, -1, 0, -1, 1, 0, -1, 1 };
                    return BFSFinder(from, to, grid, dx, dy);
                    break;
                //2 - Королева
                case ChessUnitType.Queen:
                    dx = new int[]{ -1, 1, 0, 0, -1, 1, -1, 1 };
                    dy = new int[]{ 0, 0, -1, 1, -1, -1, 1, 1 };
                    return AStarFinder(from, to, grid, dx, dy);
                    break;
                //3 - Ладья
                case ChessUnitType.Rook:
                    dx = new int[] { -1, 1, 0, 0 };
                     dy = new int[] { 0, 0, -1, 1 };
                    return AStarFinder(from, to, grid, dx, dy);
                    break;
                //4 - Конь
                case ChessUnitType.Knight:
                    dx = new int[] { 1, 2, 2, 1, -1, -2, -2, -1 };
                    dy = new int[] { 2, 1, -1, -2, -2, -1, 1, 2 };
                    return BFSFinder(from, to, grid,  dx, dy);
                    break;
                //5 - Слон
                case ChessUnitType.Bishop:
                    dx = new int[] { -1, 1, -1 , 1 };
                    dy = new int[] { -1, -1, 1, 1 };
                    return AStarFinder(from, to, grid, dx, dy);
                    break;
            } 
            return null;
            //напиши реализацию не меняя сигнатуру функции
            //throw new NotImplementedException();
        }

        //Проверка имеется ли клетка на доске и не занята ли она
        private bool IsValidCell(int x, int y, ChessGrid grid)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8 && grid.Get(new Vector2Int(x, y)) == null; 
        }

        // Алгоритм BFS по поиску пути(подходит для фигур, которые перемещаются на конкретные клетки игнорируя остальные фигуры).
        private List<Vector2Int> BFSFinder(Vector2Int from, Vector2Int to, ChessGrid grid, int[] dx, int[] dy)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>(); //Составляем очередь позиций на проверку
            queue.Enqueue(new Vector2Int(from.x, from.y));

            int[,] distance = new int[8, 8];
            Vector2Int[,] parent = new Vector2Int[8, 8]; //Можно было бы использовать Dictionary, но с ними возникали проблемы,
                                                         //потому используется двумерный массив, чтобы каждой клетке соответствовала другая клетка, с которой на неё можно перейти
            
            //По умолчанию массив "родителей" заполняется нулями, нужно его перезаполнить координатами которых не может быть на доске.
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    parent[i, j] = new Vector2Int(-1, -1);
                    distance[i, j] = -1;
                }                
            

            distance[from.x, from.y] = 0;
            
            //Проверяем возможные ходы из каждой клетки, постепенно расширяя область исследования
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                int x = current.x;
                int y = current.y;

                if (x == to.x && y == to.y)
                {
                    break; // Достигнута конечная точка
                }

                //Проверяем все возможные точки, которых фигура может достигнуть
                for (int i = 0; i < dx.Length; i++)
                {
                    int newX = x + dx[i];
                    int newY = y + dy[i];

                    if (IsValidCell(newX, newY, grid) && distance[newX, newY] == -1)
                    {
                        distance[newX, newY] = distance[x, y] + 1;
                        parent[newX, newY] = current;
                        queue.Enqueue(new Vector2Int(newX, newY));
                    }
                }
            }

            // Построение пути от конечной клетки к начальной
            List<Vector2Int> path = new List<Vector2Int>();
            int curX = to.x;
            int curY = to.y;

            //До тех пор пока мы не достигли начала пути, продолжаем увеличивать путь
            while (curX != from.x || curY != from.y)
            {
                //Если не находится клетка, из которой можно дойти до текущей, значит путь построить невозможно
                if (parent[curX, curY] == new Vector2Int(-1, -1))
                {
                    throw new ArgumentNullException("Check Comments(ChessGridNavigator (126 row)!");

                    //return null; // Тут идёт проверка возможно ли по итогу добраться до клетки и если нет, то как и сказано в задании возвращается null,
                                 // однако другие методы(в которые мне влезать нельзя) вызывают бесконечный цикл, поэтому оставляю вызов исключения.
                }
                path.Add(new Vector2Int(curX, curY));
                var previous = parent[curX, curY];
                curX = previous.x;
                curY = previous.y;
            }

            path.Add(new Vector2Int(from.x, from.y));
            path.Reverse(); // Переворачиваем путь, чтобы начало было в начальной клетке

            return path;
        }


        //Создает список позиций, на которые фигура может встать
        private List<Vector2Int> GeneratePossibleMoves(int[] dx, int[] dy, Vector2Int position, ChessGrid grid)
        {
            List<Vector2Int> moves = new List<Vector2Int>();

            for (int i = 0; i < dx.Length; i++)
            {
                for (int step = 1; step < 8; step++) //На поле 8x8 фигура в любом направлении сможет пройти от 1 до 7 клеток
                {
                    int x = position.x + step * dx[i];
                    int y = position.y + step * dy[i];
                    
                    if (x >= 0 && x < 8 && y >= 0 && y < 8)
                        if (grid.Get(new Vector2Int(x, y)) != null) break; //В случае если при постройке пути натыкаемся на фигуру, дальнейшее построение пути не имеет смысла, так что сразу пропускаем клетки
                        else moves.Add(new Vector2Int(x, y));
                }
             }
        

            return moves;
        }

        // Алгоритм A* по поиску пути(подходит для фигур, которые могут перемещаться на различное количество клеток, не игнорируют фигуры на пути).
        public List<Vector2Int> AStarFinder(Vector2Int from, Vector2Int to, ChessGrid grid, int[] dx, int[] dy)
        {
            List<Vector2Int> path = new List<Vector2Int>(); 
            Queue<Vector2Int> queue = new Queue<Vector2Int>(); //Очередь аналогичная BFS

            Vector2Int[,] parent = new Vector2Int[8,8];//Точно также запоминаем родительские клетки

            //По умолчанию массив "родителей" заполняется нулями, нужно его перезаполнить координатами которых не может быть на доске.
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    parent[i, j] = new Vector2Int(-1, -1);
                }

            Vector2Int start = new Vector2Int(from.x, from.y); //Запоминаем старовую позицию
            Vector2Int goal = new Vector2Int(to.x, to.y); //Запоминаем конечную позицию

            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue(); //Достаем верхнего участника очереди

                if (current.Equals(goal)) //В случае если уже достигли цели
                {
                    // Построение пути, начиная с конечной позиции и двигаясь к начальной.
                    while (parent[current.x, current.y] != new Vector2Int(-1,-1) && current != start)
                    {
                        path.Add(current);
                        current = parent[current.x, current.y];
                    }
                    path.Add(start);
                    path.Reverse();
                    return path;
                }

                // Генерация возможных ходов фигуры
                List<Vector2Int> moves = GeneratePossibleMoves(dx,dy,current, grid);

                foreach (var move in moves)
                {
                    if (parent[move.x, move.y] == new Vector2Int(-1,-1)) //Если для текущей клетки ещё нет родительской, тогда назначаем ей родителя
                    {
                        queue.Enqueue(move);
                        parent[move.x, move.y] = current;
                    }
                }
            }

            // В конце если путь не найдён, возвращается нулл,
            // однако другие методы(в которые мне влезать нельзя) вызывают бесконечный цикл, поэтому оставляю вызов исключения.(с ним юнити закрывается)
            throw new ArgumentNullException("Check Comments(ChessGridNavigator (212 row)!");

            
            //return null; // Если путь не найден
        }
    }
}