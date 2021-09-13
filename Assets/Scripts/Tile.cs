using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; //Меняет изображение у тайла.
    public bool isSelected; //Логическая переменная определяющая выбран тайл или нет.

    //Логическое свойство определяющее наличие изображение у тайла.
    public bool isEmpty
    {
        get
        {
            return spriteRenderer.sprite == null ? true : false; //Если у спрайта данного тайла нет изображения(ссылка на изображение пустая),
                                                                 //то мы возвращаем "true", иначе - "false".
                                                                 //При выстраивании 3 одинаковых тайлов в ряд у них не будет спрайтов.
        }
    }    
}
