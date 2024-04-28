using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class MapGen : MonoBehaviour
{
    //creates a grid on which the map is generated

    public int[,] grid = new int[9, 9];

    public int nodes = 0;

    public GameObject EnemyPrefab;
    public GameObject BossPrefab;
    public GameObject SecretPrefab;
    public GameObject ShopPrefab;
    public GameObject ItemPrefab;
    public GameObject OriginPrefab;


    public void MapGenerate()
    {
	

        //the number of ends and nodes that currently exist

        int ends = 0;
        nodes = 0;

        //this is the current position of the map

        int curX = 4;
        int curY = 4;

        // indicates a direction

        int dir;

        grid[4, 4] = 1;

        while (ends < 2)
        {

            //marked when a path reaches its end
            bool isEnd = false;


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


                    curX = 6;

                    grid[curX, curY] = 4;
                    grid[curX - 1, curY] = 2;
                    nodes += 1;
                } else if (grid[4, 6] != 4)
                {
                    curY = 6;

                    grid[curX, curY] = 4;
                    grid[curX, curY - 1] = 2;
                    nodes += 1;
                } else
                {
                    ends += 1;
                    isEnd = true;
                }
            }

            //decides where to go; 1 = up, 2 = down, 3 = left, 4 = right

            while (!isEnd) {
                dir = Random.Range(1, 5);

                if (dir == 1)
                {
                    if (grid[curX, curY + 2] != 4)
                    {
                        curY += 2;
                        grid[curX, curY] = 4;
                        grid[curX, curY - 1] = 3;
                        nodes += 1;
                    }
                }
                else if (dir == 2)
                {
                    if (grid[curX, curY - 2] != 4)
                    {
                        curY -= 2;
                        grid[curX, curY] = 4;
                        grid[curX, curY + 1] = 3;
                        nodes += 1;
                    }
                }
                else if (dir == 3)
                {
                    if (grid[curX - 2, curY] != 4)
                    {
                        curX -= 2;
                        grid[curX, curY] = 4;
                        grid[curX + 1, curY] = 3;
                        nodes += 1;
                    }
                }
                else if (dir == 4)
                {
                    if (grid[curX, curY - 2] != 4)
                    {
                        curX += 2;
                        grid[curX, curY] = 4;
                        grid[curX - 1, curY] = 3;
                        nodes += 1;
                    }
                }

                if (((curX == 0) || (curX == 9)) || ((curY == 0) || (curY == 9)))
                {
                    isEnd = true; 
                } 

            }
        }
        

    

    }

    public void PlaceNodes()
    {

        int secrets = 0;

        if (grid[0,4] == 4) {
            secrets += 1;
        }
        if (grid[8,4] == 4) {
            secrets += 1;
        }
        if (grid[4,0] == 4) {
            secrets += 1;
        }
        if (grid[4,8] == 4) {
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
        } else if (nodes > 6) {
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
            for (int r = 0; r < 9; i++)
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

    public void CreateButtons() {
        for (int i = 0; i < 9; i++)
        {
            for (int r = 0; r < 9; i++)
            {
                if ((grid[i, r] > 3) || (grid[i, r] == 1)){
                    PlaceButtons(i, r);
                }
            }
        }
    }

	void PlaceButtons(int x, int y) {

        Vector3 pos = transform.position;
        pos.x = 1000 + (150 * x);
        pos.y = 200 + (150 * y);
        pos.z = 0;
        Quaternion rotation = Quaternion.Euler(0, 0, 0);

        if (grid[x,y] == 1) {
		    GameObject button = Instantiate(OriginPrefab, pos, rotation);
		}
	    else if (grid[x,y] == 4) {
            GameObject button = Instantiate(EnemyPrefab, pos, rotation);
		}
	    else if (grid[x,y] == 5) {
            GameObject button = Instantiate(BossPrefab, pos, rotation);
		}
	    else if (grid[x,y] == 6) {
            GameObject button = Instantiate(ItemPrefab, pos, rotation);
		}
	    else if (grid[x,y] == 7) {
            GameObject button = Instantiate(ShopPrefab, pos, rotation);
		}
	    else if (grid[x,y] == 8) {
            GameObject button = Instantiate(SecretPrefab, pos, rotation);
		}	
	 }


}
