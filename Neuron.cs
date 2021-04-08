using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Neuron 
{
    public double[] weights;
    public int minimum = 50;
    int Neurolength;
    bool randomized = true;

    public Neuron(int length)
    {
        Neurolength=length;
        weights= new double[length];
        if (randomized)
        {
            randomizeWeights();
        } 
        else
        {
            FileWeights();
        }
        
    }
    void randomizeWeights()
	{
		for(int c = 0; c < Neurolength;c++){
            weights[c] = (UnityEngine.Random.Range(-100,100));
            weights[c] /= 100;
        }
			//weights[c] = Math.Round((UnityEngine.Random.Range(-.5f,.5f)), 2);

	}
    void FileWeights()
    {
        
    }
    public double GetResult(double[] input) 
    {
        double result=0;
        for(int c = 0; c < Neurolength;c++)
        {
            result+= input[c]*weights[c];
        }
        return result;
    }
}
