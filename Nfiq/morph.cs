using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfiq
{
  public static partial class Nfiq
  {
    /// <summary>
    /// Меняет в 8 битном изображении 0 на 1, 
    /// если любой из их 4 соседей != 0. 
    /// За распределение выходного изображения ответственнен вызывающий :))) 
    /// Входное изображение остается неизменным.
    /// </summary>
    /// <param name="inp">8 битное изображение, которое должно быть раширено</param>
    /// <param name="outp">результат</param>
    /// <param name="iw">ширина</param>
    /// <param name="ih">высота</param>
    internal static void dilate_charimage_2(ref int[,] inp, ref int[,] outp, int iw, int ih)
    {
      for (int i = 0; i < iw; i++)
      {
        for (int j = 0; j < ih; j++)
        {
          outp[i, j] = inp[i, j];
        }
      }

      /* for all pixels. set pixel if there is at least one true neighbor */
      for (int row = 0; row < ih; row++)
      {
        for (int col = 0; col < iw; col++)
        {
          // раньше было написано inp[col, row] != 0 
          // но так кажется неправильно

          // смотрим на нулевые пиксели: имеются ли ненулевые соседи?
          if (inp[col, row] == 0) /* pixel is already true, neighbors irrelevant */
          {
            /* more efficient with C's left to right evaluation of     */
            /* conjuctions. E N S functions not executed if W is false */
            if (get_west8_2(inp, row, col, 0) != 0 ||
             get_east8_2(inp, row, col, iw, 0) != 0 ||
             get_north8_2(inp, row, col, iw, 0) != 0 ||
             get_south8_2(inp, row, col, iw, ih, 0) != 0)
            {
              outp[col, row] = 1;
            }
          }
        }
      }
    }

    internal static void erode_charimage_2(ref int[,] inp, ref int[,] outp, int iw, int ih)
    {
      for (int i = 0; i < iw; i++)
      {
        for (int j = 0; j < ih; j++)
        {
          outp[i, j] = inp[i, j];
        }
      }

      /* for true pixels. kill pixel if there is at least one false neighbor */
      for (int row = 0; row < ih; row++)
      {
        for (int col = 0; col < iw; col++)
        {
          if (inp[col, row] != 0) /* erode only operates on true pixels */
          {
            /* more efficient with C's left to right evaluation of     */
            /* conjuctions. E N S functions not executed if W is false */
            if (get_west8_2(inp, row, col, 1) == 0 ||
                  get_east8_2(inp, row, col, iw, 1) == 0 ||
                  get_north8_2(inp, row, col, iw, 1) == 0 ||
                  get_south8_2(inp, row, col, iw, ih, 1) == 0)
            {
              outp[col, row] = 0;
            }
          }
        }
      }
    }

    internal static int get_west8_2(int[,] arr, int row, int col, int failcode)
    {
      if (col < 1) /* catch case where image is undefined westwards     */
      {
        return failcode; /* use plane geometry and return code.       */
      }

      return GetValueOfArray(arr, col, row, -1);
    }

    internal static int get_east8_2(int[,] arr, int row, int col, int iw, int failcode)
    {
      if (col >= iw - 1) /* catch case where image is undefined eastwards    */
        return failcode; /* use plane geometry and return code.           */

      return GetValueOfArray(arr, col, row, 1);
    }

    internal static int get_north8_2(int[,] arr, int row, int col, int iw, int failcode)
    {
      if (row < 1)     /* catch case where image is undefined northwards   */
        return failcode; /* use plane geometry and return code.           */

      return GetValueOfArray(arr, col, row, -iw);
    }

    internal static int get_south8_2(int[,] arr, int row, int col, int iw, int ih, int failcode)
    {
      if (row >= ih - 1) /* catch case where image is undefined southwards   */
        return failcode; /* use plane geometry and return code.           */

      return GetValueOfArray(arr, col, row, iw);
    }
  }
}
