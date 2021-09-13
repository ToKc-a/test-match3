using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController instance;

    private int xSize, ySize;
    private List<Sprite> tileSprite = new List<Sprite>();
    private Tile[,] tileArray; //Массив в который передаются данные из класса Board (массив, который заполняется при создании доски)

    private Tile oldSelectedTile;
    private Vector2[] dirRay = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right }; //Получение информации о 4х тайлах относительно централдьного.

    public void SetValue(Tile[,] tileArray, int xSize, int ySize, List<Sprite> tileSprite)
    {
        this.tileArray = tileArray;
        this.xSize = xSize;
        this.ySize = ySize;
        this.tileSprite = tileSprite;
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D ray = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (ray != false)
            {
                CheckSelectTile(ray.collider.gameObject.GetComponent<Tile>());
            }
        }
    }

    private void SelectTile(Tile tile) //Метод выделения тайла.
    {
        tile.isSelected = true;
        tile.spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);
        oldSelectedTile = tile;
    }

    private void DeselectTile(Tile tile) //Метод снятия выделения тайла.
    {
        tile.isSelected = false;
        tile.spriteRenderer.color = new Color(1f, 1f, 1f);
        oldSelectedTile = null;
    }

    private void CheckSelectTile(Tile tile) //Метод принятия решения по выделению
    {
        if (tile.isEmpty)
        {
            return;
        }
        if (tile.isSelected)
        {
            DeselectTile(tile);
        }
        else
        {
            //Первое выделение тайла.
            if (!tile.isSelected && oldSelectedTile == null)
            {
                SelectTile(tile);
            }
            //Попытка выбрать другой тайл.
            else
            {
                //Если второй выбранный тайл - сосед предыдущего тайла.
                if (AdjacentTiles().Contains(tile))
                {
                    SwapToTiles(tile);
                    DeselectTile(oldSelectedTile);
                }
                //Забыть старый тайл, выделить новый.
                else
                {
                    DeselectTile(oldSelectedTile);
                    SelectTile(tile);
                }

            }
        }
    }

    private void SwapToTiles(Tile tile) //Метод перемещения тайлов
    {
        if (oldSelectedTile.spriteRenderer.sprite==tile.spriteRenderer.sprite)
        {
            return;
        }
        Sprite cashSprite = oldSelectedTile.spriteRenderer.sprite;
        oldSelectedTile.spriteRenderer.sprite = tile.spriteRenderer.sprite;
        tile.spriteRenderer.sprite = cashSprite;
    }

    private List<Tile> AdjacentTiles () //Метод возвращающий список тайлов, которые соседствуют с выбранным.
    {
        List<Tile> cashTiles = new List<Tile>(); //Список, принимающий тайлы.
        for (int i = 0; i < dirRay.Length; i++) //Цикл "стреляющий" по соседним тайлам, относительно выделенного.
        {
            RaycastHit2D hit = Physics2D.Raycast(oldSelectedTile.transform.position, dirRay[i]);
            if (hit.collider != null)
            {
                cashTiles.Add(hit.collider.gameObject.GetComponent<Tile>()); //Запись о "попадании" в список.
            }
        }
        return cashTiles; //Возврат списка.
    }
}
