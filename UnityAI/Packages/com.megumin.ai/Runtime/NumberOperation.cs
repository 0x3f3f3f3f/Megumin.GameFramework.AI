using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megumin.AI
{
    /// <summary>
    /// 数字操作
    /// </summary>
    public enum NumberOperation
    {
        None,
        Zero,
        /// <summary>
        /// 自增
        /// </summary>
        Increment,
        /// <summary>
        /// 自减
        /// </summary>
        Decrement,
        /// <summary>
        /// 平方
        /// </summary>
        Square,

        AddTwo = 200,
        SubtractTwo,
        MultiplyTwo,
        DivideTwo,
        /// <summary>
        /// 对二取模
        /// </summary>
        ModTwo,
        /// <summary>
        /// 对二取余
        /// </summary>
        RemainderTwo,

        AddFive = 500,
        SubtractFive,
        MultiplyFive,
        DivideFive,
        /// <summary>
        /// 对五取模
        /// </summary>
        ModFive,
        /// <summary>
        /// 对五取余
        /// </summary>
        RemainderFive,

        AddTen = 1000,
        SubtractTen,
        MultiplyTen,
        DivideTen,
        /// <summary>
        /// 对十取模
        /// </summary>
        ModTen,
        /// <summary>
        /// 对十取余
        /// </summary>
        RemainderTen,
    }

    public static class NumberOperationExtension_AADFC3DCB07145D98FAC1BEE263333B4
    {
        public static int OperateInt(this NumberOperation operation, int number)
        {
            int result = number;
            switch (operation)
            {
                case NumberOperation.None:
                    break;
                case NumberOperation.Zero:
                    result = 0;
                    break;
                case NumberOperation.Increment:
                    result++;
                    break;
                case NumberOperation.Decrement:
                    result--;
                    break;
                case NumberOperation.Square:
                    result *= result;
                    break;
                case NumberOperation.AddTwo:
                    result += 2;
                    break;
                case NumberOperation.SubtractTwo:
                    result -= 2;
                    break;
                case NumberOperation.MultiplyTwo:
                    result *= 2;
                    break;
                case NumberOperation.DivideTwo:
                    result /= 2;
                    break;
                case NumberOperation.ModTwo:
                    result /= 2;
                    break;
                case NumberOperation.RemainderTwo:
                    result %= 2;
                    break;
                case NumberOperation.AddFive:
                    result += 5;
                    break;
                case NumberOperation.SubtractFive:
                    result -= 5;
                    break;
                case NumberOperation.MultiplyFive:
                    result *= 5;
                    break;
                case NumberOperation.DivideFive:
                    result /= 5;
                    break;
                case NumberOperation.ModFive:
                    result /= 5;
                    break;
                case NumberOperation.RemainderFive:
                    result %= 5;
                    break;
                case NumberOperation.AddTen:
                    result += 10;
                    break;
                case NumberOperation.SubtractTen:
                    result -= 10;
                    break;
                case NumberOperation.MultiplyTen:
                    result *= 10;
                    break;
                case NumberOperation.DivideTen:
                    result /= 10;
                    break;
                case NumberOperation.ModTen:
                    result /= 10;
                    break;
                case NumberOperation.RemainderTen:
                    result %= 10;
                    break;
                default:
                    break;
            }

            return result;
        }
    }

}

