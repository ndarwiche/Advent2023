using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Advent2023.Helpers
{
    
    public static class FileHelper
    {
        public static byte[]  ReadAndGetLineIndices(string filePath, out  List<int>  lineIndices)
        {
            byte[] buffer = File.ReadAllBytes(filePath);
            lineIndices = new(1024);
            //first line
            lineIndices.Add(0);
            //data is in utf8 , each byte is 1 char
            for (int i = 0; i < buffer.Length - 1; i++)
            {
                if ((char)buffer[i] == '\n')
                {
                    lineIndices.Add(i);
                }
            }

            return buffer;
        }
    }

}