using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rubik : MonoBehaviour, ICreature
{
    public NeuroNet net { get; set; }
    private int[,,] cube = new int[6, 3, 3];
    private GameObject[,,] planes = new GameObject[6, 3, 3];
    private int countInput, countOutput, countHidenLayer, countHidenNeuron, countTurn;
    private bool complete;
    private float prevMax;
    // Start is called before the first frame update
    void Start()
    {    
        Respawn();
        complete = false;     
        countInput = 54;
        countOutput = 18;
        countHidenLayer = 15;
        countHidenNeuron = 54;
        countTurn = 0;
        prevMax = 0;
        for (int plane = 0; plane < cube.GetLength(0); plane++)
        {
            for (int row = 0; row < cube.GetLength(1); row++)
            {
                for (int column = 0; column < cube.GetLength(2); column++)
                {
                    planes[plane, row, column] = gameObject.transform.GetChild(plane).transform.GetChild(row).transform.GetChild(column).gameObject;
                }
            }
        }
        CreateNeuro();
    }

    public GameObject Respawn()
    {
        countTurn = 0;
        for (int plane = 0; plane < cube.GetLength(0); plane++)
        {
            for (int row = 0; row < cube.GetLength(1); row++)
            {
                for (int column = 0; column < cube.GetLength(2); column++)
                {
                    cube[plane, row, column] = plane;
                }
            }
        }
        Left();
        Right2();
        Down();
        UnUp();
        Left2();
        Front2();
        UnBack();
        Up();
        Left();
        Down2();
        UnFront();
        return gameObject;
    }

    public void CreateNeuro()
    {

        net = new NeuroNet(countInput, countOutput, countHidenLayer, countHidenNeuron);
    }

    public void Complete()
    {
        bool comp = true;
        for (int plane = 0; plane < cube.GetLength(0); plane++)
        {
            if (comp)
            {
                for (int row = 0; row < cube.GetLength(1); row++)
                {
                    if (comp)
                    {
                        for (int column = 0; column < cube.GetLength(2); column++)
                        {
                            if (cube[plane, row, column] != plane)
                            {
                                comp = false;
                                break;
                            }
                        }
                    }
                    else break;                  
                }
            }
            else break;
        }
        complete = comp;
    }

    public float Score()
    {
        float score = 0f;
        for (int plane = 0; plane < cube.GetLength(0); plane++)
        {
            float localScore = 0;
            for (int row = 0; row < cube.GetLength(1); row++)
            {
                for (int column = 0; column < cube.GetLength(2); column++)
                {
                    if (cube[plane, row, column] == plane)
                    {
                        localScore++;
                    }
                }
            }
            localScore *= localScore;
            score += localScore;
        }
        if (complete)
        {
            score += 100000;
        }
        return score;
    }

    void Left()
    {
        int[,,] newCube = (int[,,])cube.Clone();
        for (int row = 0; row < cube.GetLength(1); row++)
        {
            newCube[0, row, 0] = cube[3, row, 2];
        }
        for (int plane = 1; plane < 3; plane++)
        {
            for (int row = 0; row < cube.GetLength(1); row++)
            {
                newCube[plane, row, 0] = cube[plane - 1, row, 0];
            }
        }
        for (int row = 0; row < cube.GetLength(1); row++)
        {
            newCube[3, row, 2] = cube[2, row, 0];
        }
        cube = newCube;
    }
    
    void Left2()
    {
        for (int i = 0; i < 2; i++)
        {
            Left();
        }
    }

    void UnLeft()
    {
        for (int i = 0; i < 3; i++)
        {
            Left();
        }    
    }

    void Right()
    {
        int[,,] newCube = (int[,,])cube.Clone();
        for (int row = 0; row < cube.GetLength(1); row++)
        {
            newCube[0, row, 2] = cube[3, row, 0];
        }
        for (int plane = 1; plane < 3; plane++)
        {
            for (int row = 0; row < cube.GetLength(1); row++)
            {
                newCube[plane, row, 2] = cube[plane - 1, row, 2];
            }
        }
        for (int row = 0; row < cube.GetLength(1); row++)
        {
            newCube[3, row, 0] = cube[2, row, 2];
        }
        cube = newCube;
    }

    void Right2()
    {
        for (int i = 0; i < 2; i++)
        {
            Right();
        }
    }

    void UnRight()
    {
        for (int i = 0; i < 3; i++)
        {
            Right();
        }
    }

    void Up()
    {
        int[,,] newCube = (int[,,])cube.Clone();
        for (int column = 0; column < cube.GetLength(2); column++)
        {
            newCube[3, 0, column] = cube[5, 0, column];
            newCube[5, 0, column] = cube[1, 0, column];
            newCube[1, 0, column] = cube[4, 0, column];
            newCube[4, 0, column] = cube[3, 0, column];
        }
        cube = newCube;
    }

    void Up2()
    {
        for (int i = 0; i < 2; i++)
        {
            Up();
        }
    }
    void UnUp()
    {
        for (int i = 0; i < 3; i++)
        {
            Up();
        }
    }

    void Down()
    {
        int[,,] newCube = (int[,,])cube.Clone();
        for (int column = 0; column < cube.GetLength(2); column++)
        {
            newCube[3, 2, column] = cube[5, 2, column];
            newCube[5, 2, column] = cube[1, 2, column];
            newCube[1, 2, column] = cube[4, 2, column];
            newCube[4, 2, column] = cube[3, 2, column];
        }
        cube = newCube;
    }

    void Down2()
    {
        for (int i = 0; i < 2; i++)
        {
            Down();
        }
    }

    void UnDown()
    {
        for (int i = 0; i < 3; i++)
        {
            Down();
        }
    }

    void Front()
    {
        int[,,] newCube = (int[,,])cube.Clone();
        for (int num = 0; num < cube.GetLength(2); num++)
        {
            newCube[0, 2, num] = cube[4, num, 0];
            newCube[5, num, 2] = cube[0, 2, num];
            newCube[2, 0, num] = cube[5, num, 2];
            newCube[4, num, 0] = cube[2, 0, num];
        }
        cube = newCube;
    }

    void Front2()
    {
        for (int i = 0; i < 2; i++)
        {
            Front();
        }
    }

    void UnFront()
    {
        for (int i = 0; i < 3; i++)
        {
            Front();
        }
    }

    void Back()
    {
        int[,,] newCube = (int[,,])cube.Clone();
        for (int num = 0; num < cube.GetLength(2); num++)
        {
            newCube[0, 0, num] = cube[4, num, 2];
            newCube[5, num, 0] = cube[0, 0, num];
            newCube[2, 2, num] = cube[5, num, 0];
            newCube[4, num, 2] = cube[2, 2, num];
        }
        cube = newCube;
    }

    void Back2()
    {
        for (int i = 0; i < 2; i++)
        {
            Back();
        }
    }

    void UnBack()
    {
        for (int i = 0; i < 3; i++)
        {
            Back();
        }
    }

    void Action(int index)
    {
        switch (index)
        {
            case 0:
                Left();
                break;
            case 1:
                UnLeft();
                break;
            case 2:
                Right();
                break;
            case 3:
                UnRight();
                break;
            case 4:
                Up();
                break;
            case 5:
                UnUp();
                break;
            case 6:
                Down();
                break;
            case 7:
                UnDown();
                break;
            case 8:
                Front();
                break;
            case 9:
                UnFront();
                break;
            case 10:
                Back();
                break;
            case 11:
                UnBack();
                break;
            case 12:
                Left2();
                break;
            case 13:
                Right2();
                break;
            case 14:
                Up2();
                break;
            case 15:
                Down2();
                break;
            case 16:
                Front2();
                break;
            case 17:
                Back2();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!complete)
        {
            float[] input = new float[countInput];
            for (int plane = 0; plane < cube.GetLength(0); plane++)
            {
                for (int row = 0; row < cube.GetLength(1); row++)
                {
                    for (int column = 0; column < cube.GetLength(2); column++)
                    {
                        input[plane * 6 + row * 3 + column] = cube[plane, row, column]/6f;
                    }
                }
            }
            float[] output = new float[countOutput];
            output = net.GetOutput(input);
            float max = output[0];
            int indexMax = 0;
            for (int i = 1; i < output.Length; i++)
            {
                if (output[i] > max && prevMax!=i)
                {
                    max = output[i];
                    indexMax = i;
                }
            }
            prevMax = indexMax;
            Action(indexMax);
            countTurn++;
            Complete();
        }
        for (int plane = 0; plane < cube.GetLength(0); plane++)
        {
            for (int row = 0; row < cube.GetLength(1); row++)
            {
                for (int column = 0; column < cube.GetLength(2); column++)
                {
                    Material mat = planes[plane, row, column].GetComponent<MeshRenderer>().material;
                    switch (cube[plane, row, column])
                    {
                        case 0:
                            mat.color = Color.red;
                            break;
                        case 1:
                            mat.color = Color.green;
                            break;
                        case 2:
                            mat.color = Color.magenta;
                            break;
                        case 3:
                            mat.color = Color.blue;
                            break;
                        case 4:
                            mat.color = Color.yellow;
                            break;
                        case 5:
                            mat.color = Color.white;
                            break;
                    }

                }
            }
        }
    }
}
