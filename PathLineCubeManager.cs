using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PathLineCubeManager : MonoBehaviour
{
    static GameObject[] allCube;
    List<GameObject> goListCube;
    List<GameObject> goListRemainCube;
    LineRenderer PathLine;
    List<Transform> PosCube = new List<Transform>();

    RaycastHit hit;
    GameObject cubeClicked = null;

    public void ResetEntryPoint()
    {
        foreach (var gameObj in goListCube)
        {
            gameObj.GetComponent<Outline>().enabled = false;
        }
    }
    void ResetLR()
    {
        PosCube = new List<Transform>();        
    }

    void Start()
    {
        PathLine = GetComponent<LineRenderer>();
        allCube = GameObject.FindGameObjectsWithTag("Cube");
        goListCube = new List<GameObject>(allCube);
        ResetEntryPoint();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform != null)
                {
                    cubeClicked = hit.transform.gameObject;
                    if (cubeClicked.name != "Terrain")
                    {
                        ResetEntryPoint();
                        ResetLR();
                        cubeClicked.GetComponent<Outline>().enabled = true;

                        GameObject cubeToPath = null;
                        GameObject goListCubeParam = null;
                        GameObject firstCubePathed = null;
                        GameObject lastCubePathed = null;
                        GameObject nextCubeToPathed = null;

                        firstCubePathed = DoPathCube(cubeClicked, goListCube);
                        DoPathCube(cubeClicked, goListCube);

                        var goListRemainCube = new List<GameObject>(goListCube);
                        goListRemainCube.Remove(cubeClicked);
                        goListRemainCube.Remove(firstCubePathed);
                        nextCubeToPathed = firstCubePathed;

                        var goListRemainCube2 = new List<GameObject>(goListRemainCube);
                        for (int i = 0; i < goListRemainCube.Count; i++)
                        {
                            lastCubePathed = DoPathCube(nextCubeToPathed, goListRemainCube2);
                            nextCubeToPathed = lastCubePathed;
                            goListRemainCube2.Remove(nextCubeToPathed);
                        }
                        GameObject DoPathCube(GameObject cubeToPath, List<GameObject> goListCubeParam)
                        {
                            float nearDistTest;
                            var testDist = float.MaxValue;
                            GameObject nearObj = null;
                            for (int i = 0; i < goListCubeParam.Count; i++)
                            {
                                nearDistTest = Vector3.Distance(cubeToPath.transform.position, goListCubeParam[i].transform.position);
                                if (nearDistTest < testDist && goListCubeParam[i] != cubeToPath)
                                {
                                    testDist = nearDistTest;
                                    nearObj = goListCubeParam[i];
                                }
                            }
                            //print("Prochain CUBE le plus proche ? : " + nearObj);
                            //Debug.DrawLine(cubeToPath.transform.position, nearObj.transform.position, Color.red, 2.5f, false);
                            PosCube.Add(cubeToPath.transform);
                            PosCube.Add(nearObj.transform);
                            PathLine.positionCount = PosCube.Count;
                            PathLine.SetPosition(0, PosCube[0].position);
                            PathLine.SetPosition(1, PosCube[1].position);
                            for (int i = 0; i < PosCube.Count; i++)
                            {
                                PathLine.SetPosition(i, PosCube[i].position);
                            }
                            return nearObj;
                        }
                    }
                }
            }
        }
    }
}


