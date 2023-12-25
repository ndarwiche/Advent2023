using System;
using System.Diagnostics;
using System.IO;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Advent2023.GearRatios
{
    public struct GearRatiosPart1Job : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] public readonly NativeArray<IntPtr> Lines;
        public readonly int LineSize;
        public NativeArray<int> LineNumberSum;

        public GearRatiosPart1Job(NativeArray<IntPtr> lines, int lineSize,
            NativeArray<int> lineNumberSum)
        {
            Lines = lines;
            LineSize = lineSize;
            LineNumberSum = lineNumberSum;
        }

        public unsafe void Execute(int index)
        {
            int currentNumber = 0;
            int sum = 0;
            var line = Lines[index];
            char* charPtr = (char*)line.ToPointer();
            bool isValidNumber = false;
            for (int i = 0; i < LineSize; i++)
            {
                char c = *(charPtr + i);
                if (char.IsDigit(c))
                {
                    currentNumber *= 10;
                    currentNumber += c - 48;
                    if (!isValidNumber && SearchNeighborsFordSymbol(index, i))
                        isValidNumber = true;
                }
                else if (currentNumber != 0)
                {
                    if (isValidNumber)
                        sum += currentNumber;
                    currentNumber = 0;
                    isValidNumber = false;
                }
            }

            if (isValidNumber)
                sum += currentNumber;
            LineNumberSum[index] = sum;
            //Debug.Log($"i = {index}, sum = {sum}");
        }

        private unsafe bool SearchNeighborsFordSymbol(int row, int column)
        {
            if (row > 0 && column > 0)
            {
                char northLeft = *((char*)Lines[row - 1].ToPointer() + column - 1);
                //46 is ascii for '.'
                if (!char.IsDigit(northLeft) && northLeft != 46)
                {
                    return true;
                }
            }

            if (row > 0)
            {
                char north = *((char*)Lines[row - 1].ToPointer() + column);
                if (!char.IsDigit(north) && north != 46)
                {
                    return true;
                }
            }

            if (row > 0 && column < LineSize - 1)
            {
                char northRight = *((char*)Lines[row - 1].ToPointer() + column + 1);
                if (!char.IsDigit(northRight) && northRight != 46)
                {
                    return true;
                }
            }

            if (column > 0)
            {
                char left = *((char*)Lines[row].ToPointer() + column - 1);
                if (!char.IsDigit(left) && left != 46)
                {
                    return true;
                }
            }

            if (column < LineSize - 1)
            {
                char right = *((char*)Lines[row].ToPointer() + column + 1);
                if (!char.IsDigit(right) && right != 46)
                {
                    return true;
                }
            }

            int linesLength = Lines.Length;
            if (row < linesLength - 1 && column > 0)
            {
                char southLeft = *((char*)Lines[row + 1].ToPointer() + column - 1);
                //46 is ascii for '.'
                if (!char.IsDigit(southLeft) && southLeft != 46)
                {
                    return true;
                }
            }

            if (row < linesLength - 1)
            {
                char south = *((char*)Lines[row + 1].ToPointer() + column);
                if (!char.IsDigit(south) && south != 46)
                {
                    return true;
                }
            }

            if (row < linesLength - 1 && column < LineSize - 1)
            {
                char southRight = *((char*)Lines[row + 1].ToPointer() + column + 1);
                if (!char.IsDigit(southRight) && southRight != 46)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public struct GearRatiosPart2Job : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] public readonly NativeArray<IntPtr> Lines;
        public readonly int LineSize;
        public NativeArray<int> GearRatios;

        public GearRatiosPart2Job(NativeArray<IntPtr> lines, int lineSize,
            NativeArray<int> gearRatios)
        {
            Lines = lines;
            LineSize = lineSize;
            GearRatios = gearRatios;
        }

        public unsafe void Execute(int index)
        {
            int sum = 0;
            var line = Lines[index];
            char* charPtr = (char*)line.ToPointer();
            for (int i = 0; i < LineSize; i++)
            {
                char c = *(charPtr + i);
                if (c != 42) continue;
                int x = SearchNeighborsForNumbers(index, i);
                sum += x;
            }

            GearRatios[index] = sum;
            //Debug.Log($"i = {index}, sum = {sum}");
        }

        private unsafe int SearchNeighborsForNumbers(int row, int column)
        {
            //Storing wether there's a number adjacent to the cell
            NativeArray<bool> firstRow = new NativeArray<bool>(3, Allocator.Domain);
            NativeArray<bool> secondRow = new NativeArray<bool>(3, Allocator.Domain);
            NativeArray<bool> thirdRow = new NativeArray<bool>(3, Allocator.Domain);

            if (row > 0 && column > 0)
            {
                char northLeft = *((char*)Lines[row - 1].ToPointer() + column - 1);

                if (char.IsDigit(northLeft))
                {
                    firstRow[0] = true;
                }
            }

            if (row > 0)
            {
                char north = *((char*)Lines[row - 1].ToPointer() + column);
                if (char.IsDigit(north))
                {
                    firstRow[1] = true;
                }
            }

            if (row > 0 && column < LineSize - 1)
            {
                char northRight = *((char*)Lines[row - 1].ToPointer() + column + 1);
                if (char.IsDigit(northRight))
                {
                    firstRow[2] = true;
                }
            }

            if (column > 0)
            {
                char left = *((char*)Lines[row].ToPointer() + column - 1);
                if (char.IsDigit(left))
                {
                    secondRow[0] = true;
                }
            }

            //the gear itself
            secondRow[1] = false;

            if (column < LineSize - 1)
            {
                char right = *((char*)Lines[row].ToPointer() + column + 1);
                if (char.IsDigit(right))
                {
                    secondRow[2] = true;
                }
            }

            int linesLength = Lines.Length;
            if (row < linesLength - 1 && column > 0)
            {
                char southLeft = *((char*)Lines[row + 1].ToPointer() + column - 1);
                if (char.IsDigit(southLeft))
                {
                    thirdRow[0] = true;
                }
            }

            if (row < linesLength - 1)
            {
                char south = *((char*)Lines[row + 1].ToPointer() + column);
                if (char.IsDigit(south))
                {
                    thirdRow[1] = true;
                }
            }

            if (row < linesLength - 1 && column < LineSize - 1)
            {
                char southRight = *((char*)Lines[row + 1].ToPointer() + column + 1);
                if (char.IsDigit(southRight))
                {
                    thirdRow[2] = true;
                }
            }

            int totalAdjacentNumber = CountNeighbors(ref firstRow);
            totalAdjacentNumber += CountNeighbors(ref secondRow);
            totalAdjacentNumber += CountNeighbors(ref thirdRow);

            if (totalAdjacentNumber != 2)
            {
                Debug.Log($"Non valid gear {row}, {column} , {totalAdjacentNumber}");
                return 0;
            }

            int firstNumber = 0;
            int secondNumber = 0;
            if (firstRow[0])
            {
                firstNumber = CalculateNumber(row - 1, column - 1);
            }
            else if (firstRow[1])
            {
                firstNumber = CalculateNumber(row - 1, column);
            }

            if (firstRow[2] && !firstRow[1])
            {
                if (firstNumber == 0)
                {
                    firstNumber = CalculateNumber(row - 1, column + 1);
                }
                else
                {
                    secondNumber = CalculateNumber(row - 1, column + 1);
                    return firstNumber * secondNumber;
                }
            }

            if (secondRow[0])
            {
                if (firstNumber == 0)
                {
                    firstNumber = CalculateNumber(row, column - 1);
                }
                else
                {
                    secondNumber = CalculateNumber(row, column - 1);
                    return firstNumber * secondNumber;
                }
            }

            if (secondRow[2])
            {
                if (firstNumber == 0)
                {
                    firstNumber = CalculateNumber(row, column + 1);
                }
                else
                {
                    secondNumber = CalculateNumber(row, column + 1);
                    return firstNumber * secondNumber;
                }
            }

            if (thirdRow[0])
            {
                if (firstNumber == 0)
                {
                    firstNumber = CalculateNumber(row + 1, column - 1);
                }
                else
                {
                    secondNumber = CalculateNumber(row + 1, column - 1);
                    return firstNumber * secondNumber;
                }
            }
            else if (thirdRow[1])
            {
                if (firstNumber == 0)
                {
                    firstNumber = CalculateNumber(row + 1, column);
                }
                else
                {
                    secondNumber = CalculateNumber(row + 1, column);
                    return firstNumber * secondNumber;
                }
            }

            if (thirdRow[2] && !thirdRow[1])
            {
                secondNumber = CalculateNumber(row + 1, column + 1);
            }
            if (firstNumber>999 || secondNumber >999)
                Debug.Log($"r {row}, c {column}. 1 = {firstNumber}, 2 = {secondNumber}");
            return firstNumber * secondNumber;
        }

        private int CountNeighbors(ref NativeArray<bool> neighbors)
        {
            int numberCount = 0;
            if (neighbors[0])
                numberCount = 1;
            if (neighbors[1])
                numberCount = 1;
            if (neighbors[2])
                numberCount = neighbors[1] ? 1 : numberCount + 1;
            return numberCount;
        }

        private unsafe int CalculateNumber(int row, int column)
        {
            int result = *((char*)Lines[row].ToPointer() + column) - 48;
            int currentColumn = column - 1;
            int digitCounter = 1;
            //left
            while (currentColumn >= 0)
            {
                char previousChar = *((char*)Lines[row].ToPointer() + currentColumn);
                if (!char.IsDigit(previousChar))
                    break;
                result += (int)math.pow(10, digitCounter) * (previousChar - 48);
                digitCounter++;
                currentColumn--;
            }

            //right
            currentColumn = column + 1;
            while (currentColumn < LineSize)
            {
                char nextChar = *((char*)Lines[row].ToPointer() + currentColumn);
                if (!char.IsDigit(nextChar))
                    break;
                result *= 10;
                result += nextChar - 48;
                currentColumn++;
            }

            return result;
        }
    }

    public class GearRatioSolver : MonoBehaviour
    {
        public void SolvePart1()
        {
            var lines = File.ReadAllLines("Data/3/Input.txt");
            NativeArray<IntPtr> lineDatas = new(lines.Length, Allocator.TempJob);
            //Get a pointer for each line, Jobs will accept IntPtr as a native array type  
            unsafe
            {
                for (var i = 0; i < lines.Length; i++)
                {
                    fixed (char* charPtr = lines[i])
                    {
                        lineDatas[i] = (IntPtr)charPtr;
                    }
                }
            }

            var job = new GearRatiosPart1Job
            (
                lineDatas,
                lines[0].Length,
                new NativeArray<int>(lines.Length, Allocator.TempJob)
            );
            Stopwatch stopwatch = new();
            stopwatch.Start();
            job.Schedule(lines.Length, 16).Complete();
            //job.Run(lines.Length);
            stopwatch.Stop();
            Debug.Log("Elapsed ticks : " + stopwatch.ElapsedTicks);
            int result = 0;

            foreach (int lineSum in job.LineNumberSum)
            {
                result += lineSum;
            }

            Debug.Log("Result  : " + result);

            job.Lines.Dispose();
            job.LineNumberSum.Dispose();
        }


        public void SolvePart2()
        {
            var lines = File.ReadAllLines("Data/3/Input.txt");
            NativeArray<IntPtr> lineDatas = new(lines.Length, Allocator.TempJob);
            //Get a pointer for each line, Jobs will accept IntPtr as a native array type  
            unsafe
            {
                for (var i = 0; i < lines.Length; i++)
                {
                    fixed (char* charPtr = lines[i])
                    {
                        lineDatas[i] = (IntPtr)charPtr;
                    }
                }
            }

            var job = new GearRatiosPart2Job
            (
                lineDatas,
                lines[0].Length,
                new NativeArray<int>(lines.Length, Allocator.TempJob)
            );
            Stopwatch stopwatch = new();
            stopwatch.Start();
            job.Schedule(lines.Length, 16).Complete();
            //job.Run(lines.Length);
            int result = 0;

            foreach (int lineSum in job.GearRatios)
            {
                result += lineSum;
            }

            stopwatch.Stop();
            Debug.Log("Elapsed ticks : " + stopwatch.ElapsedTicks);
            Debug.Log("Result  : " + result);

            job.Lines.Dispose();
            job.GearRatios.Dispose();
        }
    }
}