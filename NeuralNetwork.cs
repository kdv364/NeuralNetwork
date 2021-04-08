using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class NeuralNetwork : MonoBehaviour
{
    string writePath = @"D:\UN\FirstGAEM\Saves\WeightSaves\";
    string intresting_number="";
    public int Example_num = 1;
    int epoch_number = 50;
    public double learnigRate = 0.1;
    double lambda_reg = 0.01;
    public List<LearningData> LearningList = new List<LearningData> {};
    Neuron[] Layer1;
    Neuron[] LayerEnd;
    int inputLength;
    int resultLength=19;
    int layer1Length=500;
    bool isLearning=false;
    string[] spells=new string[3] {
            "0_ligthbolt",
            "1_meteor",
            "2_napalm"};
    
    public int result_index=0;

    // Start is called before the first frame update
    void Start()
    {
         
        Layer1= new Neuron[layer1Length];
        LayerEnd= new Neuron[resultLength];
        inputLength=GetComponent<Draw1>().quad;
        //Debug.Log(inputLength);
        for (int i = 0; i < layer1Length; i++)
        {
            Layer1[i]=new Neuron(inputLength);
            //Debug.Log((Layer1[i].weights[97]) + "");
        }
        for (int i = 0; i < resultLength; i++)
        {
            LayerEnd[i]=new Neuron(layer1Length);
        }
        //viewWeights();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown (KeyCode.Alpha3))
        {
            SaveWeights();
        }
        if (Input.GetKeyDown (KeyCode.Alpha4))
        {
            ReadWeights();
        }
        if (Input.GetKeyDown (KeyCode.S))
        {
            ChangeLearningFunction();
        }
        if (Input.GetKeyDown (KeyCode.V))
        {
            GetComponent<Draw1>().ClearCells();
            GetComponent<Draw1>().FromNeuro(LearningList[LearningList.Count-Example_num].data);
            Example_num++;
        }
        if (Input.GetKeyDown (KeyCode.Alpha7))
        {
            Debug.Log("List information:");
            Debug.Log("List length: " + LearningList.Count);
            foreach (LearningData item in LearningList)
            //{
                //intresting_number+=" "+item.result;
            //}
            Debug.Log(LearningList.Aggregate("",(total, i)=>total+" "+i.result));
            //Debug.Log("List content:" + intresting_number);
        }

        if (Input.GetKeyDown (KeyCode.R))
        {
            ChangeFigureNumber();
        }
        if (Input.GetKeyDown (KeyCode.Z))
        {
            Debug.Log("Data Processing...");
            ProcessingData();
        }

    }
    double[] Neurostart(double[] input)
    {
        double[] output = new double[layer1Length];
        for (int i = 0; i < layer1Length; i++)
        {
            //output[i] = Math.Round(Sigmoid(Layer1[i].GetResult(input)), 2);
            output[i] = Sigmoid(Layer1[i].GetResult(input));
        }
        return output;
    }
    double[] Neuroend(double[] input)
    {
        double[] output = new double[resultLength];
        for (int i = 0; i < resultLength; i++)
        {
            //output[i] = Math.Round(Sigmoid(LayerEnd[i].GetResult(input)), 2);
            output[i] = Sigmoid(LayerEnd[i].GetResult(input));
        }
        return output;
    }
    double Sigmoid(double value)
    {
        return 1.0f / (1.0f +  Mathf.Exp(Convert.ToSingle(-value)));
    }
    void NeuroLearn(double[] input1, double[] trueResult)
    {
        double[] input2 = Neurostart(input1);
        double[] result = Neuroend(input2);
        double[] weights_delta = new double[resultLength];
        double[] inside_error = new double[layer1Length];
        double[] inside_weights_delta = new double[layer1Length];
        for (int i = 0; i < resultLength; i++)
        {
            double error = result[i] - trueResult[i] ;
            weights_delta[i] = error * result[i] * (1-result[i]);
            for (int j = 0; j < layer1Length; j++)
            {
                LayerEnd[i].weights[j]-= learnigRate*(input2[j]*weights_delta[i]+lambda_reg*LayerEnd[i].weights[j]);
                inside_error[j] += LayerEnd[i].weights[j] * weights_delta[i]; 
            }
        }
        for (int i = 0; i < layer1Length; i++)
        {
            double error= inside_error[i];
            inside_weights_delta[i] = error * input2[i] * (1-input2[i]);
            for (int j = 0; j < inputLength; j++)
            {
                Layer1[i].weights[j]-= learnigRate*(input1[j]*inside_weights_delta[i]+lambda_reg*Layer1[i].weights[j]);
            }
        }
        
    }
    void viewWeights()
    {
        for (int i = 0; i < layer1Length; i++)
        {
            for (int j = 0; j < inputLength; j++)
            {
                //Debug.Log("1layer "+Layer1[i].weights[j]);
            }
        }
        for (int i = 0; i < resultLength; i++)
        {
            for (int j = 0; j < layer1Length; j++)
            {
                Debug.Log("2layer "+LayerEnd[i].weights[j]);
            }
        }
    }
    public void inputProcessing(double[] input)
    {
        double[] output;
        if(isLearning)
        {
            LearningList.Add(new LearningData(input, result_index));
            ChangeFigureNumber();
            
        } else
        {
            output = Neuroend(Neurostart(input));
            for (int i = 0; i < resultLength; i++)
            {
                Debug.Log(i+":" +output[i]);
            }
        }
    }
    void ProcessingData()
    { 
        double[] output;
        double average_error=0;
        for (int j = 0; j < epoch_number; j++)
        {
            for (int i = 0; i < LearningList.Count; i++)
            {
                NeuroLearn(LearningList[i].data, trueResultes(LearningList[i].result));   
            }
        }
        for (int i = 0; i < LearningList.Count; i++)
            {
                output = Neuroend(Neurostart(LearningList[i].data));
                average_error+= (trueResultes(LearningList[i].result)[LearningList[i].result]-output[LearningList[i].result]);
            }
        Debug.Log("Done!");
        Debug.Log("Error:" + average_error/LearningList.Count);
    }
    double[] trueResultes(int index)
    {
        double[] outvector = new double[resultLength];
        outvector[index] = 1f;
        return outvector;
    }
    public void ChangeLearningFunction()
    {
        isLearning= !isLearning;
            if (isLearning)
            {
                Debug.Log("System is learning");
            } else
            {
                Debug.Log("System is working");
            }
    }
    public void ChangeFigureNumber()
    {
        if(result_index<resultLength-1){
                result_index++;
            } else
            {
                result_index=0;
            }
            Debug.Log(result_index);
    }
    public void DeleteLast()
    {
        LearningList.RemoveAt(LearningList.Count-1);
    }
    void SaveWeights()
    {
        using (StreamWriter sw = new StreamWriter((writePath+"weights"+".txt"), false, System.Text.Encoding.Default))
                {
                    foreach (Neuron item in Layer1)
                    {
                        for (int i = 0; i < inputLength; i++)
                        {
                            sw.WriteLine(item.weights[i]);
                        }
                    }
                    foreach (Neuron item in LayerEnd)
                    {
                        for (int i = 0; i < layer1Length; i++)
                        {
                            sw.WriteLine(item.weights[i]);
                        }
                    }
                    
                }
        Debug.Log("Weights are saved");
    }
    void ReadWeights()
    {
        using (StreamReader sr = new StreamReader((writePath+"weights"+".txt")))
                {
                    foreach (Neuron item in Layer1)
                    {
                        for (int i = 0; i < inputLength; i++)
                        {
                            item.weights[i]=Convert.ToDouble(sr.ReadLine());
                        }
                    }
                    foreach (Neuron item in LayerEnd)
                    {
                        for (int i = 0; i < layer1Length; i++)
                        {
                            item.weights[i]=Convert.ToDouble(sr.ReadLine());
                        }
                    }
                }
        Debug.Log("Weights are in system");
        /*using (StreamReader sr = new StreamReader((writePath+"1"+".txt")))
                {
                    Debug.Log("ss" + (1+Convert.ToDouble(sr.ReadLine())));
                }*/
    }
}
