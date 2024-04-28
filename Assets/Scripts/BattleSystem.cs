using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{


    public int[,] grid = new int[9, 9];
    
    public int nodes = 0;

    public Transform ButtonParent;

    public GameObject EnemyPrefab;
    public GameObject BossPrefab;
    public GameObject SecretPrefab;
    public GameObject ShopPrefab;
    public GameObject ItemPrefab;
    public GameObject OriginPrefab;
    public GameObject HorPrefab;
    public GameObject VertPrefab;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;


    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public Text dialogueText;

    public BattleState state;



    void Start()
    {

        state = BattleState.START;
        MapGenerate();
        PlaceNodes();
        CreateButtons();
        SetupBattle();
        StartCoroutine(SetupBattle());

    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogueText.text = "Game Start!";
            
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        enemyHUD.SetHP(enemyUnit.currentHP);

        yield return new WaitForSeconds(2f);

        if(isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.unitName + " attacks!";

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

        if(isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        if(state == BattleState.WON)
        {
            if (state == BattleState.WON)
            {
                dialogueText.text = "You won the battle!";
            } else if (state == BattleState.LOST)
            {
                dialogueText.text = "You were defeated.";
            }
        }
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose your move";
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void MapGenerate()
    {


        //the number of ends and nodes that currently exist

        int ends = 0;
        nodes = 0;

        //this is the current position of the map

        int curX = 4;
        int curY = 4;

        // indicates a direction

        int dir = 0;

        grid[4, 4] = 1;

        while (ends < 2)
        {

            //marked when a path reaches its end
            int isEnd = 0;


            //0 = empty, 1 = origin, 2 = R/L pathway, 3 = U/D pathway, 4 = node
            if (ends == 0)
            {
                curX = 2;

                grid[curX, curY] = 4;
                grid[curX + 1, curY] = 2;
                nodes += 1;
            }

            if (ends == 1)
            {
                if (grid[6, 4] != 4)
                {

                    curY = 4;
                    curX = 6;

                    grid[curX, curY] = 4;
                    grid[curX - 1, curY] = 2;
                    nodes += 1;
                }
                else if (grid[4, 6] != 4)
                {
                    curY = 6;
                    curX = 4;

                    grid[curX, curY] = 4;
                    grid[curX, curY - 1] = 3;
                    nodes += 1;
                }
                else
                {
                    ends += 1;
                    isEnd = 1;
                }
            }

            //decides where to go; 1 = up, 2 = down, 3 = left, 4 = right

            while (isEnd == 0)
            {
                dir = Random.Range(1, 5);

                if (dir == 1)
                {
                    if (curY < 7)
                    {
                        if ((grid[curX, curY + 2] != 4) && (grid[curX, curY + 2] != 1))
                        {
                            curY += 2;
                            grid[curX, curY] = 4;
                            grid[curX, curY - 1] = 3;
                            nodes += 1;
                        }
                    }
                }
                else if (dir == 2)
                {
                    if (curY > 1)
                    {
                        if ((grid[curX, curY - 2] != 4) && (grid[curX, curY - 2] != 1))
                        {
                            curY -= 2;
                            grid[curX, curY] = 4;
                            grid[curX, curY + 1] = 3;
                            nodes += 1;
                        }
                    }
                }
                else if (dir == 3)
                {
                    if (curX > 1)
                    {
                        if ((grid[curX - 2, curY] != 4) && (grid[curX - 2, curY] != 1))
                        {
                            curX -= 2;
                            grid[curX, curY] = 4;
                            grid[curX + 1, curY] = 2;
                            nodes += 1;
                        }
                    }
                }
                else if (dir == 4)
                {
                    if (curX < 7)
                    {
                        if ((grid[curX + 2, curY] != 4) && (grid[curX + 2, curY] != 1))
                        {
                            curX += 2;
                            grid[curX, curY] = 4;
                            grid[curX - 1, curY] = 2;
                            nodes += 1;
                        }
                    }
                }

                if (((curX == 0) || (curX == 8)) || ((curY == 0) || (curY == 8)))
                {
                    isEnd = 1;
                    ends += 1;
                }

            }
        }




    }

    public void PlaceNodes()
    {

        int secrets = 0;

        if (grid[0, 4] == 4)
        {
            secrets += 1;
        }
        if (grid[8, 4] == 4)
        {
            secrets += 1;
        }
        if (grid[4, 0] == 4)
        {
            secrets += 1;
        }
        if (grid[4, 8] == 4)
        {
            secrets += 1;
        }

        if (secrets == 2)
        {
            greatestDist(8);
        }

        //Places boss, index 5 indicates the boss, 6 indicates an item, 7 indicates a shop, 8 indcates a secret. 9 is then placed in all remaining rooms as encounters.
        greatestDist(5);
        nodes -= 1;

        int coinflip;

        if ((nodes > 3) && (nodes < 7))
        {
            coinflip = Random.Range(0, 2);
            if (coinflip == 1)
            {
                greatestDist(6);
                nodes -= 1;
            }
            else
            {
                greatestDist(7);
                nodes -= 1;
            }
        }
        else if (nodes > 6)
        {
            greatestDist(6);
            nodes -= 1;
            greatestDist(7);
            nodes -= 1;
        }


    }

    void greatestDist(int type)
    {

        int furthestX = 4;
        int furthestY = 4;

        //finds the greatest distance from the origin, makes it a node type

        for (int i = 0; i < 9; i++)
        {
            for (int r = 0; r < 9; r++)
            {
                if (grid[i, r] == 4)
                {
                    if (Mathf.Sqrt((4 - i) * (4 - i) + (4 - r) * (4 - r)) > Mathf.Sqrt((4 - furthestX) * (4 - furthestX) + (4 - furthestY) * (4 - furthestY)))
                    {
                        furthestX = i;
                        furthestY = r;
                    }
                }
            }
        }

        grid[furthestX, furthestY] = type;

    }

    public void CreateButtons()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int r = 0; r < 9; r++)
            {
                if (grid[i, r] != 0)
                {
                    PlaceButtons(i, r);
                }
            }
        }
    }

    void PlaceButtons(int x, int y)
    {

        Vector3 pos = transform.position;
        pos.x = 1000 + (25 * x);
        pos.y = 100 + (25 * y);
        pos.z = 0;
        Quaternion rotation = Quaternion.Euler(0, 0, 0);

        if (grid[x, y] == 1)
        {
            GameObject button = Instantiate(OriginPrefab, pos, rotation, ButtonParent);
            button.GetComponent<Button>();
        }
        else if (grid[x, y] == 2)
        {
            GameObject button = Instantiate(VertPrefab, pos, rotation, ButtonParent);
            button.GetComponent<Button>();
        }
        else if (grid[x, y] == 3)
        {
            GameObject button = Instantiate(HorPrefab, pos, rotation, ButtonParent);
            button.GetComponent<Button>();
        }
        else if (grid[x, y] == 4)
        {
            GameObject button = Instantiate(EnemyPrefab, pos, rotation, ButtonParent);
            button.GetComponent<Button>();
        }
        else if (grid[x, y] == 5)
        {
            GameObject button = Instantiate(BossPrefab, pos, rotation, ButtonParent);
            button.GetComponent<Button>();
        }
        else if (grid[x, y] == 6)
        {
            GameObject button = Instantiate(ItemPrefab, pos, rotation, ButtonParent);
            button.GetComponent<Button>();
        }
        else if (grid[x, y] == 7)
        {
            GameObject button = Instantiate(ShopPrefab, pos, rotation, ButtonParent);
            button.GetComponent<Button>();
        }
        else if (grid[x, y] == 8)
        {
            GameObject button = Instantiate(SecretPrefab, pos, rotation, ButtonParent);
            button.GetComponent<Button>();
        }
    }




}
