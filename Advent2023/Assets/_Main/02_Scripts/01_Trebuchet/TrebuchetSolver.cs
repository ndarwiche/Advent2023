using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Advent2023.UI;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Advent2023.Trebuchet
{
    public class TrebuchetSolver : MonoBehaviour
    {
        public struct TrebuchetPart1Job : IJobParallelFor
        {
            public NativeArray<IntPtr> Lines;
            public NativeArray<int> LineSizes;
            public NativeArray<int> LineSumArray;

            public unsafe void Execute(int index)
            {
                var line = Lines[index];
                char* charPtr = (char*)line.ToPointer();
                bool isFirstFound = false;
                bool isLastFound = false;
                int digit1 = 0;
                int digit2 = 0;
                int startInc = 1;
                int endInc = 1;

                for (int start = 0, end = LineSizes[index] - 1; start <= end; start += startInc, end -= endInc)
                {
                    char left = *(charPtr + start);
                    char right = *(charPtr + end);
                    if (!isFirstFound && char.IsNumber(left))
                    {
                        isFirstFound = true;
                        startInc = 0;
                        digit1 = left - 48;
                    }

                    if (!isLastFound && char.IsNumber(right))
                    {
                        isLastFound = true;
                        endInc = 0;
                        digit2 = right - 48;
                    }

                    if (isFirstFound && isLastFound)
                        break;
                }

                LineSumArray[index] = digit1 * 10 + digit2;
            }
        }

        public void SolvePart1WithoutJobs()
        {
            Advent1 advent1 = new();
            ResultUi.Instance.ShowResult(0, advent1.Solve());
        }

        public void SolvePart1()
        {
            var lines = File.ReadAllLines("Data/1/Input.txt");
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
                        lineDatas[i] = (IntPtr)charPtr;
                    }
                }
            }

            NativeArray<int> lineLengths = new(lines.Length, Allocator.TempJob);
            for (int i = 0; i < lines.Length; i++)
            {
                lineLengths[i] = lines[i].Length;
            }

            var job = new TrebuchetPart1Job
            {
                Lines = lineDatas,
                LineSizes = lineLengths,
                LineSumArray = new(lines.Length, Allocator.TempJob),
            };
            job.Schedule(lines.Length, 16).Complete();
            //job.Run(lines.Length);

            //Sum the indices of the valid games
            int result = 0;
            for (int i = 0; i < job.Lines.Length; i++)
            {
                result += job.LineSumArray[i];
            }

            job.Lines.Dispose();
            job.LineSumArray.Dispose();
            job.LineSizes.Dispose();
            stopwatch.Stop();
            ResultUi.Instance.ShowResult(stopwatch.ElapsedTicks, result);
        }
    }
}