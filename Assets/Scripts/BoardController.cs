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
    private Vector2[] dirRay = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right }; //Получение информации о 4х тайлах относительно центрального.

    private bool isFindMatch = false; //Логическое поле, отвечающее за совпадения.
    private bool isShift = false; //Логическое поле, отвечающее за начало и конец смещения тайла.
    private bool isSearchEmptyTile = false; //Логическое поле, разрешает или запрещает работу метода поиска пустого тайла.

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
    
    void Update()
    {
        if (isSearchEmptyTile)
        {
            SearchEmptyTile();
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D ray = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (ray != false)
            {
                CheckSelectTile(ray.collider.gameObject.GetComponent<Tile>());
            }
        }
    }

    #region(Выделить тайл, Снять выделение, Управление выделением)
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
        if (tile.isEmpty || isShift)
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
                    FindAllMatch(tile);
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
    #endregion

    #region(Поиск совпадения, Удаление спрайтов, Поиск всех совпадений)
    private List <Tile> FindMatch (Tile tile, Vector2 dir) //Метод, отвечающий за поиск совпадений по горизонтали/вертикали
                                                           //и возвращение списка тайлов в случае обнаружения. Имеет 2 входящих параметра:
                                                           //Тайл с которого начинается поиск и двухмерный вектор, отвечающий за направление луча.
    {
        List<Tile> cashFindTiles = new List<Tile>(); //Список, в который помещаются тайлы, при обнаружении совпадения.
        RaycastHit2D hit = Physics2D.Raycast(tile.transform.position, dir); //Переменная, содержащая информацию о тайлах в которые попадет луч.
                                                                            //Луч Raycast стреляет в указанном направлении из тайла, который перемещается.

        while (hit.collider != null && hit.collider.gameObject.GetComponent<Tile>().spriteRenderer.sprite == tile.spriteRenderer.sprite) //Цикл, ищущий совпадения
        {
            cashFindTiles.Add(hit.collider.gameObject.GetComponent<Tile>());
            hit = Physics2D.Raycast(hit.collider.gameObject.transform.position, dir);
        }
        return cashFindTiles;
    }

    private void DeleteSprite(Tile tile, Vector2[] dirArray) //Метод, отвечающий за удаление изображений у выбранных тайлов (Тайлы выстроенные в ряд).
                                                             //Имеет 2 входящих параметра: Тайл и массив направления.
    {
        List<Tile> cashFindSprite = new List<Tile>();
        for (int i = 0; i < dirArray.Length; i++)
        {
            cashFindSprite.AddRange(FindMatch(tile, dirArray[i]));
        }
        if (cashFindSprite.Count >= 2)
        {
            for (int i = 0; i < cashFindSprite.Count; i++)
            {
                cashFindSprite[i].spriteRenderer.sprite = null;
            }
            isFindMatch = true;
        }
    }

    private void FindAllMatch (Tile tile) //Метод, отвечающий за поиск всех совпадений.
    {
        if (tile.isEmpty)
        {
            return;
        }

        DeleteSprite(tile, new Vector2[2] { Vector2.up, Vector2.down });
        DeleteSprite(tile, new Vector2[2] { Vector2.left, Vector2.right });

        if (isFindMatch)
        {
            isFindMatch = false;
            tile.spriteRenderer.sprite = null;
            isSearchEmptyTile = true;
        }
    }
    #endregion

    #region(Смена 2х тайлов местами, Поиск соседних тайлов)
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
    #endregion

    #region(Поиск пустого тайла, Сдвиг тайла вниз, Установить новое изображение, Выбрать новое изображение)
    private void SearchEmptyTile()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = ySize-1; y > -1; y--)
            {
                if (tileArray[x,y].isEmpty)
                {
                    ShiftTileDown(x,y);
                }
            }
        }

        isSearchEmptyTile = false;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                FindAllMatch(tileArray[x, y]);
            }
        }
    }

    private void ShiftTileDown(int xPos, int yPos)
    {
        isShift = true;
        for (int y = yPos; y < ySize - 1; y++)
        {
            if (!tileArray[xPos, y+1].isEmpty)
            {
                Tile tile = tileArray[xPos, y];
                tile.spriteRenderer.sprite = tileArray[xPos, y + 1].spriteRenderer.sprite;
            }
        }

        tileArray[xPos, ySize - 1].spriteRenderer.sprite = tileSprite[Random.Range(0, tileSprite.Count)];
        isShift = false;
    }

    private void SetNewSprite(int xPos, List<SpriteRenderer> renderer)
    {
        for (int y = 0; y < renderer.Count - 1; y++)
        {
            renderer[y].sprite = renderer[y + 1].sprite;
            renderer[y + 1].sprite = GetNewSprite(xPos, ySize - 1);
        }
    }

    private Sprite GetNewSprite(int xPos, int yPos)
    {
        List<Sprite> cashSprite = new List<Sprite>();
        cashSprite.AddRange(tileSprite);

        if (xPos > 0)
        {
            cashSprite.Remove(tileArray[xPos - 1, yPos].spriteRenderer.sprite);
        }

        if (xPos < xSize - 1)
        {
            cashSprite.Remove(tileArray[xPos + 1, yPos].spriteRenderer.sprite);
        }

        if (yPos > 0)
        {
            cashSprite.Remove(tileArray[xPos, yPos - 1].spriteRenderer.sprite);
        }

        return cashSprite[Random.Range(0, cashSprite.Count)];
    }
    #endregion
}