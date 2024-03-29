﻿using Nancy.Json;
using NeiroNetInterfaces.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace NeiroNetLogic.Services
{
    public class NeironService : INeironService
    {
        public const int NEIRON_IN_ARRAY_WIDTH = 10;        // количество по горизонтали
        public const int NEIRON_IN_ARRAY_HEIGHT = 10;       // количество по вертикали
        private const string MEMORY = "memory.txt";         // имя файла хранения сети
        private int[,] arr;

        private readonly List<Neiron> neironArray = null;   // массив нейронов

        public NeironService()
        {
            neironArray = InitWeb();
        }

        // функция сравнивает входной массив с каждым нейроном из сети и 
        // возвращает имя нейрона наиболее похожего на него
        // именно эта функция отвечает за распознавание образа

        public string CheckLitera(int[,] arr)
        {
            string res = null;
            double max = 0;

            foreach (var n in neironArray)
            {
                double d = n.GetRes(arr);

                if (d > max)
                {
                    max = d;
                    res = n.GetName();
                }
            }

            return res;
        }

        public string RecognizeText(int [,] arr)
        {
            
            //if (clipArr == null) return;
           // arr = NeiroGraphUtils.LeadArray(clipArr, new int[NeiroWeb.neironInArrayWidth, NeiroWeb.neironInArrayHeight]);
            //pictureBox2.Image = NeiroGraphUtils.GetBitmapFromArr(clipArr);
            //pictureBox3.Image = NeiroGraphUtils.GetBitmapFromArr(arr);
            string s = CheckLitera(arr);
            if (s == null) s = "null";
            //DialogResult askResult = MessageBox.Show("Результат распознавания - " + s + " ?", "", MessageBoxButtons.YesNo);
            //if (askResult != DialogResult.Yes || !enableTraining || MessageBox.Show("Добавить этот образ в память нейрона '" + s + "'", "", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            //SetTraining(s, arr);


            return s;
        }

        // функция сохраняет массив нейронов в файл
        public void SaveState()
        {
            JavaScriptSerializer json = new JavaScriptSerializer();

            string jStr = json.Serialize(neironArray);
            StreamWriter file = new StreamWriter(MEMORY);

            file.WriteLine(jStr);
            file.Close();
        }

        // получить список имён образов, имеющихся в памяти
        public string[] GetLiteras()
        {
            var res = new List<string>();

            for (int i = 0; i < neironArray.Count; i++)
            {
                res.Add(neironArray[i].GetName());
            }

            res.Sort();
            return res.ToArray();
        }

        // эта функция заносит в память нейрона с именем trainingName
        // новый вариант образа data
        public string SetTraining(string trainingName, int[,] data)
        {
            Neiron neiron = neironArray.Find(v => v.name.Equals(trainingName));

            if (neiron == null) // если нейрона с таким именем не существует, создадим новыи и добавим
            {                   // его в массив нейронов
                neiron = new Neiron();
                neiron.Clear(trainingName, NEIRON_IN_ARRAY_WIDTH, NEIRON_IN_ARRAY_HEIGHT);

                neironArray.Add(neiron);
            }

            int countTrainig = neiron.Training(data); // обучим нейрон новому образу

            string messageStr = $"Имя образа - {neiron.GetName()} вариантов образа в памяти - {countTrainig}";

            // покажем визуальное отображение памяти обученного нейрона
            //Form resultForm = new ShowMemoryVeight(neiron);
            //resultForm.Text = messageStr;
            //resultForm.Show();
            return messageStr;
        }

        // функция открывает текстовой файл и преобразовывает его в массив нейронов
        private static List<Neiron> InitWeb()
        {
            if (!File.Exists(MEMORY))
            {
                return new List<Neiron>();
            }

            string[] lines = File.ReadAllLines(MEMORY);

            if (lines.Length == 0)
            {
                return new List<Neiron>();
            }

            string jStr = lines[0];
            JavaScriptSerializer json = new JavaScriptSerializer();

            List<Object> objects = json.Deserialize<List<Object>>(jStr);
            List<Neiron> res = new List<Neiron>();

            foreach (var o in objects)
            {
                res.Add(NeironCreate((Dictionary<string, Object>)o));
            }

            return res;
        }

        // преобразовать структуру данных в клас нейрона
        private static Neiron NeironCreate(Dictionary<string, object> o)
        {
            Neiron res = new Neiron();

            res.name = (string)o["name"];
            res.countTrainig = (int)o["countTrainig"];
            Object[] veightData = (Object[])o["veight"];

            int arrSize = (int)Math.Sqrt(veightData.Length);
            res.veight = new double[arrSize, arrSize];

            int index = 0;

            for (int n = 0; n < res.veight.GetLength(0); n++)
            {
                for (int m = 0; m < res.veight.GetLength(1); m++)
                {
                    res.veight[n, m] = Double.Parse(veightData[index].ToString());
                    index++;
                }
            }

            return res;
        }
    }
}
