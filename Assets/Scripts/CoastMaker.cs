using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    North = 0,
    South = 1,
    East = 2,
    West = 3
};

public class CoastMaker {

    public static float[,] landmass;

    static int widthConst;
    public static int limit = 20;

    public static void InitLandmass(int width) {
        widthConst = width;
        landmass = new float[width, width];
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < width; j++) {
                landmass[i, j] = 0;
            }
        }
    }

    public static void CoastLineGenerate(Agent agent) {
        System.Random random = new System.Random();
        if (agent.tokens >= limit) {
            Agent[] c1 = new Agent[] { new Agent(), new Agent() };
            Agent c2 = new Agent();
            foreach (Agent a in c1) {
                int randomNumber = random.Next(0, 4);
                a.seedPos = GetNewPosInDir(agent.seedPos, (Direction)randomNumber);

                a.tokens = agent.tokens / 2;

                randomNumber = random.Next(0, 3);
                a.preferredDir = (Direction)(randomNumber);

                CoastLineGenerate(a);
            }
        } else {
            for (int t = 0; t < agent.tokens; t++) {
                int randomNumber = random.Next(0, 3);
                IntVec point = new IntVec(0,0);
                Direction dir = (Direction)randomNumber;
                //Possibly check no neighbours instead
                while(landmass[GetNewPosInDir(agent.seedPos, Direction.North).x, GetNewPosInDir(agent.seedPos, Direction.North).y] == 1
                      && landmass[GetNewPosInDir(agent.seedPos, Direction.South).x, GetNewPosInDir(agent.seedPos, Direction.South).y] == 1
                      && landmass[GetNewPosInDir(agent.seedPos, Direction.East).x, GetNewPosInDir(agent.seedPos, Direction.East).y] == 1
                      && landmass[GetNewPosInDir(agent.seedPos, Direction.West).x, GetNewPosInDir(agent.seedPos, Direction.West).y] == 1
                ) {
                    agent.seedPos = GetNewPosInDir(agent.seedPos, agent.preferredDir);
                }
                point = GetNewPosInDir(agent.seedPos, agent.preferredDir);
                while(landmass[point.x, point.y] == 1) {
                    dir = (Direction)((int)(dir + 1) % 4);
                    point = GetNewPosInDir(agent.seedPos, dir);
                    if(dir == agent.preferredDir) {
                        return;
                    }
                }
                landmass[point.x, point.y] = 1;
            }
        }
    }

    public static IntVec GetNewPosInDir(IntVec pos, Direction dir) {
        IntVec returnPos = new IntVec(0,0);
        if (dir == Direction.North) {
            returnPos.x = pos.x;
            returnPos.y = pos.y + 1;
        } else if (dir == Direction.South) {
            returnPos.x = pos.x;
            returnPos.y = pos.y - 1;
        } else if (dir == Direction.East) {
            returnPos.x = pos.x + 1;
            returnPos.y = pos.y;
        } else if (dir == Direction.West){
            returnPos.x = pos.x - 1;
            returnPos.y = pos.y;
        } else {
            returnPos.x = pos.x - 1;
            returnPos.y = pos.y;
           // Debug.Log("No direction, was: " + dir);
        }


        if(returnPos.x >= widthConst || returnPos.x < 0 || returnPos.y >= widthConst || returnPos.y < 0) {
            Debug.LogError("Value went outside area");
            return GetNewPosInDir(pos, (Direction)( ((int)dir+1)%4 ));
        }
        return returnPos;
    }
	
}

public class Agent {
    public IntVec seedPos;
    public Direction preferredDir;
    public int tokens;

    public Agent() {

    }
}

public class IntVec {
    public int x;
    public int y;

    public IntVec(int x, int y) {
        this.x = x;
        this.y = y;
    }
}
