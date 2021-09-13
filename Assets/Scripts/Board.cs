using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance; //Ссылка глобального доступа к данным из GameManager.

    private int xSize, ySize;
    private Tile tileGO;
    private List<Sprite> tileSprite = new List<Sprite>();

    private void Awake()
    {
        instance = this;
    }

    public Tile[,] SetValue(int xSize, int ySize, Tile tileGO, List<Sprite> tileSprite) //Метод для получения данных из GameManager.
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.tileGO = tileGO;
        this.tileSprite = tileSprite;

         return CreateBoard();
    }

    private Tile[,] CreateBoard() //Метод создающий игровую доску и возвращающий двумерный массив.
    {
        Tile[,] tileArray = new Tile[xSize, ySize]; //Двухмерный массив в который по X и Y помещаются созданные тайлы.
                                                    //Пробегает по всем тайлам в поисках пустых изображений.
       
        float xPos = transform.position.x; //Получение координат Board по X и Y для создания
        float yPos = transform.position.y; //тайлов именно там где находится данный объект.

        Vector2 tileSize = tileGO.spriteRenderer.bounds.size; //Вектор в который помещается размер тайла по X и Y.
                                                              //Необходим для смещения, чтобы тайлы не находили друг на друге.

        Sprite cashSprite = null; //Переменная содержащая изображение, помещенное в тайл.

        for (int x = 0; x < xSize; x++) //Циклы через которые создаются и заполняются тайлы.
        {
            for(int y = 0; y < ySize; y++)
            {
                Tile newTile = Instantiate(tileGO, transform.position, Quaternion.identity); //Переменная, содержащая ссылку на Tile созданного объекта.
                                                                                             //Тайл создается на месте объекта Board и без вращения.

                newTile.transform.position = new Vector3(xPos + (tileSize.x * x), yPos + (tileSize.y * y), 0); //Настройка позиции нового Тайла
                                                                                                               //в рамках игровой доски, с учетом смещения.
                newTile.transform.parent = transform; //Новые тайлы становятся дочерними объектами Board.

                tileArray[x, y] = newTile; //Помещение созданного тайла в массив.

                List<Sprite> tempSprite = new List<Sprite>(); //Список доступных изображений переданных через GameManager.
                                                              //Из него будут рандомно выбираться изображения для тайла и потом удаляться.
                                                              //Нужен для того, чтобы не было повторов.

                tempSprite.AddRange(tileSprite); //Помещаем в tempSprite список полученный из GameManager.

                //Исключение возможности появления 3 подряд одинаковых изображений по X и Y.
                tempSprite.Remove(cashSprite);
                if (x > 0)
                {
                    tempSprite.Remove(tileArray[x - 1, y].spriteRenderer.sprite);
                }

                newTile.spriteRenderer.sprite = tempSprite[Random.Range(0, tempSprite.Count)]; //Обращаемся к списку и помещаем в новый тайл случайный спрайт.
                cashSprite = newTile.spriteRenderer.sprite;
            }
        }

        return tileArray; //Возврат массива
    }
}
