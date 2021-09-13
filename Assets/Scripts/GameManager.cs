using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //Сериализация класса, для отображения данных для настройки.
public class BoardSetting //Публичный класс, через который настраивается игровая доска.
{
    public int xSize, ySize; //Размеры по X и Y (Количество тайлов на игровой доске).
    public Tile tileGO; //Поле для создаваемого префаба.
    public List<Sprite> tileSprite; //Список всех спрайтов для тайла.
}

public class GameManager : MonoBehaviour
{
    [Header ("Параметры игровой доски")]
    public BoardSetting boardSetting;

    void Start()
    {
        BoardController.instance.SetValue(Board.instance.SetValue(boardSetting.xSize, boardSetting.ySize, boardSetting.tileGO, boardSetting.tileSprite),
            boardSetting.xSize, boardSetting.ySize, boardSetting.tileSprite); //Передача данных для Доски и возврат массива.
    }

    void Update()
    {
        
    }
}
