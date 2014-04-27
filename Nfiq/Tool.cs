using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfiq
{
    public static partial class Nfiq
    {
        internal static void IncrementPointer(ref int column, ref int row, int width, int height, int p)
        {
            //if (x >= maxX || y >= maxY)
            //{
            //    throw new Exception(String.Format(
            //            "IncrementPointer: smth wrong. x = {0}, y = {1}, maxX = {2}, maxY = {3}, p = {4}",
            //            x, y, maxX, maxY, p));
            //}

            while (column + p >= width)
            {
                //if (y + 1 >= maxY || p - (maxX - x) < 0)
                //{
                //    throw new Exception(String.Format(
                //        "IncrementPointer: smth wrong. x = {0}, y = {1}, maxX = {2}, maxY = {3}, p = {4}",
                //        x, y, maxX, maxY, p));
                //}

                p = p - (width - column);
                column = 0;
                row++;
            }

            column = column + p;
        }

        internal static void DecrementPointer(ref int column, ref int row, int width, int height, int p)
        {
            //if (x >= maxX || y >= maxY)
            //{
            //    throw new Exception(String.Format(
            //            "DecrementPointer: smth wrong. x = {0}, y = {1}, maxX = {2}, maxY = {3}, p = {4}",
            //            x, y, maxX, maxY, p));
            //}

            while (column - p < 0)
            {
                //if (y + 1 >= maxY || p - (maxX - x) < 0)
                //{
                //    throw new Exception(String.Format(
                //        "DecrementPointer: smth wrong. x = {0}, y = {1}, maxX = {2}, maxY = {3}, p = {4}",
                //        x, y, maxX, maxY, p));
                //}

                p = p - (column + 1);
                column = width - 1;
                row--;
            }

            column = column - p;
        }

        // x = column, y = row
        internal static int GetValueOfArray(int[,] array, int column, int row, int p)
        {
            if (p > 0)
            {
                IncrementPointer(ref column, ref row, array.GetLength(0), array.GetLength(1), p);
            }
            else
            {
                DecrementPointer(ref column, ref row, array.GetLength(0), array.GetLength(1), p * (-1));
            }

            return array[column, row];
        }

        internal static void PrintMap(int[,] map, string filename)
        {
            string path = Resources.ImagePath + filename + Resources.Txt;
            StreamWriter sw = new StreamWriter(path);

            for (int i = 0; i < map.GetLength(1); i++)
            {
                for (int j = 0; j < map.GetLength(0); j++)
                {
                    sw.Write("{0} ", map[j,i]);
                }

                sw.Write("\n");
            }

            sw.Close();
            OpenResult(path);
        }

        internal static void PrintMap(int[] map, string filename)
        {
            string path = Resources.ImagePath + filename + Resources.Txt;
            StreamWriter sw = new StreamWriter(path);

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    sw.Write("{0} ", map[i]);
                }

                sw.Write("\n");
            }

            sw.Close();
            OpenResult(path);
        }

        private static void OpenResult(string path)
        {
            //Process.Start(path);
        }
    }
}
