using System;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using Advent2023.UI;
using Unity.Collections;
using Unity.Jobs;
using Debug = UnityEngine.Debug;

namespace Advent2023.CubeConundrum
{
    public struct CubeConundrumPart1Job : IJobParallelFor
    {
        public NativeArray<IntPtr> Lines;
        public NativeArray<int> LineSizes;
        public NativeArray<bool> IsValidArray;

        public unsafe void Execute(int index)
        {
            int currentNumber = 0;
            int redCount = 0;
            int blueCount = 0;
            int greenCount = 0;
            var line = Lines[index];
            char* charPtr = (char*) line.ToPointer();
            bool isValid = true;
            //Start from 8, so we can skip "Game x: " 
            for (int i = 8; i < LineSizes[index]; i++)
            {
                char c = *(charPtr + i);
                if (char.IsDigit(c))
                {
                    currentNumber *= 10;
                    currentNumber += c - 48;
                }
                else if (c == ' ')
                {
                    switch (*(charPtr + i + 1))
                    {
                        case 'r':
                            redCount += currentNumber;
                            //skip "red"
                            i += 3;
                            break;
                        case 'g':
                            greenCount += currentNumber;
                            //skip "green"
                            i += 5;
                            break;
                        case 'b':
                            blueCount += currentNumber;
                            //skip "blue"
                            i += 4;
                            break;
                    }

                    currentNumber = 0;
                }
                else if (c == ',')
                {
                    //skip ", " after red, green and blue
                    i++;
                }
                //end of current set
                else if (c == ';')
                {
                    isValid = redCount <= 12 && greenCount <= 13 && blueCount <= 14;
                    if (!isValid)
                    {
                        break;
                    }

                    //skip next space
                    i++;
                    //put back in the bag
                    redCount = 0;
                    greenCount = 0;
                    blueCount = 0;
                    currentNumber = 0;
                }
            }

            IsValidArray[index] = redCount <= 12 && greenCount <= 13 && blueCount <= 14;
            // if (!IsValidArray[index])
            // {
            //     Debug.Log($"i = {index}, r = {redCount}, g = {greenCount}, b = {blueCount}");
            // }
        }
    }

    public struct CubeConundrumPart2Job : IJobParallelFor
    {
        public NativeArray<IntPtr> Lines;
        public NativeArray<int> LineSizes;
        public NativeArray<int> CubePowers;

        public unsafe void Execute(int index)
        {
            int currentNumber = 0;
            int redCount = 0;
            int blueCount = 0;
            int greenCount = 0;
            int maxRed = 0;
            int maxGreen = 0;
            int maxBlue = 0;
            var line = Lines[index];
            char* charPtr = (char*) line.ToPointer();
            //Start from 8, so we can skip "Game x: " 
            for (int i = 8; i < LineSizes[index]; i++)
            {
                char c = *(charPtr + i);
                if (char.IsDigit(c))
                {
                    currentNumber *= 10;
                    currentNumber += c - 48;
                }
                else if (c == ' ')
                {
                    switch (*(charPtr + i + 1))
                    {
                        case 'r':
                            redCount += currentNumber;
                            //skip "red"
                            i += 3;
                            break;
                        case 'g':
                            greenCount += currentNumber;
                            //skip "green"
                            i += 5;
                            break;
                        case 'b':
                            blueCount += currentNumber;
                            //skip "blue"
                            i += 4;
                            break;
                    }

                    currentNumber = 0;
                }
                else if (c == ',')
                {
                    //skip ", " after red, green and blue
                    i++;
                }
                //end of current set
                else if (c == ';')
                {
                    maxRed = redCount > maxRed ? redCount : maxRed;
                    maxBlue = blueCount > maxBlue ? blueCount : maxBlue;
                    maxGreen = greenCount > maxGreen ? greenCount : maxGreen;
                    //skip next space
                    i++;
                    //put back in the bag
                    redCount = 0;
                    greenCount = 0;
                    blueCount = 0;
                    currentNumber = 0;
                }
            }
            //one last check for last set that doesn't end with ;
            maxRed = redCount > maxRed ? redCount : maxRed;
            maxBlue = blueCount > maxBlue ? blueCount : maxBlue;
            maxGreen = greenCount > maxGreen ? greenCount : maxGreen;
            CubePowers[index] = maxRed * maxGreen * maxBlue;
            //Debug.Log($"i = {index}, r = {maxRed}, g = {maxGreen}, b = {maxBlue}");
        }
    }

    public class CubeConundrumSolver : MonoBehaviour
    {
        public void SolvePart1()
        {
            var lines = File.ReadAllLines("Data/2/Input.txt");
            Stopwatch stopwatch = new();
            stopwatch.Start();
            NativeArray<IntPtr> lineDatas = new(lines.Length, Allocator.TempJob);
            //Get a pointer for each line, Jobs will accept IntPtr as a native array type  
            unsafe
            {
                for (var i = 0; i < lines.Length; i++)
                {
                    fixed (char* charPtr = lines[i])
                    {
                        lineDatas[i] = (IntPtr) charPtr;
                    }
                }
            }

            NativeArray<int> lineLengths = new(lines.Length, Allocator.TempJob);
            for (int i = 0; i < lines.Length; i++)
            {
                lineLengths[i] = lines[i].Length;
            }

            var job = new CubeConundrumPart1Job
            {
                Lines = lineDatas,
                LineSizes = lineLengths,
                IsValidArray = new(lines.Length, Allocator.TempJob),
            };
            job.Schedule(lines.Length, 16).Complete();
            //job.Run(lines.Length);

            //Sum the indices of the valid games
            int result = 0;
            for (int i = 0; i < job.Lines.Length; i++)
            {
                result += job.IsValidArray[i] ? i + 1 : 0;
            }

            job.Lines.Dispose();
            job.IsValidArray.Dispose();
            job.LineSizes.Dispose();
            stopwatch.Stop();
            ResultUi.Instance.ShowResult(stopwatch.ElapsedTicks, result);
        }


        public void SolvePart2()
        {
            var lines = File.ReadAllLines("Data/2/Input.txt");
            Stopwatch stopwatch = new();
            stopwatch.Start();
            
            NativeArray<IntPtr> lineDatas = new(lines.Length, Allocator.TempJob);
            //Get a pointer for each line, Jobs will accept IntPtr as a native array type  
            unsafe
            {
                for (var i = 0; i < lines.Length; i++)
                {
                    fixed (char* charPtr = lines[i])
                    {
                        lineDatas[i] = (IntPtr) charPtr;
                    }
                }
            }

            NativeArray<int> lineLengths = new(lines.Length, Allocator.TempJob);
            for (int i = 0; i < lines.Length; i++)
            {
                lineLengths[i] = lines[i].Length;
            }

            var job = new CubeConundrumPart2Job
            {
                Lines = lineDatas,
                LineSizes = lineLengths,
                CubePowers = new(lines.Length, Allocator.TempJob),
            };
            job.Schedule(lines.Length, 16).Complete();
            //job.Run(lines.Length);

            //Sum the power of cubes
            int result = 0;
            for (int i = 0; i < job.Lines.Length; i++)
            {
                result += job.CubePowers[i];
            }


            job.Lines.Dispose();
            job.CubePowers.Dispose();
            job.LineSizes.Dispose();
            stopwatch.Stop();
            ResultUi.Instance.ShowResult(stopwatch.ElapsedTicks, result);
        }
    }
}